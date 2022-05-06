using System.Collections.Generic;
using UnityEngine;

public class MAP_editorPreferences : ScriptableObject
{
    public List<string> layerNames = new();
    public List<bool> layerFreeze = new();
    public List<bool> layerStatic = new();
    public bool convexCollision;
    public float iconWidth = 32f;
    public Color brushCursorColor = new(1f, 0.78f, 0.52f, 1f);
    public Color pickCursorColor = new(0.35f, 0.93f, 0.45f, 1f);
    public Color eraseCursorColor = new(1f, 0f, 0f, 1f);
    public float gridHeight;
    public Color gridColorNormal = new(0.32f, 0.28f, 0.28f, 1f);
    public Color gridColorBorder = new(0.19f, 0.34f, 0.54f, 1f);
    public Color gridColorFill = new(0.36f, 0.36f, 0.36f, 0.78F);
    public float gridOffset = 0.01f;
    public float gridLayerHeightScaler = 1f;
    public Vector2 gridDimensions = new(40f, 40f);
    public float gridScaleFactor = 1f;
    public bool invertMouseWheel;
    public Vector3 initialOffset = Vector3.zero;
    public bool twoPointFiveDMode;
    public bool centreGrid = true;
    public bool useAlternativeKeyShortcuts;
    public bool hideUIObjects;
}