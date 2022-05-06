using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class GameSettingEditor : Editor2DTab
{
    private const string editorPrefsName = "game_setting_name_path";
    private int newLevel;


    private Vector2 scrollPos;
    private int selectedTabIndex;
    private ReorderableList tileScoreOverridesList;

    public GameSettingEditor(MAP2DEditor01 editor) : base(editor)
    {
        if (EditorPrefs.HasKey(editorPrefsName))
        {
            var path = EditorPrefs.GetString(editorPrefsName);
            m_MAP2DEditor01.GameConfig = LoadJsonFile<GameConfig>(path);
            if (m_MAP2DEditor01.GameConfig != null)
            {
            }

            newLevel = PlayerPrefs.GetInt("next_level");
        }
    }


    public override void Draw()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        var oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 100;

        GUILayout.Space(15);

        //绘制主菜单
        DrawMenu();

        GUILayout.Space(15);

        //绘制二级选择界面
        var preselectedTabIndex = selectedTabIndex;
        selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, new[] { "x", "y", "z" }, GUILayout.Width(500),
            GUILayout.Height(30));
        if (preselectedTabIndex != selectedTabIndex) GUI.FocusControl(null);

        if (selectedTabIndex == 0) DrawGameTab();

        EditorGUILayout.EndScrollView();
    }

    private void DrawGameTab()
    {
        if (m_MAP2DEditor01.GameConfig != null)
        {
            GUILayout.Space(15);
            DrawScoreSettings();
            GUILayout.Space(15);
        }
    }

    private void DrawMenu()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("新", GUILayout.Width(200), GUILayout.Height(50)))
        {
            m_MAP2DEditor01.GameConfig = new GameConfig();
            SetTileShow();
        }

        if (GUILayout.Button("加载", GUILayout.Width(200), GUILayout.Height(50)))
        {
            var path = EditorUtility.OpenFilePanel("game config", Application.dataPath + "2DEditoro1/Resources",
                "json");
            if (!string.IsNullOrEmpty(path))
            {
                SetTileShow();
                EditorPrefs.SetString(editorPrefsName, path);
            }
        }

        if (GUILayout.Button("以json形式储存起来", GUILayout.Width(200), GUILayout.Height(50)))
            SaveGameConfiguration(Application.dataPath + "/2DEditoro1/Resources");

        GUILayout.EndHorizontal();
    }


    private void SetTileShow()
    {
        // tileScoreOverridesList = SetupReorderableList("score overridr", m_MAP2DEditor01.GameConfig.tileScoreOverrides);
    }

    public void SaveGameConfiguration(string path)
    {
        var fullPath = path + "/game_config.json";
        SaveJsonFile(fullPath, m_MAP2DEditor01.GameConfig);
        EditorPrefs.SetString(editorPrefsName, fullPath);
        AssetDatabase.Refresh();
    }


    private void DrawScoreSettings()
    {
        var gameConfig = m_MAP2DEditor01.GameConfig;

        EditorGUILayout.LabelField("Score", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal(GUILayout.Width(300));
        const string helpText = "abbababbsb21354324a";
        EditorGUILayout.HelpBox(helpText, MessageType.Info);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Default Score", "yoahdfhddsi;ajkhvj;dkl"),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        gameConfig.maxLives = EditorGUILayout.IntField(gameConfig.maxLives, GUILayout.Width(70));
        GUILayout.EndHorizontal();
    }
}