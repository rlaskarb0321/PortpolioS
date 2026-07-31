using UnityEngine;

[CreateAssetMenu
(
    fileName = "Character Combat Config",
    menuName = "Scriptable Objects/Config/Character Combat Config",
    order = int.MaxValue
)]
public class CharacterCombatConfig : ScriptableObject
{
    [SerializeField] private float maxMoveSpeed;

    [Header("Normal Combo Step")]
    [Tooltip("배열 순서가 콤보 순서다. 원소마다 clip 만 지정하고 Bake 로 마커를 굽는다")]
    [SerializeField] private AnimationTimeline[] normalComboSteps;

    [Header("Dodge Anim Step")]
    [SerializeField] private AnimationTimeline[] dodgeComboSteps;

    [Header("Normal Expert Step")]
    [SerializeField] private AnimationTimeline[] normalExpertSteps;
    
    [Header("Enhanced Expert Step")]
    [SerializeField] private AnimationTimeline[] enhancedExpertSteps;

    [Header("Dodge Movement")]
    [SerializeField] private AnimationCurve dodgeSpeedCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField] private float dodgeSpeedMultiplier = 8f;

    public float MaxMoveSpeed => maxMoveSpeed;
    public int NormalComboStepCount => normalComboSteps != null ? normalComboSteps.Length : 0;
    public AnimationCurve DodgeSpeedCurve => dodgeSpeedCurve;
    public float DodgeSpeedMultiplier => dodgeSpeedMultiplier;

    public AnimationTimeline GetComboStep(int index)
    {
        return normalComboSteps[index];
    }

    public AnimationTimeline GetDodgeComboStep(int index = 0)
    {
        return dodgeComboSteps[index];
    }
}
