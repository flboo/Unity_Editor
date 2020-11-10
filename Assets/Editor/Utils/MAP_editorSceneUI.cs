using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MAP_editorSceneUI : EditorWindow
{

    public static void drawToolUI(SceneView scenesView)
    {
        Handles.BeginGUI();
        GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
        if (MAP_Editor.randomRotationMode)
        {
            var centeredStyle = GUI.skin.GetStyle("Lable");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUI.Label(new Rect(Screen.width / 2 - 250, 10, 500, 50), "Random rotation paint mode is on. Press R to toggle off.", centeredStyle);
        }

        for (int i = 0; i < MAP_Editor.editorData.primaryIconData.Count; i++)
        {
            drawToolIcons(i);
        }
        for (int i = 0; i < MAP_Editor.editorData.secondaryIconData.Count; i++)
        {
            drawSecondaryToolIcons(i, scenesView.position);
        }
        if (MAP_Editor.selectTiles.Count > 0 || Selection.gameObjects.Length > 0)
        {
            for (int i = 0; i < MAP_Editor.editorData.selectIconData.Count; i++)
            {
                drawSelectToolIcons(i, scenesView.position);
            }
        }

        drawLayerToolIcons(scenesView.position);
        Handles.EndGUI();
    }


    private static void drawToolIcons(int index)
    {
        bool isActive = false;
        if (MAP_Editor.selectTool == (eToolIcons)index)
            isActive = true;
        GUIContent buttonContent;
        buttonContent = new GUIContent(MAP_Editor.editorData.primaryIconData[index], MAP_Editor.editorData.primaryIconTooltip[index]);
        bool overrideSelection = false;
        if (MAP_Editor.eraseToolOverride && index == (int)eToolIcons.eraseTool)
        {
            overrideSelection = true;
        }
        if (MAP_Editor.pickToolOverride && index == (int)eToolIcons.pickTool)
        {
            overrideSelection = true;
        }
        if (overrideSelection)
        {
            GUI.Toggle(new Rect(10, index * MAP_Editor.editorPreferences.iconWidth + 10, MAP_Editor.editorPreferences.iconWidth, MAP_Editor.editorPreferences.iconWidth), overrideSelection, buttonContent, GUI.skin.button);
        }
        else
        {
            bool isToggleDown = GUI.Toggle(new Rect(10, index * MAP_Editor.editorPreferences.iconWidth + 10, MAP_Editor.editorPreferences.iconWidth, MAP_Editor.editorPreferences.iconWidth), isActive, buttonContent, GUI.skin.button);

            if (isToggleDown == true && isActive == false)
            {
                MAP_Editor.selectTool = (eToolIcons)index;
            }
        }
    }
    private static void drawSecondaryToolIcons(int index, Rect position)
    {
        bool isActive = false;
        int toolOffset = MAP_Editor.editorData.primaryIconData.Count;
        if (MAP_Editor.selectTool == (eToolIcons)index + toolOffset)
        {
            isActive = true;
        }
        GUIContent buttonContent;
        buttonContent = new GUIContent(MAP_Editor.editorData.secondaryIconData[index], MAP_Editor.editorData.secondaryIconTooltip[index]);

        bool isToggleDown = GUI.Toggle(new Rect((index * -1) * MAP_Editor.editorPreferences.iconWidth + position.width - MAP_Editor.editorPreferences.iconWidth - 10,
                            position.height - 25 - MAP_Editor.editorPreferences.iconWidth,
                            MAP_Editor.editorPreferences.iconWidth,
                            MAP_Editor.editorPreferences.iconWidth), isActive, buttonContent, GUI.skin.button);
        if (isToggleDown && !isActive)
        {
            MAP_Editor.selectTool = (eToolIcons)index + toolOffset;
        }

        if (index + toolOffset == (int)eToolIcons.isolateTool)
        {
            isActive = MAP_Editor.isolataTiles;

            GUI.Toggle(new Rect((index * -1) * MAP_Editor.editorPreferences.iconWidth + position.width - MAP_Editor.editorPreferences.iconWidth - 10,
                position.height - 25 - MAP_Editor.editorPreferences.iconWidth,
                MAP_Editor.editorPreferences.iconWidth,
                MAP_Editor.editorPreferences.iconWidth), isActive, buttonContent, GUI.skin.button);
        }

        if (index + toolOffset == (int)eToolIcons.showGizmos)
        {
            isActive = MAP_Editor.showGizmos;

            GUI.Toggle(new Rect((index * -1) * MAP_Editor.editorPreferences.iconWidth + position.width - MAP_Editor.editorPreferences.iconWidth - 10,
                position.height - 25 - MAP_Editor.editorPreferences.iconWidth,
                MAP_Editor.editorPreferences.iconWidth,
                MAP_Editor.editorPreferences.iconWidth), isActive, buttonContent, GUI.skin.button);
        }
    }

    private static void drawSelectToolIcons(int index, Rect position)
    {
        bool isActive = false;
        int toolOffset = MAP_Editor.editorData.primaryIconData.Count + MAP_Editor.editorData.secondaryIconData.Count;
        float screenOffset = MAP_Editor.editorPreferences.iconWidth * MAP_Editor.editorData.primaryIconData.Count;

        if (MAP_Editor.selectTool == (eToolIcons)index + toolOffset)
        {
            isActive = true;
        }

        GUIContent buttonContent;
        buttonContent = new GUIContent(MAP_Editor.editorData.selectIconData[index], MAP_Editor.editorData.selectionIconTooltip[index]);

        bool isToggleDown = GUI.Toggle(new Rect(10, index * MAP_Editor.editorPreferences.iconWidth + screenOffset + 16f, MAP_Editor.editorPreferences.iconWidth, MAP_Editor.editorPreferences.iconWidth), isActive, buttonContent, GUI.skin.button);
        //bool isToggleDown = GUI.Toggle(new Rect(10, index * MAP_Editor.editorPreferences.iconWidth + 192, MAP_Editor.editorPreferences.iconWidth , MAP_Editor.editorPreferences.iconWidth ), isActive, buttonContent, GUI.skin.button);

        if (isToggleDown == true && isActive == false)
        {
            MAP_Editor.selectTool = (eToolIcons)index + toolOffset;
        }

    }


    private static void drawLayerToolIcons(Rect position)
    {
        bool isActive = false;
        isActive = MAP_Editor.isolateLayer;
        GUIContent buttonContent;
        buttonContent = new GUIContent(MAP_Editor.editorData.layerIconData[0], MAP_Editor.editorData.layerIconTooltip[0]);

        bool isToggleDown = GUI.Toggle(new Rect(10, position.height - 25 - MAP_Editor.editorPreferences.iconWidth, MAP_Editor.editorPreferences.iconWidth, MAP_Editor.editorPreferences.iconWidth), isActive, buttonContent, GUI.skin.button);

        if (isToggleDown == true && isActive == false)
        {
            MAP_Editor.selectTool = eToolIcons.isolateLayerTool;
        }
        else if (isToggleDown == false && isActive == true)
        {
            MAP_Editor.selectTool = eToolIcons.isolateLayerTool;
        }
        GUILayout.BeginArea(new Rect(10 + MAP_Editor.editorPreferences.iconWidth + 2, position.height - 25 - (MAP_Editor.editorPreferences.iconWidth / 2), MAP_Editor.editorPreferences.iconWidth * 2, MAP_Editor.editorPreferences.iconWidth / 2), EditorStyles.textArea);
        {
            GUILayout.Label(MAP_Editor.editorPreferences.layerNames[MAP_Editor.currentLayer - 1], EditorStyles.label, GUILayout.Width(MAP_Editor.editorPreferences.iconWidth * 2));
        }
        GUILayout.EndArea();
        isActive = false;
        if (MAP_Editor.selectTool == eToolIcons.layerUp)
        {
            isActive = true;
        }
        buttonContent = new GUIContent(MAP_Editor.editorData.layerIconData[1], MAP_Editor.editorData.layerIconTooltip[1]);

        isToggleDown = GUI.Toggle(new Rect(10 + MAP_Editor.editorPreferences.iconWidth + 2, position.height - 25 - MAP_Editor.editorPreferences.iconWidth, MAP_Editor.editorPreferences.iconWidth, MAP_Editor.editorPreferences.iconWidth / 2), isActive, buttonContent, GUI.skin.button);

        if (isToggleDown == true && isActive == false)
        {
            MAP_Editor.selectTool = eToolIcons.layerUp;
        }

        isActive = false;
        if (MAP_Editor.selectTool == eToolIcons.layerDown)
        {
            isActive = true;
        }
        buttonContent = new GUIContent(MAP_Editor.editorData.layerIconData[2], MAP_Editor.editorData.layerIconTooltip[2]);
        isToggleDown = GUI.Toggle(new Rect(10 + (MAP_Editor.editorPreferences.iconWidth * 2) + 2, position.height - 25 - MAP_Editor.editorPreferences.iconWidth, MAP_Editor.editorPreferences.iconWidth, MAP_Editor.editorPreferences.iconWidth / 2), isActive, buttonContent, GUI.skin.button);
        if (isToggleDown == true && isActive == false)
        {
            MAP_Editor.selectTool = eToolIcons.layerDown;
        }
    }


}

