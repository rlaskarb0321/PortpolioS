using System;
using UnityEngine;

/// <summary>
/// 애니메이션 타임라인에 찍히는 마커의 종류.
///
/// 오써링은 클립의 애니메이션 이벤트로 한다.
/// · 점   마커 → EventTiming("ComboDecision")
/// · 구간 마커 → EventTiming("ComboInput.Open") + EventTiming("ComboInput.Close")
///
/// 문자열은 클립(에디터)에만 존재하고, Bake 가 이 enum 으로 번역·검증해서 굽는다.
/// 런타임 코드는 문자열을 보지 않고 이 enum 으로만 조회한다.
/// </summary>
public enum EAnimMarker
{
    None = 0,

    /// <summary>구간 · 다음 콤보를 예약할 수 있는 입력창</summary>
    ComboInput,

    /// <summary>점 · 콤보 분기 판정 시점</summary>
    ComboDecision,

    /// <summary>구간 · 무적</summary>
    Invincible,

    /// <summary>구간 · 히트박스 활성</summary>
    Hitbox,
}

/// <summary>마커가 한 순간인지 구간인지.</summary>
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

/// <summary>
/// 클립 하나의 타이밍 데이터.
/// clip 필드만 지정하고, 나머지는 인스펙터의 "Bake" 버튼이 클립의 애니메이션 이벤트에서 채운다.
///
/// 오써링은 애니메이션 이벤트로(디자이너 친화적), 런타임 판정은 구워진 수치로(결정론적).
/// 클립 이벤트를 수정한 뒤 Bake 를 다시 누르면 갱신된다.
/// </summary>
[Serializable]
public struct AnimationTimeline
{
    [Tooltip("베이크 소스가 되는 애니메이션 클립")]
    public AnimationClip clip;

    [Header("Baked (Bake 버튼으로 자동 채움 · 직접 수정 금지)")]
    [Tooltip("clip.length (초)")]
    public float clipLength;

    [Tooltip("start 오름차순으로 정렬되어 굽힌다")]
    public AnimationMarker[] markers;

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
    ///
    /// 같은 태그가 여러 번 나오면(다단 히트, 입력창이 두 번 열리는 공격 등) 구간들의 합집합으로 판정한다.
    /// 그래서 태그가 일치해도 조기 리턴하지 않고 끝까지 스캔한다 — 여기서 조기 리턴하면
    /// 두 번째 이후의 구간을 영원히 못 본다.
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
