using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 열려 있는 씬들의 EventSystem·AudioListener 오브젝트에 <see cref="SceneSingletonGuard"/>를
/// 일괄 부착하는 에디터 편의 메뉴. Bootstrap 경유 로드 시 중복 경고 로그를 방지한다.
/// </summary>
public static class SceneSingletonGuardMenu
{
    [MenuItem("Tools/Bootstrap/Add Singleton Guard To Open Scenes")]
    private static void AddGuardToOpenScenes()
    {
        int added = 0;

        added += AddGuardTo<EventSystem>();
        added += AddGuardTo<AudioListener>();

        if (added > 0)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetSceneAt(i));
        }

        Debug.Log($"[SceneSingletonGuard] Added guard to {added} object(s) in open scenes.");
    }

    private static int AddGuardTo<T>() where T : Component
    {
        int added = 0;
        T[] targets = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (T target in targets)
        {
            if (target.GetComponent<SceneSingletonGuard>() != null)
                continue;

            Undo.AddComponent<SceneSingletonGuard>(target.gameObject);
            added++;
        }

        return added;
    }
}
