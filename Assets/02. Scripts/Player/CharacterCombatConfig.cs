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

    [Tooltip("배열 순서가 콤보 순서다. 원소마다 clip 만 지정하고 Bake 로 마커를 굽는다")]
    [SerializeField] private AnimationTimeline[] normalComboSteps;

    public float MaxMoveSpeed => maxMoveSpeed;

    public int NormalComboStepCount => normalComboSteps != null ? normalComboSteps.Length : 0;

    public AnimationTimeline GetComboStep(int index)
    {
        return normalComboSteps[index];
    }
}
