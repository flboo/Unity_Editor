using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class MAP_EditorData : ScriptableObject
{
    public Texture2D mapEditorHeader;
    public Texture2D titleImporterHeader;
    public Texture2D titleConverterHeader;
    public Texture2D configButton;
    public List<Texture> primaryIconData = new List<Texture>();
    public List<string> primaryIconTooltip = new List<string>();
    public List<Texture> secondaryIconData = new List<Texture>();
    public List<string> secondaryIconTooltip = new List<string>();
    public List<Texture> selectIconData = new List<Texture>();
    public List<string> selectionIconTooltip = new List<string>();
    public List<Texture> layerIconData = new List<Texture>();
    public List<string> layerIconTooltip = new List<string>();
}