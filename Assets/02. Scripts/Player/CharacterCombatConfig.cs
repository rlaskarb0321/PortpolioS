using UnityEngine;

[CreateAssetMenu
(
    fileName = "Character Combat Config",
    menuName = "Scriptable Objects/Config/Character Combat Config",
    order = int.MaxValue
)]
public class CharacterCombatConfig : ScriptableObject
{
    public int normalAttackMaxCombo;
    public float maxMoveSpeed;
}
