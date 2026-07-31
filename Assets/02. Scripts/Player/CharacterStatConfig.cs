using UnityEngine;

[CreateAssetMenu
(
    fileName = "Character Stat Config",
    menuName = "Scriptable Objects/Config/Character Stat Config",
    order = int.MaxValue
)]
public class CharacterStatConfig : ScriptableObject
{
    [Header("Stat")]
    [SerializeField] private float maxHp;
    [SerializeField] private float maxEnergy;
    [SerializeField] private float moveSpeed;

    [Header("Resource")]
    [SerializeField] private float enhancedSkillEnergyCost;
    [SerializeField] private float dodgeCoolTime;
    [SerializeField] private float energyGainOnHit;

    public float MaxHp => maxHp;
    public float MaxEnergy => maxEnergy;
    public float MoveSpeed => moveSpeed;
    public float EnhancedSkillEnergyCost => enhancedSkillEnergyCost;
    public float DodgeCoolTime => dodgeCoolTime;
    public float EnergyGainOnHit => energyGainOnHit;
}
