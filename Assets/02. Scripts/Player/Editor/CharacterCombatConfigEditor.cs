using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

/// <summary>
/// 애니메이션 클립에 찍힌 EventTiming 이벤트를 읽어 AnimationTimeline 에 굽는다.
///
/// 오써링은 애니메이션 이벤트로(디자이너 친화적), 런타임 판정은 구워진 수치로(결정론적).
/// 클립 안의 태그 문자열은 컴파일러가 검증할 수 없으므로 이 Bake 가 유일한 검증 게이트다.
/// 따라서 잘못된 오써링은 조용히 넘기지 않고 에러로 보고하고, 해당 원소는 기존 값을 그대로 둔다.
/// </summary>
[CustomEditor(typeof(CharacterCombatConfig))]
public class CharacterCombatConfigEditor : Editor
{
    private const string FieldStepGroups = "stepGroups";
    private const string FieldKey        = "key";
    private const string FieldSteps      = "steps";

    private const string FieldClip       = "clip";
    private const string FieldClipLength = "clipLength";
    private const string FieldMarkers    = "markers";
    private const string FieldTag        = "tag";
    private const string FieldStart      = "start";
    private const string FieldEnd        = "end";
    private const string FieldNote       = "note";

    private const string FieldCues           = "cues";
    private const string FieldOccurrence     = "occurrence";
    private const string FieldPhase          = "phase";
    private const string FieldSoundAddress   = "soundAddress";
    private const string FieldEffectAddress  = "effectAddress";

    /// <summary>주소 → 메인 에셋 타입. BakeAll 한 번당 한 번만 만든다 (큐마다 그룹을 훑으면 느리다).</summary>
    private Dictionary<string, Type> addressMap;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Timing Marker Bake", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox
        (
            $"클립의 '{AnimationMarkerReceiver.EventFunctionName}' 이벤트에서 타이밍 마커를 읽어옵니다.\n" +
            $"· 점   마커 : {AnimationMarkerReceiver.EventFunctionName}(\"{EAnimMarker.ComboDecision}\")\n" +
            $"· 구간 마커 : {AnimationMarkerReceiver.EventFunctionName}(\"{EAnimMarker.ComboInput}{AnimationMarkerReceiver.OpenSuffix}\")" +
            $" + {AnimationMarkerReceiver.EventFunctionName}(\"{EAnimMarker.ComboInput}{AnimationMarkerReceiver.CloseSuffix}\")\n" +
            $"다른 이름의 이벤트는 무시하므로 연출용 이벤트와 같은 클립에 공존할 수 있습니다.\n" +
            $"태그 오타 · 짝 안 맞음 등은 에러로 보고되고 해당 원소는 기존 값을 유지합니다.\n" +
            $"아래 버튼 하나로 '{FieldStepGroups}' 의 모든 그룹을 한 번에 굽습니다.\n\n" +
            $"'{FieldCues}' 는 덮어쓰지 않습니다 — 앵커가 실재하는 마커를 가리키는지, " +
            $"주소가 Addressables 에 등록되어 있는지 검증하고 시각 오름차순으로 정렬만 합니다.",
            MessageType.Info
        );

        var stepGroupsProp = serializedObject.FindProperty(FieldStepGroups);

        using (new EditorGUI.DisabledScope(stepGroupsProp == null || stepGroupsProp.arraySize == 0))
        {
            if (GUILayout.Button("클립에서 타이밍 마커 굽기 (Bake)"))
                BakeAll();
        }

        if (stepGroupsProp == null)
        {
            EditorGUILayout.HelpBox($"'{FieldStepGroups}' 필드를 찾지 못했습니다. CharacterCombatConfig 의 필드명이 바뀌었다면 이 에디터도 함께 갱신하세요.", MessageType.Warning);
            return;
        }

        for (int i = 0; i < stepGroupsProp.arraySize; ++i)
        {
            var groupProp = stepGroupsProp.GetArrayElementAtIndex(i);
            var stepsProp = groupProp.FindPropertyRelative(FieldSteps);

            if (stepsProp != null && stepsProp.arraySize == 0)
            {
                var key = (EAnimStepKey)groupProp.FindPropertyRelative(FieldKey).intValue;
                EditorGUILayout.HelpBox($"{key} 배열이 비어 있습니다.", MessageType.Warning);
            }
        }
    }

    // ─── Bake ────────

    /// <summary>
    /// key 별로 반드시 가져야 하는 점 마커 — 없으면 clipLength 로 폴백해 굽는다.
    /// (없으면 HasPassed 가 영원히 false 라서 그 마커로 전이를 판정하는 스테이트가 못 빠져나간다)
    /// 그런 전이 판정이 없는 key(Dodge, Expert 등)는 null.
    /// </summary>
    private static EAnimMarker? RequiredMarkerFor(EAnimStepKey key)
    {
        switch (key)
        {
            case EAnimStepKey.NormalCombo: return EAnimMarker.ComboDecision;
            default:                       return null;
        }
    }

    private void BakeAll()
    {
        var log = new StringBuilder();
        log.AppendLine($"[CharacterCombatConfig] '{target.name}' 타이밍 마커 베이크 결과");

        addressMap = BuildAddressMap();

        if (addressMap == null)
            Debug.LogWarning("[CharacterCombatConfig] Addressables 설정을 찾지 못해 큐 주소 검증을 건너뜁니다.", target);

        var stepGroupsProp = serializedObject.FindProperty(FieldStepGroups);
        int totalFailed = 0;
        int totalCueFailed = 0;

        for (int i = 0; i < stepGroupsProp.arraySize; ++i)
        {
            var groupProp = stepGroupsProp.GetArrayElementAtIndex(i);
            var key       = (EAnimStepKey)groupProp.FindPropertyRelative(FieldKey).intValue;
            var stepsProp = groupProp.FindPropertyRelative(FieldSteps);

            log.AppendLine($" [{key}]");
            totalFailed += BakeTimelines(stepsProp, RequiredMarkerFor(key), log, ref totalCueFailed);
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssetIfDirty(target);

        addressMap = null;

        if (totalFailed > 0)
            log.AppendLine($"  → 마커 {totalFailed}개 실패. 해당 원소는 기존 값을 유지했습니다. 위의 에러 로그를 확인하세요.");

        // 큐는 굽는 대상이 아니라 애초에 덮어쓰지 않는다 — 마커 실패와 문장을 나눠 오해를 막는다.
        if (totalCueFailed > 0)
            log.AppendLine($"  → 큐 {totalCueFailed}건에 문제가 있습니다. 큐 데이터 자체는 그대로 두었습니다. 위의 에러 로그를 확인하세요.");

        Debug.Log(log.ToString(), target);
    }

    /// <summary>배열 하나를 굽는다. 실패한 원소 개수를 반환한다. (큐 문제는 cueFailed 로 따로 센다)</summary>
    private int BakeTimelines(SerializedProperty stepsProp, EAnimMarker? requiredMarker, StringBuilder log, ref int cueFailed)
    {
        int failed = 0;

        for (int i = 0; i < stepsProp.arraySize; ++i)
        {
            var timelineProp = stepsProp.GetArrayElementAtIndex(i);
            var clip         = timelineProp.FindPropertyRelative(FieldClip).objectReferenceValue as AnimationClip;

            if (clip == null)
            {
                Debug.LogWarning($"[CharacterCombatConfig] Step {i}: clip 이 비어 있어 건너뜁니다.", target);
                ++failed;
                continue;
            }

            if (TryCollectMarkers(clip, i, out List<AnimationMarker> markers) == false)
            {
                ++failed;
                continue;
            }

            if (requiredMarker.HasValue == true)
                EnsureRequiredMarker(clip, i, markers, requiredMarker.Value);

            // HasPassed 의 "가장 이른 것 기준" 의미와 note 보존이 모두 이 정렬 순서에 기댄다.
            markers.Sort((a, b) => a.start.CompareTo(b.start));

            timelineProp.FindPropertyRelative(FieldClipLength).floatValue = clip.length;
            WriteMarkers(timelineProp, markers);

            log.AppendLine($"  Step {i} [{clip.name}] length={clip.length:F3}");

            foreach (var marker in markers)
                log.AppendLine($"    · {Describe(marker)}");

            cueFailed += ProcessCues(timelineProp, markers, clip, i, log);
        }

        return failed;
    }

    /// <summary>클립의 EventTiming 이벤트를 마커 목록으로 번역한다. 오써링이 잘못됐으면 false.</summary>
    private bool TryCollectMarkers(AnimationClip clip, int index, out List<AnimationMarker> markers)
    {
        markers = new List<AnimationMarker>();

        var ranges = new Dictionary<EAnimMarker, List<RangeEvent>>();

        foreach (var evt in AnimationUtility.GetAnimationEvents(clip))
        {
            // EventTiming 이외의 이벤트(연출용 등)는 마커가 아니므로 무시한다.
            if (evt.functionName != AnimationMarkerReceiver.EventFunctionName)
                continue;

            if (TryParseTag(evt.stringParameter, out EAnimMarker tag, out bool hasSuffix, out bool isOpen) == false)
            {
                LogBakeError(clip, index, $"알 수 없는 마커 태그 '{evt.stringParameter}' (@{evt.time:F3}s)");
                return false;
            }

            if (AnimMarkerInfo.KindOf(tag) == EMarkerKind.Point)
            {
                if (hasSuffix == true)
                {
                    LogBakeError(clip, index, $"'{tag}' 는 점 마커라 접미사를 붙이지 않습니다 (@{evt.time:F3}s)");
                    return false;
                }

                markers.Add(new AnimationMarker { tag = tag, start = evt.time, end = evt.time });
                continue;
            }

            if (hasSuffix == false)
            {
                LogBakeError
                (
                    clip, index,
                    $"'{tag}' 는 구간 마커라 '{tag}{AnimationMarkerReceiver.OpenSuffix}' / " +
                    $"'{tag}{AnimationMarkerReceiver.CloseSuffix}' 로 찍어야 합니다 (@{evt.time:F3}s)"
                );
                return false;
            }

            if (ranges.TryGetValue(tag, out List<RangeEvent> list) == false)
            {
                list = new List<RangeEvent>();
                ranges.Add(tag, list);
            }

            list.Add(new RangeEvent(evt.time, isOpen));
        }

        foreach (var pair in ranges)
        {
            if (TryPairRange(clip, index, pair.Key, pair.Value, markers) == false)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 같은 태그의 .Open / .Close 를 시간순으로 짝지어 구간 마커로 접는다.
    /// 같은 태그가 여러 번 나오는 것은 허용한다 — Open → Close 로 번갈아 나오기만 하면 된다.
    /// </summary>
    private bool TryPairRange(AnimationClip clip, int index, EAnimMarker tag, List<RangeEvent> events, List<AnimationMarker> markers)
    {
        events.Sort((a, b) =>
        {
            int byTime = a.time.CompareTo(b.time);

            // 같은 시각이면 Open 을 먼저 둔다 — 아래 구간 길이 검사에서 명확한 에러로 잡히게.
            return byTime != 0 ? byTime : b.isOpen.CompareTo(a.isOpen);
        });

        if (events.Count % 2 != 0)
        {
            LogBakeError(clip, index, $"'{tag}' 의 {AnimationMarkerReceiver.OpenSuffix}/{AnimationMarkerReceiver.CloseSuffix} 짝이 맞지 않습니다 (이벤트 {events.Count}개)");
            return false;
        }

        for (int i = 0; i < events.Count; i += 2)
        {
            RangeEvent open  = events[i];
            RangeEvent close = events[i + 1];

            if (open.isOpen == false || close.isOpen == true)
            {
                LogBakeError
                (
                    clip, index,
                    $"'{tag}' 의 마커가 {AnimationMarkerReceiver.OpenSuffix} → {AnimationMarkerReceiver.CloseSuffix} 순으로 " +
                    $"번갈아 나오지 않습니다 (@{open.time:F3}s 부근)"
                );
                return false;
            }

            if (open.time < close.time == false)
            {
                LogBakeError(clip, index, $"'{tag}' 의 구간 길이가 0 입니다 (@{open.time:F3}s). {AnimationMarkerReceiver.CloseSuffix} 를 뒤로 옮기세요");
                return false;
            }

            markers.Add(new AnimationMarker { tag = tag, start = open.time, end = close.time });
        }

        return true;
    }

    /// <summary>
    /// requiredMarker 가 없으면 clipLength 를 그 시점으로 폴백한다.
    /// 없는 채로 두면 HasPassed 가 영원히 false 라서 그 마커로 전이를 판정하는 스테이트가 못 빠져나간다.
    /// </summary>
    private void EnsureRequiredMarker(AnimationClip clip, int index, List<AnimationMarker> markers, EAnimMarker requiredMarker)
    {
        foreach (var marker in markers)
        {
            if (marker.tag == requiredMarker)
                return;
        }

        markers.Add(new AnimationMarker
        {
            tag   = requiredMarker,
            start = clip.length,
            end   = clip.length,
        });

        Debug.LogWarning
        (
            $"[CharacterCombatConfig] Step {index} ({clip.name}): '{requiredMarker}' 마커가 없어 " +
            $"clipLength({clip.length:F3}s) 를 판정 시점으로 사용합니다.",
            target
        );
    }

    // ─── Cues ────────

    /// <summary>
    /// 큐를 검증하고 시각 오름차순으로 정렬한다. 큐는 사람이 쓰는 데이터라 <b>내용을 덮어쓰지 않는다</b> — 원소 순서만 바꾼다.
    /// 문제가 있는 큐 개수를 반환한다.
    /// </summary>
    private int ProcessCues(SerializedProperty timelineProp, List<AnimationMarker> markers, AnimationClip clip, int index, StringBuilder log)
    {
        var cuesProp = timelineProp.FindPropertyRelative(FieldCues);

        if (cuesProp == null || cuesProp.arraySize == 0)
            return 0;

        // 앵커 해석은 런타임과 같은 코드를 쓴다 — 규칙이 갈라지면 로그가 보여준 시각과 실제 발화 시점이 어긋난다.
        var probe = new AnimationTimeline { markers = markers.ToArray() };
        var times = new List<float>(cuesProp.arraySize);

        int failed = 0;
        bool anchorsResolved = true;

        for (int i = 0; i < cuesProp.arraySize; ++i)
        {
            var          cueProp = cuesProp.GetArrayElementAtIndex(i);
            AnimationCue cue     = ReadCueAnchor(cueProp);

            if (probe.TryResolveCueTime(in cue, out float clipTime) == false)
            {
                LogBakeError
                (
                    clip, index,
                    $"cue {i}: '{cue.tag}' 마커의 {cue.occurrence}번째를 가리키는데 이 클립에는 {CountOf(markers, cue.tag)}개뿐입니다"
                );

                anchorsResolved = false;
                ++failed;
            }

            times.Add(clipTime);

            // 주소 검증은 앵커와 독립이라 앵커가 깨져도 계속 본다 — 한 번의 Bake 로 문제를 다 보여주는 게 낫다.
            if (ValidateAddress(cueProp.FindPropertyRelative(FieldSoundAddress).stringValue, typeof(AudioClip), "sound", clip, index, i) == false)
                ++failed;

            if (ValidateAddress(cueProp.FindPropertyRelative(FieldEffectAddress).stringValue, typeof(GameObject), "effect", clip, index, i) == false)
                ++failed;
        }

        // 앵커가 하나라도 끊겼으면 정렬하지 않는다. 시각을 모르는 원소를 섞어 넣으면
        // 순서가 반쯤 맞는 상태가 되어, 고친 뒤 다시 구워도 원래 의도를 복원하기 어렵다.
        if (anchorsResolved == false)
        {
            log.AppendLine($"    ▸ 큐 {cuesProp.arraySize}개 — 앵커가 끊겨 정렬을 건너뜁니다");
            return failed;
        }

        SortCuesByTime(cuesProp, times);

        for (int i = 0; i < cuesProp.arraySize; ++i)
            log.AppendLine($"    ▸ {DescribeCue(cuesProp.GetArrayElementAtIndex(i), times[i])}");

        return failed;
    }

    /// <summary>큐에서 앵커 세 필드만 읽는다. 페이로드는 검증 대상이 아니라 따로 본다.</summary>
    private static AnimationCue ReadCueAnchor(SerializedProperty cueProp)
    {
        return new AnimationCue
        {
            tag        = (EAnimMarker)cueProp.FindPropertyRelative(FieldTag).intValue,
            occurrence = cueProp.FindPropertyRelative(FieldOccurrence).intValue,
            phase      = (EAnimCuePhase)cueProp.FindPropertyRelative(FieldPhase).intValue,
        };
    }

    /// <summary>
    /// times 순서에 맞춰 큐 배열을 제자리 정렬한다. 드라이버가 커서로 발화하므로 이 순서가 곧 발화 순서다.
    ///
    /// 필드를 하나씩 복사하지 않고 MoveArrayElement 로 원소를 통째로 옮긴다 —
    /// AnimationCue 에 페이로드 필드가 추가돼도 이 코드는 손댈 필요가 없다.
    /// </summary>
    private static void SortCuesByTime(SerializedProperty cuesProp, List<float> times)
    {
        for (int i = 0; i < times.Count; ++i)
        {
            int min = i;

            for (int j = i + 1; j < times.Count; ++j)
            {
                if (times[j] < times[min])
                    min = j;
            }

            if (min == i)
                continue;

            cuesProp.MoveArrayElement(min, i);

            // MoveArrayElement 는 min 에서 빼서 i 에 끼워 넣는다. times 도 같은 연산으로 맞춘다.
            float time = times[min];
            times.RemoveAt(min);
            times.Insert(i, time);
        }
    }

    /// <summary>주소가 비어 있으면 그 항목을 쓰지 않는다는 뜻이라 통과시킨다.</summary>
    private bool ValidateAddress(string address, Type expectedType, string label, AnimationClip clip, int index, int cueIndex)
    {
        if (string.IsNullOrWhiteSpace(address) == true)
            return true;

        // Addressables 설정이 없는 프로젝트 상태 — BakeAll 에서 이미 한 번 경고했다.
        if (addressMap == null)
            return true;

        if (addressMap.TryGetValue(address.Trim(), out Type actualType) == false)
        {
            LogBakeError(clip, index, $"cue {cueIndex}: {label} 주소 '{address}' 가 Addressables 에 등록되어 있지 않습니다");
            return false;
        }

        if (actualType != null && expectedType.IsAssignableFrom(actualType) == false)
        {
            LogBakeError(clip, index, $"cue {cueIndex}: {label} 주소 '{address}' 의 타입이 {actualType.Name} 입니다 ({expectedType.Name} 이어야 합니다)");
            return false;
        }

        return true;
    }

    /// <summary>등록된 모든 주소 → 메인 에셋 타입. 폴더 엔트리는 내부 에셋까지 펼친다.</summary>
    private static Dictionary<string, Type> BuildAddressMap()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
            return null;

        var map    = new Dictionary<string, Type>();
        var buffer = new List<AddressableAssetEntry>();

        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group == null)
                continue;

            foreach (AddressableAssetEntry entry in group.entries)
            {
                if (entry == null)
                    continue;

                buffer.Clear();
                entry.GatherAllAssets(buffer, true, true, false);

                foreach (AddressableAssetEntry asset in buffer)
                {
                    if (asset.IsFolder == true || string.IsNullOrEmpty(asset.address) == true)
                        continue;

                    // 같은 주소가 여러 엔트리에 있어도 Bake 를 멈출 일은 아니다 — 마지막 것으로 덮는다.
                    map[asset.address] = asset.MainAssetType;
                }
            }
        }

        return map;
    }

    private static int CountOf(List<AnimationMarker> markers, EAnimMarker tag)
    {
        int count = 0;

        foreach (AnimationMarker marker in markers)
        {
            if (marker.tag == tag)
                ++count;
        }

        return count;
    }

    private static string DescribeCue(SerializedProperty cueProp, float clipTime)
    {
        AnimationCue cue = ReadCueAnchor(cueProp);

        string sound  = cueProp.FindPropertyRelative(FieldSoundAddress).stringValue;
        string effect = cueProp.FindPropertyRelative(FieldEffectAddress).stringValue;
        var    text   = new StringBuilder($"{cue.tag}[{cue.occurrence}].{cue.phase} @{clipTime:F3}");

        if (string.IsNullOrWhiteSpace(sound) == false)
            text.Append($"  sound={sound}");

        if (string.IsNullOrWhiteSpace(effect) == false)
            text.Append($"  effect={effect}");

        if (string.IsNullOrWhiteSpace(sound) == true && string.IsNullOrWhiteSpace(effect) == true)
            text.Append("  (페이로드 없음)");

        return text.ToString();
    }

    // ─── Serialization ────────

    private void WriteMarkers(SerializedProperty timelineProp, List<AnimationMarker> markers)
    {
        var markersProp = timelineProp.FindPropertyRelative(FieldMarkers);
        var notes       = SnapshotNotes(markersProp);
        var occurrences = new Dictionary<EAnimMarker, int>();

        markersProp.arraySize = markers.Count;

        for (int i = 0; i < markers.Count; ++i)
        {
            AnimationMarker marker = markers[i];

            occurrences.TryGetValue(marker.tag, out int occurrence);
            occurrences[marker.tag] = occurrence + 1;

            var elementProp = markersProp.GetArrayElementAtIndex(i);

            elementProp.FindPropertyRelative(FieldTag).intValue     = (int)marker.tag;
            elementProp.FindPropertyRelative(FieldStart).floatValue = marker.start;
            elementProp.FindPropertyRelative(FieldEnd).floatValue   = marker.end;
            elementProp.FindPropertyRelative(FieldNote).stringValue = TakeNote(notes, marker.tag, occurrence);
        }
    }

    /// <summary>
    /// 굽기 전의 note 를 태그별 등장 순서대로 보관한다.
    /// note 는 사람이 손으로 쓰는 메모라서, markers 배열을 통째로 덮어쓰는 Bake 에 날아가면 안 된다.
    /// </summary>
    private Dictionary<EAnimMarker, List<string>> SnapshotNotes(SerializedProperty markersProp)
    {
        var notes = new Dictionary<EAnimMarker, List<string>>();

        for (int i = 0; i < markersProp.arraySize; ++i)
        {
            var elementProp = markersProp.GetArrayElementAtIndex(i);
            var tag         = (EAnimMarker)elementProp.FindPropertyRelative(FieldTag).intValue;

            if (notes.TryGetValue(tag, out List<string> list) == false)
            {
                list = new List<string>();
                notes.Add(tag, list);
            }

            list.Add(elementProp.FindPropertyRelative(FieldNote).stringValue);
        }

        return notes;
    }

    private static string TakeNote(Dictionary<EAnimMarker, List<string>> notes, EAnimMarker tag, int occurrence)
    {
        if (notes.TryGetValue(tag, out List<string> list) == true && occurrence < list.Count)
            return list[occurrence];

        return string.Empty;
    }

    // ─── Helpers ────────

    /// <summary>"ComboInput.Open" → (ComboInput, hasSuffix: true, isOpen: true)</summary>
    private static bool TryParseTag(string raw, out EAnimMarker tag, out bool hasSuffix, out bool isOpen)
    {
        tag       = EAnimMarker.None;
        hasSuffix = false;
        isOpen    = false;

        if (string.IsNullOrWhiteSpace(raw) == true)
            return false;

        string name = raw.Trim();

        if (name.EndsWith(AnimationMarkerReceiver.OpenSuffix, StringComparison.OrdinalIgnoreCase) == true)
        {
            hasSuffix = true;
            isOpen    = true;
            name      = name.Substring(0, name.Length - AnimationMarkerReceiver.OpenSuffix.Length);
        }
        else if (name.EndsWith(AnimationMarkerReceiver.CloseSuffix, StringComparison.OrdinalIgnoreCase) == true)
        {
            hasSuffix = true;
            isOpen    = false;
            name      = name.Substring(0, name.Length - AnimationMarkerReceiver.CloseSuffix.Length);
        }

        // Enum.TryParse 는 숫자 문자열("1")도 통과시키므로 오타가 조용히 매핑되지 않게 막는다.
        if (name.Length == 0 || char.IsLetter(name[0]) == false)
            return false;

        if (Enum.TryParse(name, true, out tag) == false)
            return false;

        return tag != EAnimMarker.None;
    }

    private static string Describe(in AnimationMarker marker)
    {
        return marker.IsPoint == true
            ? $"{marker.tag} @{marker.start:F3}"
            : $"{marker.tag} {marker.start:F3}~{marker.end:F3}";
    }

    private void LogBakeError(AnimationClip clip, int index, string message)
    {
        Debug.LogError($"[CharacterCombatConfig] Step {index} ({clip.name}): {message}", target);
    }

    /// <summary>짝짓기 전의 구간 마커 이벤트 하나.</summary>
    private readonly struct RangeEvent
    {
        public readonly float time;
        public readonly bool  isOpen;

        public RangeEvent(float inTime, bool inIsOpen)
        {
            time   = inTime;
            isOpen = inIsOpen;
        }
    }
}
