using UnityEngine;

public abstract class SceneLoadStrategyBase : MonoBehaviour
{
    [Header("Scene Load Strategy")]
    [SerializeField] private ESceneLoadStrategy loadStrategy;
    
    public ESceneLoadStrategy LoadStrategy { get => loadStrategy; }
}

public enum ESceneLoadStrategy
{
    LocalSceneLoad,
    NetworkSceneLoad,
    Count
}