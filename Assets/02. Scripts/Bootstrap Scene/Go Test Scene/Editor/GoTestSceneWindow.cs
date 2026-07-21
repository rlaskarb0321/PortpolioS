using UnityEditor;
using UnityEngine;

// Develop(테스트) 씬 진입 여부를 켜고 끄는 커스텀 에디터 창.
// 켜지면 창 전체가 노랗게 변해 테스트 모드임을 눈에 띄게 알린다.
public class GoTestSceneWindow : EditorWindow
{
    private static readonly Color WarnColor = new Color(1f, 0.82f, 0.05f, 1f);

    [MenuItem("Tools/Go Test Scene")]
    private static void Open()
    {
        var window = GetWindow<GoTestSceneWindow>();
        window.titleContent = new GUIContent("Go Test Scene");
        window.minSize = new Vector2(260f, 150f);
        window.Show();
    }

    private void OnGUI()
    {
        bool enabled = GoTestSceneSettings.Enabled;

        // 켜져 있으면 창 배경 전체를 노랗게 칠한다.
        if (enabled)
            EditorGUI.DrawRect(new Rect(0f, 0f, position.width, position.height), WarnColor);

        // 노란 배경 위에서는 검은 글씨가 잘 보이도록 대비를 준다.
        Color prevContent = GUI.contentColor;
        if (enabled)
            GUI.contentColor = Color.black;

        GUILayout.Space(14f);

        var headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 15,
            alignment = TextAnchor.MiddleCenter
        };
        GUILayout.Label(enabled ? "⚠  TEST SCENE MODE  ON" : "Test Scene Mode : OFF", headerStyle);

        GUILayout.Space(14f);

        // 토글을 바꾸면 즉시 EditorPrefs에 반영한다.
        bool next = GUILayout.Toggle(enabled, "  Go Test Scene?", GUILayout.Height(26f));
        if (next != enabled)
        {
            GoTestSceneSettings.Enabled = next;
            Repaint();
        }

        GUILayout.Space(10f);

        var helpStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };
        GUILayout.Label("켜면 플레이 시작 시 로그인·앱 초기화를 건너뛰고\nDevelop 씬으로 바로 진입한다.", helpStyle);

        GUI.contentColor = prevContent;
    }
}
