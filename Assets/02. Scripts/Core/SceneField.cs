using System;
using UnityEngine;

[Serializable]
public class SceneField : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset sceneAsset;
#endif

    [SerializeField] private string sceneName;

    public string SceneName => sceneName;

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
            sceneName = sceneAsset.name;
#endif
    }

    public void OnAfterDeserialize() { }
}
