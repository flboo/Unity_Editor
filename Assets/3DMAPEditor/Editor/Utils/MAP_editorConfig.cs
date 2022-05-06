using UnityEditor;
using UnityEngine;

public class MAP_editorConfig : EditorWindow
{
    private static float previousGlobleScale;
    private Vector2 _scrollPosition;

    private void OnEnable()
    {
        previousGlobleScale = MAP_Editor.editorPreferences.gridScaleFactor;
        checkForValidArrays();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("editor settings", EditorStyles.boldLabel);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("cursor color setting", EditorStyles.boldLabel);

        MAP_Editor.editorPreferences.brushCursorColor =
            EditorGUILayout.ColorField("brush cursor color", MAP_Editor.editorPreferences.brushCursorColor);
        MAP_Editor.editorPreferences.pickCursorColor =
            EditorGUILayout.ColorField("brush picker color", MAP_Editor.editorPreferences.pickCursorColor);
        MAP_Editor.editorPreferences.eraseCursorColor =
            EditorGUILayout.ColorField("erase picker color", MAP_Editor.editorPreferences.eraseCursorColor);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField(Define.ICON_SIZE, EditorStyles.boldLabel);
        MAP_Editor.editorPreferences.iconWidth =
            (int)EditorGUILayout.Slider(Define.ICON_SIZE, MAP_Editor.editorPreferences.iconWidth, 16f, 64f);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("invert mouse wheel scrolling", EditorStyles.boldLabel);
        MAP_Editor.editorPreferences.invertMouseWheel =
            EditorGUILayout.Toggle("Invert Mouse Wheel", MAP_Editor.editorPreferences.invertMouseWheel);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("center grid", EditorStyles.boldLabel);
        MAP_Editor.editorPreferences.centreGrid =
            EditorGUILayout.Toggle("center grid", MAP_Editor.editorPreferences.centreGrid);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("Globle Scale Setting", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Warning. Changing the grid scale will have a knock on effect to all your maps. The default scale is 1. If you are seeing issues with your maps, please reset the scale.",
            MessageType.Warning);
        MAP_Editor.editorPreferences.gridScaleFactor = EditorGUILayout.Slider("grid size factor",
            MAP_Editor.editorPreferences.gridScaleFactor, 1f, 10.0f);
        MAP_Editor.editorPreferences.gridLayerHeightScaler = EditorGUILayout.FloatField("grid layer height scaler",
            MAP_Editor.editorPreferences.gridLayerHeightScaler);

        if (previousGlobleScale != MAP_Editor.editorPreferences.gridScaleFactor)
        {
            MAP_Editor.gridHeight = 0;
            MAP_Editor.editorPreferences.gridScaleFactor = MAP_Editor.editorPreferences.gridScaleFactor * 2;
            MAP_Editor.editorPreferences.gridScaleFactor =
                Mathf.Round(MAP_Editor.editorPreferences.gridScaleFactor) / 2;
            if (MAP_Editor.editorPreferences.gridScaleFactor > 10) MAP_Editor.editorPreferences.gridScaleFactor = 10;
            if (MAP_Editor.editorPreferences.gridScaleFactor < 1) MAP_Editor.editorPreferences.gridScaleFactor = 1;
            previousGlobleScale = MAP_Editor.editorPreferences.gridScaleFactor;
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("grid color setting", EditorStyles.boldLabel);
        MAP_Editor.editorPreferences.gridColorNormal =
            EditorGUILayout.ColorField("Grid color", MAP_Editor.editorPreferences.gridColorNormal);
        MAP_Editor.editorPreferences.gridColorFill =
            EditorGUILayout.ColorField("Fill color", MAP_Editor.editorPreferences.gridColorFill);
        MAP_Editor.editorPreferences.gridColorBorder =
            EditorGUILayout.ColorField("Border color", MAP_Editor.editorPreferences.gridColorBorder);
        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("grid spawn position", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        MAP_Editor.editorPreferences.initialOffset =
            EditorGUILayout.Vector3Field("", MAP_Editor.editorPreferences.initialOffset);
        MAP_Editor.editorPreferences.initialOffset.x = MAP_Editor.editorPreferences.initialOffset.x * 2;
        MAP_Editor.editorPreferences.initialOffset.x = Mathf.Round(MAP_Editor.editorPreferences.initialOffset.x) / 2;
        MAP_Editor.editorPreferences.initialOffset.y = MAP_Editor.editorPreferences.initialOffset.y * 2;
        MAP_Editor.editorPreferences.initialOffset.y = Mathf.Round(MAP_Editor.editorPreferences.initialOffset.y) / 2;
        MAP_Editor.editorPreferences.initialOffset.z = MAP_Editor.editorPreferences.initialOffset.z * 2;
        MAP_Editor.editorPreferences.initialOffset.z = Mathf.Round(MAP_Editor.editorPreferences.initialOffset.z) / 2;
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("Grid Offset", EditorStyles.boldLabel);
        MAP_Editor.gridOffset = EditorGUILayout.Slider("Grid Offset", MAP_Editor.gridOffset, -0.25f, 0.25f);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("alternative shortcut keys(ASD=>GHJ),use for unreal style editor navigation ",
            EditorStyles.boldLabel);
        MAP_Editor.editorPreferences.useAlternativeKeyShortcuts = EditorGUILayout.Toggle("AlterNative Keys",
            MAP_Editor.editorPreferences.useAlternativeKeyShortcuts);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("Hide Unity UI Object while Tool is enable (EXPERIMENTAL !)",
            EditorStyles.boldLabel);
        MAP_Editor.editorPreferences.hideUIObjects =
            EditorGUILayout.Toggle("Hide Unity UI Object", MAP_Editor.editorPreferences.hideUIObjects);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Layer Name", EditorStyles.boldLabel, GUILayout.Width(125));
        EditorGUILayout.LabelField("Freeze", EditorStyles.boldLabel, GUILayout.Width(75));
        EditorGUILayout.LabelField("Static", EditorStyles.boldLabel, GUILayout.Width(75));
        EditorGUILayout.EndHorizontal();

        for (var i = 0; i < MAP_Editor.editorPreferences.layerNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            MAP_Editor.editorPreferences.layerNames[i] =
                EditorGUILayout.TextField(MAP_Editor.editorPreferences.layerNames[i], GUILayout.Width(125));
            MAP_Editor.editorPreferences.layerFreeze[i] =
                EditorGUILayout.Toggle(MAP_Editor.editorPreferences.layerFreeze[i], GUILayout.Width(75));
            MAP_Editor.editorPreferences.layerStatic[i] =
                EditorGUILayout.Toggle(MAP_Editor.editorPreferences.layerStatic[i], GUILayout.Width(75));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(MAP_Editor.editorPreferences);
            SceneView.RepaintAll();
        }
    }

    private static void Initialize()
    {
        var editorConfig = GetWindow<MAP_editorConfig>(true, Define.EDITOR_CONFIG);
        editorConfig.titleContent.text = Define.EDITOR_CONFIG;
    }

    private void checkForValidArrays()
    {
        if (MAP_Editor.editorPreferences.layerNames.Count < 8)
        {
            MAP_Editor.editorPreferences.layerNames.Clear();
            for (var i = 1; i < 9; i++) MAP_Editor.editorPreferences.layerNames.Add(Define.LAYER + i);
        }

        if (MAP_Editor.editorPreferences.layerStatic.Count < 8)
        {
            MAP_Editor.editorPreferences.layerStatic.Clear();
            for (var i = 1; i < 9; i++) MAP_Editor.editorPreferences.layerStatic.Add(true);
        }

        if (MAP_Editor.editorPreferences.layerFreeze.Count < 8)
        {
            MAP_Editor.editorPreferences.layerFreeze.Clear();
            for (var i = 1; i < 9; i++) MAP_Editor.editorPreferences.layerFreeze.Add(true);
        }
    }
}