using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class MAP_tilesetData : ScriptableObject
{
    public string tileSetName;
    public string customBrushDestinationFolder;
    public List<string> tileData = new List<string>();
    public List<string> customBrushData = new List<string>();
}