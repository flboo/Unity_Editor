using UnityEditor;
using UnityEngine;

internal class YuME_customBrushNameDialog : EditorWindow
{
    private string customBrushName;

    private void OnGUI()
    {
        customBrushName = EditorGUILayout.TextField("Custom Brush Name", customBrushName);

        if (GUILayout.Button("Save Custom Brush"))
        {
            OnClickSavePrefab();
            GUIUtility.ExitGUI();
        }
    }

    private void OnClickSavePrefab()
    {
        customBrushName = customBrushName.Trim();

        if (string.IsNullOrEmpty(customBrushName))
        {
            EditorUtility.DisplayDialog("Unable to save custom Brush", "Please specify a valid custom brush name.",
                "Close");
            return;
        }

        MAP_customBrushFunctions.createCustomBrush(customBrushName);

        Close();
    }
}