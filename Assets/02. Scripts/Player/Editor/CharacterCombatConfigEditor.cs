using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 애니메이션 클립에 찍힌 이벤트에서 콤보 타이밍을 읽어 Config 에 굽는다.
///
/// 오써링은 애니메이션 이벤트로(디자이너 친화적), 런타임 판정은 구워진 수치로(결정론적).
/// 클립 이벤트를 수정한 뒤 Bake 를 다시 누르면 Config 가 갱신된다.
/// </summary>
[CustomEditor(typeof(CharacterCombatConfig))]
public class CharacterCombatConfigEditor : Editor
{
    // 클립에 찍는 이벤트 함수 이름 규약
    private const string EventInputWindowStart = "SetCanInput";
    private const string EventInputWindowEnd   = "SetCannotInput";
    private const string EventComboDecision    = "HandleComboInput";

    private const string StepsProperty = "normalComboSteps";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var stepsProp = serializedObject.FindProperty(StepsProperty);
        if (stepsProp == null)
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Normal Combo Bake", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox
        (
            $"클립의 애니메이션 이벤트에서 콤보 타이밍을 읽어옵니다.\n" +
            $"· {EventInputWindowStart} → Input Window Start\n" +
            $"· {EventInputWindowEnd} → Input Window End\n" +
            $"· {EventComboDecision} → Combo Decision Time\n" +
            $"이벤트가 없으면 -1 로 기록됩니다.",
            MessageType.Info
        );

        using (new EditorGUI.DisabledScope(stepsProp.arraySize == 0))
        {
            if (GUILayout.Button("클립에서 콤보 타이밍 굽기 (Bake)"))
                BakeComboSteps(stepsProp);
        }

        if (stepsProp.arraySize == 0)
            EditorGUILayout.HelpBox("Normal Combo Steps 배열이 비어 있습니다.", MessageType.Warning);
    }

    private void BakeComboSteps(SerializedProperty stepsProp)
    {
        var log = new StringBuilder();
        log.AppendLine($"[CharacterCombatConfig] '{target.name}' 콤보 타이밍 베이크 결과");

        for (int i = 0; i < stepsProp.arraySize; ++i)
        {
            var stepProp = stepsProp.GetArrayElementAtIndex(i);
            var clipProp = stepProp.FindPropertyRelative("clip");
            var clip     = clipProp.objectReferenceValue as AnimationClip;

            if (clip == null)
            {
                Debug.LogWarning($"[CharacterCombatConfig] Step {i}: clip 이 비어 있어 건너뜁니다.", target);
                continue;
            }

            float windowStart  = FindEventTime(clip, EventInputWindowStart);
            float windowEnd    = FindEventTime(clip, EventInputWindowEnd);
            float decisionTime = FindEventTime(clip, EventComboDecision);

            if (decisionTime < 0f)
            {
                // 판정 시점이 없으면 콤보를 끊을 시점을 알 수 없다. 클립 끝으로 폴백.
                decisionTime = clip.length;
                Debug.LogWarning
                (
                    $"[CharacterCombatConfig] Step {i} ({clip.name}): '{EventComboDecision}' 이벤트가 없어 " +
                    $"clipLength({clip.length:F3}s) 를 판정 시점으로 사용합니다.",
                    target
                );
            }

            stepProp.FindPropertyRelative("clipLength").floatValue        = clip.length;
            stepProp.FindPropertyRelative("inputWindowStart").floatValue  = windowStart;
            stepProp.FindPropertyRelative("inputWindowEnd").floatValue    = windowEnd;
            stepProp.FindPropertyRelative("comboDecisionTime").floatValue = decisionTime;

            log.AppendLine
            (
                $"  Step {i} [{clip.name}] length={clip.length:F3} " +
                $"window={FormatTime(windowStart)}~{FormatTime(windowEnd)} " +
                $"decision={decisionTime:F3}"
            );
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssetIfDirty(target);

        Debug.Log(log.ToString(), target);
    }

    /// <summary>지정한 함수 이름의 이벤트 시간을 반환. 없으면 -1.</summary>
    private static float FindEventTime(AnimationClip clip, string functionName)
    {
        var events = AnimationUtility.GetAnimationEvents(clip);

        foreach (var evt in events)
        {
            if (evt.functionName == functionName)
                return evt.time;
        }

        return -1f;
    }

    private static string FormatTime(float time)
    {
        return time < 0f ? "none" : $"{time:F3}";
    }
}
