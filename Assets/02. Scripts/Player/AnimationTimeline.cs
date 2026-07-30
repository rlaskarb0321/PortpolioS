using System;
using UnityEngine;

public enum EAnimMarker
{
    None = 0,
    ComboInput,
    ComboDecision,
    Invincible,
    Hitbox,
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

    /// <summary>
    /// playbackSpeed 를 반영한 실제 재생 시간(초). CanExitState 등 실시간(Action.Elapsed) 기준 판정에는 clipLength 대신 이 값을 써야 한다.
    /// playbackSpeed 가 0 이하면(필드 추가 전에 구워둔 기존 데이터라 값이 비어있는 경우 포함) 1배속으로 취급해 0 나누기를 피한다.
    /// </summary>
    public float EffectiveClipLength => clipLength / (playbackSpeed > 0f ? playbackSpeed : 1f);

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
}
