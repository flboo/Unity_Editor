using UnityEditor;
using UnityEngine;

public class ParentScriptableObject : ScriptableObject
{
    private const string path = "Assets/Example/ScriptAbleAsset/NewParentScripableObject.asset";

    [SerializeField] private ChildScriptableObject child;


    [MenuItem("Tools/Example/Create ScriptableObject")]
    private static void CreateScriptableObject()
    {
        var parent = CreateInstance<ParentScriptableObject>();
        parent.child = CreateInstance<ChildScriptableObject>();
        parent.child.hideFlags = HideFlags.HideInHierarchy;

        AssetDatabase.CreateAsset(parent, path);
        AssetDatabase.AddObjectToAsset(parent.child, path);

        AssetDatabase.ImportAsset(path);
        AssetDatabase.Refresh();
    }

    //除去隐藏
    [MenuItem("Tools/Example/HideFlag")]
    private static void SetHideFlag()
    {
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);

        foreach (var item in AssetDatabase.LoadAllAssetsAtPath(path)) item.hideFlags = HideFlags.None;

        Test12();
        AssetDatabase.ImportAsset(path);
    }


    //移除资产
    [MenuItem("Tools/Example/Remove Slelect Object")]
    private static void Remove()
    {
        var parent = AssetDatabase.LoadAssetAtPath<ParentScriptableObject>(path);

        if (parent.child != null) DestroyImmediate(parent.child);

        parent.child = null;

        AssetDatabase.ImportAsset(path);
        Test12();
    }

    private static void Test12()
    {
        // sd
    }
}