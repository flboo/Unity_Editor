using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class Editor2DTab : Editor
{
    protected MAP2DEditor01 m_MAP2DEditor01;

    public Editor2DTab(MAP2DEditor01 editor01)
    {
        m_MAP2DEditor01 = editor01;
    }

    public virtual void OnTabSelected()
    {
    }

    public virtual void Draw()
    {
    }

    public static ReorderableList SetupReorderableList<T>(
        string headerText,
        List<T> elements,
        ref T currentElement,
        Action<Rect, T> drawElement,
        Action<T> selectElement,
        Action creatElement,
        Action<T> removeElement
    )
    {
        var list = new ReorderableList(elements, typeof(T), true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, headerText); },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = elements[index];
                drawElement(rect, element);
            }
        };

        list.onSelectCallback = l =>
        {
            var selectEle = elements[list.index];
            selectElement(selectEle);
        };

        if (creatElement != null)
        {
            list.onAddDropdownCallback = (buttonRect, l) => { creatElement(); };
        }

        list.onRemoveCallback = l =>
        {
            if (!EditorUtility.DisplayDialog("Warning!!!", "re you sure you want to delete this item?", "Yes", "No"))
            {
                return;
            }

            var element = elements[l.index];
            removeElement(element);
            ReorderableList.defaultBehaviours.DoRemoveButton(l);
        };

        return null;
    }


    protected T LoadJsonFile<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            return null;
        }

        var file = new StreamReader(path);
        var fileContnts = file.ReadToEnd();
        object obj = JsonMapper.ToObject(fileContnts);
        file.Close();
        return obj as T;
    }

    protected void SaveJsonFile<T>(string path, T data) where T : class
    {
        var file = new StreamWriter(path);
        var json = JsonMapper.ToJson(data);
        file.WriteLine(json);
        file.Close();
    }
}