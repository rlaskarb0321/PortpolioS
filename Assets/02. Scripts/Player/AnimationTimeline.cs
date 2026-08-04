using System;
using UnityEngine;

public enum EAnimMarker
{
    None = 0,
    ComboInput,
    ComboDecision,
    Invincible,
    Hitbox,
    Trigger,
}

public enum EMarkerKind
{
    Point,
    Range,
}

/// <summary>
/// 태그별 메타데이터. Bake 의 검증 규칙이 여기 한 곳에 모인다.
/// 태그를 추가하면 KindOf 에 명시할 것 (명시하지 않으면 점 마커로 취급된다).
/// </summary>
public static class AnimMarkerInfo
{
    public static EMarkerKind KindOf(EAnimMarker tag)
    {
        switch (tag)
        {
            case EAnimMarker.ComboInput: return EMarkerKind.Range;
            case EAnimMarker.Invincible: return EMarkerKind.Range;
            case EAnimMarker.Hitbox:     return EMarkerKind.Range;

            case EAnimMarker.ComboDecision: return EMarkerKind.Point;
            case EAnimMarker.Trigger:       return EMarkerKind.Point;

            // 태그를 추가하면 위에 명시할 것.
            default: return EMarkerKind.Point;
        }
    }
}

/// <summary>
/// 베이크된 마커 하나. 점 마커는 start == end.
/// 직접 수정하지 말 것 — Bake 가 클립의 이벤트로 덮어쓴다. (note 만 보존된다)
/// </summary>
[Serializable]
public struct AnimationMarker
{
    public EAnimMarker tag;

    [Tooltip("구간 시작 (초). 점 마커는 end 와 같다")]
    public float start;

    [Tooltip("구간 끝 (초). 점 마커는 start 와 같다")]
    public float end;

    [Tooltip("에디터 가독성용 메모. 런타임 식별에는 쓰이지 않는다 (Bake 해도 보존됨)")]
    public string note;

    public bool IsPoint => Mathf.Approximately(start, end);
}

/// <summary>큐가 마커의 어느 시점에 붙는가. 점 마커는 start == end 라 어느 쪽을 골라도 같은 순간이다.</summary>
public enum EAnimCuePhase
{
    Start,
    End,
}

/// <summary>
/// 마커 시점에 터지는 연출 하나. 마커가 "언제" 라면 큐는 "무엇을" 이다.
///
/// 마커와 달리 사람이 인스펙터에서 직접 채운다 — Bake 는 이 배열을 덮어쓰지 않고,
/// 앵커가 실재하는 마커를 가리키는지 검증하고 시각 오름차순으로 정렬만 한다.
/// </summary>
[Serializable]
public struct AnimationCue
{
    // ─── 앵커 : 어느 마커의 어느 시점에 붙는가 ────────

    [Tooltip("이 큐가 붙을 마커의 태그")]
    public EAnimMarker tag;

    [Tooltip("같은 태그의 마커가 여럿일 때 몇 번째인가 (0-based, markers 의 시각 오름차순 기준). 하나뿐이면 0")]
    public int occurrence;

    [Tooltip("구간 마커의 시작/끝 중 어디서 터질지. 점 마커는 어느 쪽이든 같다")]
    public EAnimCuePhase phase;

    // ─── 페이로드 : 무엇을 할 것인가 ────────

    [Tooltip("재생할 AudioClip 의 Addressable 주소. 비우면 사운드 없음")]
    public string soundAddress;

    [Tooltip("생성할 이펙트 프리팹의 Addressable 주소. 비우면 이펙트 없음")]
    public string effectAddress;

    [Tooltip("이펙트를 붙일 본 이름. 비우면 캐릭터 루트 기준")]
    public string attachBone;

    [Tooltip("attachBone(비었으면 루트) 의 로컬 기준 위치 오프셋")]
    public Vector3 localOffset;

    [Tooltip("켜면 본의 자식으로 붙어 따라다닌다 (손에 붙는 트레일 등).\n" +
             "끄면 생성 시점의 위치에 남는다 (바닥에 터지는 폭발 등)")]
    public bool followBone;

    [Tooltip("이펙트 회수까지의 시간 (초). 0 이하면 회수하지 않는다 — 스스로 정리되는 프리팹 전용")]
    public float lifetime;

    [Tooltip("에디터 가독성용 메모. 런타임 식별에는 쓰이지 않는다")]
    public string note;
}

[Serializable]
public struct AnimationTimeline
{
    [Tooltip("베이크 소스가 되는 애니메이션 클립")]
    public AnimationClip clip;

    [Tooltip("이 클립이 걸린 Animator State 의 Speed 값과 반드시 동일하게 맞출 것 (Bake 대상 아님, 수동 입력).\n" +
             "AnimationClip 은 자기가 어느 State 에서 몇 배속으로 쓰이는지 모르기 때문에 Bake 로 자동 채울 수 없다.")]
    public float playbackSpeed;

    [Header("Baked (Bake 버튼으로 자동 채움 · 직접 수정 금지)")]
    [Tooltip("clip.length (초). Animator State Speed 배수는 반영되지 않은 원본 길이다.")]
    public float clipLength;

    [Tooltip("start 오름차순으로 정렬되어 굽힌다")]
    public AnimationMarker[] markers;

    [Header("Cues (직접 채움 · Bake 는 검증/정렬만 한다)")]
    [Tooltip("마커 시점에 터질 연출. 발화는 시각 오름차순 커서로 진행하므로 Bake 가 이 순서를 보장한다")]
    public AnimationCue[] cues;

    /// <summary>
    /// 0 나누기를 피한 재생 배속. playbackSpeed 가 0 이하면(필드 추가 전에 구워둔 기존 데이터라 값이 비어있는 경우 포함) 1배속으로 취급한다.
    /// 클립 시간(marker.start 등)을 실시간(Action.Elapsed)으로 옮길 때 이 값으로 나눈다.
    /// </summary>
    public float SpeedOrOne => playbackSpeed > 0f ? playbackSpeed : 1f;

    /// <summary>
    /// playbackSpeed 를 반영한 실제 재생 시간(초). CanExitState 등 실시간(Action.Elapsed) 기준 판정에는 clipLength 대신 이 값을 써야 한다.
    /// </summary>
    public float EffectiveClipLength => clipLength / SpeedOrOne;

    // ─── Queries ────────

    /// <summary>
    /// 해당 태그의 마커가 존재하는가.
    /// "없음" 을 -1 같은 센티넬 없이 표현한다.
    /// </summary>
    public bool Has(EAnimMarker tag)
    {
        if (markers == null)
            return false;

        for (int i = 0; i < markers.Length; ++i)
        {
            if (markers[i].tag == tag)
                return true;
        }

        return false;
    }

    /// <summary>
    /// elapsed 가 해당 태그의 구간 안에 있는가.
    /// </summary>
    public bool IsActive(EAnimMarker tag, float elapsed)
    {
        if (markers == null)
            return false;

        for (int i = 0; i < markers.Length; ++i)
        {
            if (markers[i].tag != tag)
                continue;

            if (markers[i].start <= elapsed && elapsed < markers[i].end)
                return true;
        }

        return false;
    }

    /// <summary>
    /// elapsed 가 해당 태그의 시작 시각을 지났는가.
    /// 같은 태그가 여러 개면 "가장 이른 시작 시각" 기준이다. (markers 는 start 오름차순으로 굽힌다)
    /// </summary>
    public bool HasPassed(EAnimMarker tag, float elapsed)
    {
        if (markers == null)
            return false;

        for (int i = 0; i < markers.Length; ++i)
        {
            if (markers[i].tag == tag && markers[i].start <= elapsed)
                return true;
        }

        return false;
    }

    /// <summary>가장 먼저 나오는 해당 태그의 마커.</summary>
    public bool TryGet(EAnimMarker tag, out AnimationMarker marker)
    {
        if (markers != null)
        {
            for (int i = 0; i < markers.Length; ++i)
            {
                if (markers[i].tag == tag)
                {
                    marker = markers[i];
                    return true;
                }
            }
        }

        marker = default;
        return false;
    }

    // ─── Cues ────────

    /// <summary>
    /// 큐의 앵커(tag + occurrence + phase)를 클립 시간으로 푼다.
    /// occurrence 는 같은 태그 안에서의 등장 순서다 — markers 가 start 오름차순으로 굽히므로 시각 순서와 같다.
    ///
    /// 반환값은 <b>클립 시간</b>이다. 실시간(Action.Elapsed) 과 비교하려면 SpeedOrOne 으로 나눠야 한다.
    /// 앵커가 가리키는 마커가 없으면 false — Bake 가 걸러내야 하는 상황이다.
    /// </summary>
    public bool TryResolveCueTime(in AnimationCue cue, out float clipTime)
    {
        if (markers != null)
        {
            int seen = 0;

            for (int i = 0; i < markers.Length; ++i)
            {
                if (markers[i].tag != cue.tag)
                    continue;

                if (seen == cue.occurrence)
                {
                    clipTime = cue.phase == EAnimCuePhase.End ? markers[i].end : markers[i].start;
                    return true;
                }

                ++seen;
            }
        }

        clipTime = 0f;
        return false;
    }
}
