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

    public float MaxHp => maxHp;
    public float MaxEnergy => maxEnergy;
    public float MoveSpeed => moveSpeed;
}
