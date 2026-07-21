#if UNITY_EDITOR
using UnityEditor;

// Develop(테스트) 씬으로 바로 진입할지 여부를 담는 에디터 전용 토글.
// 값은 EditorPrefs에 저장되어 창을 닫거나 플레이를 반복해도 유지되고, 빌드에는 포함되지 않는다.
public static class GoTestSceneSettings
{
    private const string EnabledKey = "Portfolio.GoTestScene.Enabled";

    public static bool Enabled
    {
        get => EditorPrefs.GetBool(EnabledKey, false);
        set => EditorPrefs.SetBool(EnabledKey, value);
    }
}
#endif
