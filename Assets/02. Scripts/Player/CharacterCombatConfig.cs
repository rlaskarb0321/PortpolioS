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

    public int   NormalAttackMaxCombo => normalAttackMaxCombo;
    public float MaxMoveSpeed         => maxMoveSpeed;
}
