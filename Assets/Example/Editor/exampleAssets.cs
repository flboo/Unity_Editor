using UnityEditor;
using UnityEngine;

public class exampleAssets : ScriptableObject
{
    [SerializeField] private string str;

    [SerializeField] [Range(0, 5)] private int sum;

    //创建scriptableasset 方法1
    [MenuItem("Tools/Example/CreateExample")]
    private static void CreateExample()
    {
        var exampleAsse = CreateInstance<exampleAssets>();
        AssetDatabase.CreateAsset(exampleAsse, "Assets/Example/ScriptAbleAsset/ExampleAsset.asset");
        AssetDatabase.Refresh();
    }

    //资产加载
    [MenuItem("Tools/Example/LoadExample")]
    private static void LoadExampleAsset()
    {
        var exampleAsse =
            AssetDatabase.LoadAssetAtPath<exampleAssets>("Assets/Example/ScriptAbleAsset/ExampleAsset.asset");
    }
}