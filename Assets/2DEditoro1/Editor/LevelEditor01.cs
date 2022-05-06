using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelEditor01 : Editor2DTab
{
    private Level currentLevel;
    private Vector2 scrollPos;

    private int selectIndex = 0;
    private readonly Dictionary<string, Texture> tileTextures = new();

    public LevelEditor01(MAP2DEditor01 editor) : base(editor)
    {
        tileTextures.Clear();
        var dicImgPath = new DirectoryInfo(Application.dataPath + "/2DEditoro1/Resources/Game");
        var fileInfo = dicImgPath.GetFiles("*.png", SearchOption.TopDirectoryOnly);
        foreach (var file in fileInfo)
        {
            var filename = Path.GetFileNameWithoutExtension(file.Name);
            tileTextures[filename] = Resources.Load("Game/" + filename) as Texture;
        }
    }

    public override void Draw()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        var oldLableWith = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 90;

        GUILayout.Space(15);

        DrawMenu();


        EditorGUILayout.EndScrollView();
    }

    private void DrawMenu()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("new", GUILayout.Width(120), GUILayout.Height(50))) currentLevel = new Level();

        if (GUILayout.Button("open", GUILayout.Width(120), GUILayout.Height(50)))
        {
        }

        if (GUILayout.Button("save", GUILayout.Width(120), GUILayout.Height(50)))
        {
        }


        EditorGUILayout.EndHorizontal();
    }


    private void InitializeNewLevel()
    {
        foreach (var type in Enum.GetValues(typeof(ColorBlockType)))
            currentLevel.availableColors.Add((ColorBlockType)type);

        foreach (var type in Enum.GetValues(typeof(BoosterType)))
            currentLevel.availableBoosters.Add((BoosterType)type, true);
    }
}