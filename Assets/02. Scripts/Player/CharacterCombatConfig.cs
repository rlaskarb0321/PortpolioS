using System.Collections.Generic;
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

    [Header("Animation Steps")]
    [Tooltip("Key 별로 스텝 배열을 묶는다. 새 상태의 애니메이션이 필요하면 여기 원소를 추가")]
    [SerializeField] private AnimationStepGroup[] stepGroups;

    [Header("Dodge Movement")]
    [SerializeField] private AnimationCurve dodgeSpeedCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField] private float dodgeSpeedMultiplier = 8f;

    private Dictionary<EAnimStepKey, AnimationTimeline[]> stepMap;

    public float MaxMoveSpeed => maxMoveSpeed;
    public AnimationCurve DodgeSpeedCurve => dodgeSpeedCurve;
    public float DodgeSpeedMultiplier => dodgeSpeedMultiplier;

    public int GetStepCount(EAnimStepKey key)
    {
        return BuildStepMap().TryGetValue(key, out AnimationTimeline[] steps) == true ? steps.Length : 0;
    }

    public AnimationTimeline GetStep(EAnimStepKey key, int index = 0)
    {
        return BuildStepMap()[key][index];
    }

    private Dictionary<EAnimStepKey, AnimationTimeline[]> BuildStepMap()
    {
        if (stepMap == null)
        {
            stepMap = new Dictionary<EAnimStepKey, AnimationTimeline[]>();

            if (stepGroups != null)
            {
                foreach (var group in stepGroups)
                    stepMap[group.key] = group.steps;
            }
        }

        return stepMap;
    }
}

public enum EAnimStepKey
{
    NormalCombo,
    Dodge,
    ExpertNormal,
    ExpertEnhanced,
}

[System.Serializable]
public struct AnimationStepGroup
{
    public EAnimStepKey key;

    [Tooltip("배열 순서가 콤보 순서다 (콤보가 아니면 0번 원소만 쓴다). 원소마다 clip 만 지정하고 Bake 로 마커를 굽는다")]
    public AnimationTimeline[] steps;
}
