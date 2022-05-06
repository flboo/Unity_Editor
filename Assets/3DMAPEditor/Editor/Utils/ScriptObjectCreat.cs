using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptObjectCreat : EditorWindow
{
    private static readonly string savePath = "Assets/ScriptableObject";

    [MenuItem("MAP/ScriptObject/editor_data")]
    public static void CreateTestAsset()
    {
        //创建数据
        var editorData = CreateInstance<MAP_EditorData>();
        //赋值
        editorData.name = "MAP_EditorData";

        //检查保存路径
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        //删除原有文件，生成新文件
        var fullPath = savePath + "/" + "editor_Data.asset";
        AssetDatabase.DeleteAsset(fullPath);
        AssetDatabase.CreateAsset(editorData, fullPath);
        AssetDatabase.Refresh();
    }

    [MenuItem("MAP/ScriptObject/prefrence_data")]
    public static void CreatePrefrencesAsset()
    {
        //创建数据
        var editorData = CreateInstance<MAP_editorPreferences>();
        //赋值
        editorData.name = "map_editorPreferences";

        //检查保存路径
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        //删除原有文件，生成新文件
        var fullPath = savePath + "/" + "preference_data.asset";
        AssetDatabase.DeleteAsset(fullPath);
        AssetDatabase.CreateAsset(editorData, fullPath);
        AssetDatabase.Refresh();
    }

    [MenuItem("MAP/ScriptObject/tileset_data")]
    public static void CreateTilesetAsset()
    {
        //说明  这个.asset不要放在创建的目录下  详情参考  TileTestObject01目录
        //创建数据
        var editorData = CreateInstance<MAP_tilesetData>();
        editorData.name = "tileset_data";
        CreatAssetData(editorData);
    }

    private static void CreatAssetData(Object asset)
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        //删除原有文件，生成新文件
        var fullPath = savePath + "/" + asset.name + ".asset";
        AssetDatabase.DeleteAsset(fullPath);
        AssetDatabase.CreateAsset(asset, fullPath);
        AssetDatabase.Refresh();
    }
}