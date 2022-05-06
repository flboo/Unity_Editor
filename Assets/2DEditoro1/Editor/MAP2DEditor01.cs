using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MAP2DEditor01 : EditorWindow
{
    public GameConfig GameConfig;
    private readonly List<Editor2DTab> tabs = new();
    private int preSelectedTabIndex = -1;
    private int selectedTabIndex = -1;

    private void OnEnable()
    {
        tabs.Add(new GameSettingEditor(this));
        tabs.Add(new LevelEditor01(this));
        selectedTabIndex = 0;
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, new[] { "game setting ", "Level editor" },
            GUILayout.MinHeight(50));
        if (selectedTabIndex >= 0 && selectedTabIndex < tabs.Count)
        {
            var selectedEditor = tabs[selectedTabIndex];
            if (selectedTabIndex != preSelectedTabIndex)
            {
                selectedEditor.OnTabSelected();
                GUI.FocusControl(null);
            }

            selectedEditor.Draw();
            preSelectedTabIndex = selectedTabIndex;
        }
    }


    [MenuItem("Tools/2DEditoro1/MAP2DEditor01")]
    private static void ShowWindow()
    {
        var window = GetWindow<MAP2DEditor01>();
        window.titleContent = new GUIContent("MAP2D01");
        window.Show();
    }
}