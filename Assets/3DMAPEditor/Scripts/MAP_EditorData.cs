using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MAP_EditorData : ScriptableObject
{
    public Texture2D mapEditorHeader;
    public Texture2D titleImporterHeader;
    public Texture2D titleConverterHeader;
    public Texture2D configButton;
    public List<Texture> primaryIconData = new();
    public List<string> primaryIconTooltip = new();
    public List<Texture> secondaryIconData = new();
    public List<string> secondaryIconTooltip = new();
    public List<Texture> selectIconData = new();
    public List<string> selectionIconTooltip = new();
    public List<Texture> layerIconData = new();
    public List<string> layerIconTooltip = new();
}