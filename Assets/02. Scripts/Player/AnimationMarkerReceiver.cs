using UnityEngine;

/// <summary>
/// 클립에 찍힌 타이밍 마커 이벤트의 수신처. Animator 와 같은 GameObject 에 붙어야 한다.
///
/// 메서드 본문이 비어 있는 것은 의도된 것이다 — 이 이벤트는 Bake 의 오써링 소스일 뿐이고
/// 런타임 판정에는 관여하지 않는다. (Render 도메인이라 클라이언트마다 타이밍이 달라 비결정적)
///
/// 그래도 메서드가 존재해야 하는 이유는 두 가지.
/// · Unity 애니메이션 이벤트의 함수 드롭다운에 노출된다
/// · "has no receiver!" 경고가 뜨지 않는다
/// </summary>
public class AnimationMarkerReceiver : MonoBehaviour
{
    /// <summary>Bake 가 읽는 이벤트 함수 이름. 다른 이름의 이벤트(연출용 등)는 마커로 취급하지 않는다.</summary>
    public const string EventFunctionName = nameof(EventTiming);

    /// <summary>구간 마커의 시작/끝 접미사 규약.</summary>
    public const string OpenSuffix  = ".Open";
    public const string CloseSuffix = ".Close";

    /// <param name="tag">EAnimMarker 이름. 구간 마커는 ".Open" / ".Close" 를 붙인다.</param>
    public void EventTiming(string tag) { }
}
