using UnityEditor;
using UnityEngine;

// 테스트 모드가 켜져 있으면 씬 뷰 좌상단에 경고 배지를 항상 표시한다.
// 창을 닫아도 보이므로 "테스트 모드인 걸 깜빡하고 진행하는" 사고를 막는다.
[InitializeOnLoad]
public static class GoTestSceneSceneViewOverlay
{
    private static readonly Color BadgeColor = new Color(1f, 0.82f, 0.05f, 0.92f);

    // 에디터 로드 시 자동 실행되어 씬 뷰 GUI 콜백에 등록한다.
    static GoTestSceneSceneViewOverlay()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (!GoTestSceneSettings.Enabled)
            return;

        Handles.BeginGUI();

        var rect = new Rect(8f, 8f, 210f, 26f);
        EditorGUI.DrawRect(rect, BadgeColor);

        var style = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.black }
        };
        GUI.Label(rect, "⚠  TEST SCENE MODE ON", style);

        Handles.EndGUI();
    }
}
