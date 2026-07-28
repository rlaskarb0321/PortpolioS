using UnityEngine;

[CreateAssetMenu
(
    fileName = "Character Combat Config",
    menuName = "Scriptable Objects/Config/Character Combat Config",
    order = int.MaxValue
)]
public class CharacterCombatConfig : ScriptableObject
{
    [SerializeField] private int   normalAttackMaxCombo;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private NormalComboStep[] normalComboSteps;

    public int   NormalAttackMaxCombo => normalAttackMaxCombo;
    public float MaxMoveSpeed         => maxMoveSpeed;

    public int NormalComboStepCount => normalComboSteps != null ? normalComboSteps.Length : 0;

    public NormalComboStep GetComboStep(int index)
    {
        return normalComboSteps[index];
    }
}

/// <summary>
/// 노멀 콤보 한 타의 타이밍 데이터.
/// clip 필드만 지정하고, 나머지 수치는 인스펙터의 "Bake" 버튼으로 클립의
/// 애니메이션 이벤트에서 자동으로 채워진다. (직접 수정하지 말 것)
/// 런타임 시뮬레이션은 이 구워진 수치를 TickTimer 로만 사용한다.
/// </summary>
[System.Serializable]
public struct NormalComboStep
{
    [Tooltip("베이크 소스가 되는 애니메이션 클립")]
    public AnimationClip clip;

    [Header("Baked (Bake 버튼으로 자동 채움 · 직접 수정 금지)")]
    [Tooltip("clip.length (초)")]
    public float clipLength;

    [Tooltip("SetCanInput 이벤트 시간 (초). -1 = 다음 콤보로 못 이음")]
    public float inputWindowStart;

    [Tooltip("SetCannotInput 이벤트 시간 (초). -1 = 없음")]
    public float inputWindowEnd;

    [Tooltip("HandleComboInput 이벤트 시간 (초) · 콤보 분기 판정 시점")]
    public float comboDecisionTime;
}
