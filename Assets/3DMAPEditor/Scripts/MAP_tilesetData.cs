using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MAP_tilesetData : ScriptableObject
{
    public string tileSetName;
    public string customBrushDestinationFolder;
    public List<string> tileData = new();
    public List<string> customBrushData = new();
}