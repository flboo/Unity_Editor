using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
public class ScriptObjectCreat : EditorWindow
{
    private static string savePath = "Assets/ScriptableObject";

    [MenuItem("windows/MAP/ScriptObject/editor_data")]
    public static void CreateTestAsset()
    {
        //创建数据
        MAP_EditorData editorData = ScriptableObject.CreateInstance<MAP_EditorData>();
        //赋值
        editorData.name = "MAP_EditorData";

        //检查保存路径
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        //删除原有文件，生成新文件
        string fullPath = savePath + "/" + "editor_Data.asset";
        UnityEditor.AssetDatabase.DeleteAsset(fullPath);
        UnityEditor.AssetDatabase.CreateAsset(editorData, fullPath);
        UnityEditor.AssetDatabase.Refresh();
    }
    [MenuItem("windows/MAP/ScriptObject/prefrence_data")]
    public static void CreatePrefrencesAsset()
    {
        //创建数据
        MAP_editorPreferences editorData = ScriptableObject.CreateInstance<MAP_editorPreferences>();
        //赋值
        editorData.name = "map_editorPreferences";

        //检查保存路径
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        //删除原有文件，生成新文件
        string fullPath = savePath + "/" + "preference_data.asset";
        UnityEditor.AssetDatabase.DeleteAsset(fullPath);
        UnityEditor.AssetDatabase.CreateAsset(editorData, fullPath);
        UnityEditor.AssetDatabase.Refresh();
    }

    [MenuItem("windows/MAP/ScriptObject/tileset_data")]
    public static void CreateTilesetAsset()
    {
        //创建数据
        MAP_tilesetData editorData = ScriptableObject.CreateInstance<MAP_tilesetData>();
        editorData.name = "tileset_data";
        CreatAssetData(editorData);
    }








    private static void CreatAssetData(UnityEngine.Object asset)
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        //删除原有文件，生成新文件
        string fullPath = savePath + "/" + asset.name + ".asset";
        UnityEditor.AssetDatabase.DeleteAsset(fullPath);
        UnityEditor.AssetDatabase.CreateAsset(asset, fullPath);
        UnityEditor.AssetDatabase.Refresh();
    }


}
