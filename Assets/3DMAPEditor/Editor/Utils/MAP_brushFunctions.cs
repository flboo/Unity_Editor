using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MAP_brushFunctions : EditorWindow
{
    static List<GameObject> brushTilesInParent = new List<GameObject>();

    public static void updateBrushPosition()
    {
        if (MAP_Editor.brushTile != null)
        {
            MAP_Editor.brushTile.transform.position = MAP_Editor.tilePosition;
            MAP_Editor.brushTile.transform.eulerAngles = new Vector3(MAP_Editor.tileRotationX, MAP_Editor.tileRotation, 0f);
            if (MAP_Editor.currentBrushType == eBrushTypes.standardBrush)
            {
                MAP_Editor.brushTile.transform.localScale = Vector3.Scale(new Vector3(MAP_Editor.globalScale, MAP_Editor.globalScale, MAP_Editor.globalScale), MAP_Editor.tileScale);
            }
            else
            {
                MAP_Editor.brushTile.transform.localScale = Vector3.Scale(Vector3.one, MAP_Editor.tileScale);
            }

            if (MAP_Editor.eraseToolOverride || MAP_Editor.pickToolOverride)
            {
                foreach (GameObject child in MAP_Editor.tileChildObjects)
                {
                    child.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject child in MAP_Editor.tileChildObjects)
                {
                    child.SetActive(true);
                }
            }
        }
    }

    public static void displayBrush(bool display)
    {
        if (MAP_Editor.selectTool != MAP_Editor.previousSelectTool)
        {
            foreach (Transform child in MAP_Editor.brushTile.transform)
            {
                MAP_Editor.showWireBrush = !display;
                child.gameObject.SetActive(display);
            }
        }
    }


    public static void createBrushTile()
    {
        if (MAP_Editor.selectTool != MAP_Editor.previousSelectTool)
        {
            updateBrushTile();
        }
    }

    public static void updateBrushTile()
    {
        MAP_Editor.pickTileScale = Vector3.zero;

        if (MAP_Editor.currentTile != null)
            updateBrushTile(MAP_Editor.currentTile.transform.localScale);
    }

    public static void updateBrushTile(Vector3 tileScale)
    {
        if (MAP_Editor.findTileMapParent() && MAP_Editor.toolEnabled)
        {
            destoryBrushTile();
            MAP_Editor.brushTile = Instantiate(MAP_Editor.currentTile, Vector3.one, Quaternion.identity) as GameObject;
            MAP_Editor.brushTile.transform.eulerAngles = new Vector3(MAP_Editor.tileRotationX, MAP_Editor.tileRotation, 0);
            MAP_Editor.brushTile.transform.parent = MAP_Editor.tileMapParent.transform;
            MAP_Editor.brushTile.transform.localScale = tileScale;
            MAP_Editor.brushTile.name = Define.MAP_BRUSH_TILE;
            MAP_Editor.brushTile.hideFlags = HideFlags.HideAndDontSave;
            MAP_Editor.brushTile.isStatic = false;

            foreach (Transform child in MAP_Editor.brushTile.transform)
            {
                child.gameObject.isStatic = false;
            }
            MAP_Editor.tileChildObjects.Clear();
            foreach (Transform child in MAP_Editor.brushTile.transform)
            {
                MAP_Editor.tileChildObjects.Add(child.gameObject);
            }
            MAP_Editor.showWireBrush = false;
        }
    }

    public static void destoryBrushTile()
    {
        brushTilesInParent.Clear();
        foreach (Transform child in MAP_Editor.tileMapParent.transform)
        {
            if (child.name == Define.MAP_BRUSH_TILE)
            {
                brushTilesInParent.Add(child.gameObject);
            }
        }
        for (int i = 0; i < brushTilesInParent.Count; i++)
        {
            DestroyImmediate(brushTilesInParent[i]);
        }
        SceneView.RepaintAll();
        MAP_Editor.showWireBrush = true;
    }

    public static void cleanSceneOfBrushObjects()
    {
        if (MAP_Editor.findTileMapParent())
        {
            List<GameObject> destoryList = new List<GameObject>();
            foreach (Transform child in MAP_Editor.tileMapParent.transform)
            {
                if (child.name == Define.MAP_BRUSH_TILE)
                {
                    destoryList.Add(child.gameObject);
                }
            }
            foreach (GameObject obj in destoryList)
            {
                DestroyImmediate(obj);
            }
        }
    }




}
