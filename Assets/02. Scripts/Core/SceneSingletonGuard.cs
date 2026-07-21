using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 씬 단독 실행/편집 편의를 위해 각 작업 씬에 남겨둔 EventSystem·MainCamera(AudioListener) 등이,
/// Bootstrap 씬 경유(additive) 로드 시 중복되어 발생하는 경고 로그를 방지한다.
///
/// "씬에 하나만 존재해야 하는" 컴포넌트가 다른 씬(DontDestroyOnLoad 포함)에 이미 존재하면
/// 이 오브젝트를 스스로 제거한다. 단독 실행 시엔 중복이 없으므로 그대로 유지된다.
///
/// 사용법: 각 작업 씬의 개발용 EventSystem 오브젝트, MainCamera 오브젝트에 이 컴포넌트를 붙인다.
/// </summary>
[DefaultExecutionOrder(-10000)]
public class SceneSingletonGuard : MonoBehaviour
{
    private void Awake()
    {
        // 이 오브젝트가 들고 있는 대상 컴포넌트가 다른 곳에 이미 있으면 중복 → 자기 제거.
        if (HasExternalDuplicate<EventSystem>() || HasExternalDuplicate<AudioListener>())
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 이 오브젝트(및 자식)에 T가 존재하고, 그 외의 다른 곳에도 T가 존재하면 true.
    /// </summary>
    private bool HasExternalDuplicate<T>() where T : Component
    {
        if (GetComponentInChildren<T>(true) == null)
            return false;

        T[] all = FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (T component in all)
        {
            // 자기 자신(또는 자식) 이 아닌 인스턴스가 하나라도 있으면 중복으로 간주.
            if (component.transform.IsChildOf(transform) == false)
                return true;
        }

        return false;
    }
}
