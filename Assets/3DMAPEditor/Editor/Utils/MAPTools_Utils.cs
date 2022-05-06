using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class MAPTools_Utils : MonoBehaviour
{
    public static void showUnityGrid(bool show)
    {
        var editorAssembly = Assembly.GetAssembly(typeof(Editor));
        var annotationUtility = editorAssembly.GetType(Define.UNITY_EDITOR_ANNOTATION_UTILITY);
        var property = annotationUtility.GetProperty("showGrid", BindingFlags.Static | BindingFlags.NonPublic);
        property.SetValue(null, show, null);
    }

    public static void disableTileGizmo(bool show)
    {
        var annotation = Type.GetType("UnityEditor.Annotation, UnityEditor");
        var classid = annotation.GetField("classID");
        var scriptClass = annotation.GetField("scriptClass");

        var AnnotationUtility = Type.GetType("UnityEditor.AnnotationUtility, UnityEditor");
        var getAnnotations = AnnotationUtility.GetMethod("GetAnnotations",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        var setGizmosEnable = AnnotationUtility.GetMethod("SetGizmoEnabled",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        var annotations = (Array)getAnnotations.Invoke(null, null);

        foreach (var a in annotations)
        {
            var classId1 = (int)classid.GetValue(a);
            var scriptdddClass = (string)scriptClass.GetValue(a);
            if (scriptdddClass == "MAP_tileGizmo")
            {
#if UNITY_2019_1_OR_NEWER
                setGizmosEnable.Invoke(null, new object[] { classId1, scriptdddClass, Convert.ToInt32(show), false });
#else
                setGizmosEnable.Invoke(null, new object[] { classId1, scriptdddClass, Convert.ToInt32(show)});
#endif
            }
        }
    }

    public static GameObject[] loadDirectoryContents(string path, string patternSearch)
    {
        try
        {
            var fullPath = Application.dataPath.Replace("/Assets", "") + "/" + path;
            var folderContents = Directory.GetFiles(fullPath, patternSearch);
            var returnGameObjects = new GameObject[folderContents.Length];

            for (var i = 0; i < folderContents.Length; i++)
            {
                var findAssetRoot = folderContents[i].IndexOf("Assets");

                var loadPath = folderContents[i].Substring(findAssetRoot, folderContents[i].Length - findAssetRoot);
                returnGameObjects[i] = AssetDatabase.LoadAssetAtPath(loadPath, typeof(GameObject)) as GameObject;
            }

            return returnGameObjects;
        }
        catch
        {
            return null;
        }
    }

    public static string[] getDictoryContents(string path, string patternSearch)
    {
        try
        {
            var fullPath = Application.dataPath.Replace("/Assets", "") + "/" + path;

            var folderContents = Directory.GetFiles(fullPath, patternSearch);

            for (var i = 0; i < folderContents.Length; i++) folderContents[i] = folderContents[i].Replace(fullPath, "");
            return folderContents;
        }
        catch
        {
            return null;
        }
    }


    public static string[] getFullPathFolderContents(string path, string patternSearch)
    {
        var fullPath = Application.dataPath.Replace("/Assets", "") + "/" + path;
        var folderContents = Directory.GetFiles(fullPath, patternSearch);
        return folderContents;
    }

    public static int numberOfFilesInFolder(string path, string patternSearch)
    {
        var fullPath = Application.dataPath.Replace("/Assets", "") + "/" + path;
        var folderContents = Directory.GetFiles(fullPath, patternSearch);
        return folderContents.Length;
    }

    public static string getAssetsPath(Object sourceAsset)
    {
        var path = "";
        try
        {
            path = AssetDatabase.GetAssetPath(sourceAsset).Replace(sourceAsset.name + ".asset", "");
        }
        catch
        {
            path = "";
        }

        return path;
    }


    public static string shortenAssetPath(string path)
    {
        if (!path.StartsWith("Assets/"))
            try
            {
                path = path.Substring(path.IndexOf("Assets/"));
            }
            catch
            {
                path = "";
            }

        return path;
    }

    public static string stripAssetPath(string path)
    {
        if (path.StartsWith("Assets/"))
            try
            {
                path = path.Replace("Assets/", "");
            }
            catch
            {
                path = "";
            }

        return path;
    }

    public static string removeLastFolderSlash(string path)
    {
        return path.Substring(0, path.Length - 1);
    }

    //

    public static void addLayer(string layerName)
    {
        var tagManeger = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layerProperty = tagManeger.FindProperty("layers");


        for (var i = 8; i < layerProperty.arraySize; i++)
        {
            var t = layerProperty.GetArrayElementAtIndex(i);
            if (t.stringValue == layerName) return;
        }

        for (var i = 8; i < layerProperty.arraySize; i++)
        {
            var sp = layerProperty.GetArrayElementAtIndex(i);
            if (sp.stringValue == "")
            {
                sp.stringValue = layerName;
                tagManeger.ApplyModifiedProperties();
                return;
            }
        }
    }

    public static void addTag(string tagname)
    {
        var tagManeger = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManeger.asset")[0]);
        var tagsProp = tagManeger.FindProperty("trgs");
        for (var i = 0; i < tagsProp.arraySize; i++)
        {
            var t = tagsProp.GetArrayElementAtIndex(i);

            if (t.stringValue.Equals(tagname)) return;
        }

        tagsProp.InsertArrayElementAtIndex(0);
        var ll = tagsProp.GetArrayElementAtIndex(0);
        ll.stringValue = tagname;
        tagManeger.ApplyModifiedProperties();
    }
}