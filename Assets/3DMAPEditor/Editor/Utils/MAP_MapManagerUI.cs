﻿using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MAP_MapManagerUI : EditorWindow
{
    private Vector2 _scrollPosition;
    private string newMapName;

    private void OnGUI()
    {
        EditorGUILayout.Space();
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        EditorGUILayout.BeginVertical(Define.BOX);

        EditorGUILayout.LabelField("new map name", EditorStyles.boldLabel);

        newMapName = EditorGUILayout.TextField(newMapName);


        if (GUILayout.Button("add new map", GUILayout.Height(20))) Map_mapManagerFunctions.buildNewMap(newMapName);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);

        for (var i = 0; i < MAP_Editor.ref_MapManager.mapList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            MAP_Editor.ref_MapManager.mapList[i].name =
                EditorGUILayout.TextField(MAP_Editor.ref_MapManager.mapList[i].name, GUILayout.Width(200));
            if (i != 0)
            {
                if (GUILayout.Button("close map", GUILayout.Height(20), GUILayout.Width(75)))
                {
                    MAP_Editor.cloneMap(MAP_Editor.ref_MapManager.mapList[i]);
                    EditorSceneManager.MarkAllScenesDirty();
                }

                if (GUILayout.Button("delete map", GUILayout.Height(20), GUILayout.Width(75)))
                {
                    DestroyImmediate(MAP_Editor.ref_MapManager.mapList[i]);
                    MAP_Editor.ref_MapManager.mapList.RemoveAt(i);
                    MAP_Editor.currentMapIndex = 0;
                    EditorSceneManager.MarkAllScenesDirty();
                }
            }
            else
            {
                if (GUILayout.Button("clone map", GUILayout.Height(20), GUILayout.Width(75)))
                {
                    MAP_Editor.cloneMap(MAP_Editor.ref_MapManager.mapList[i]);
                    EditorSceneManager.MarkAllScenesDirty();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private static void Initialize()
    {
        var mapManagerUI = GetWindow<MAP_MapManagerUI>(true, Define.MAP_MANAGER);
        mapManagerUI.titleContent.text = Define.MAP_MANAGER;
    }
}