using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.SceneManagement;
using System.Linq;
using UnityEditor;


public class MAP_tileFunctions : EditorWindow
{


    public static void addTile(Vector3 position)
    {
        GameObject placedTile;
        if (MAP_Editor.findTileMapParent())
        {
            if (MAP_Editor.useAltTiles == true)
            {
                s_AltTiles checkAltTiles = new s_AltTiles();
                try
                {
                    checkAltTiles = MAP_Editor.altTiles.Single(s => s.masterTile == MAP_Editor.currentTile.name);
                }
                catch { }
                if (checkAltTiles.masterTile != null)
                {
                    int randomTile = UnityEngine.Random.Range(0, checkAltTiles.altTileObjects.Length + 1);
                    if (randomTile < checkAltTiles.altTileObjects.Length)
                    {
                        placedTile = PrefabUtility.InstantiatePrefab(checkAltTiles.altTileObjects[randomTile] as GameObject) as GameObject;
                    }
                    else
                    {
                        placedTile = PrefabUtility.InstantiatePrefab(MAP_Editor.currentTile as GameObject) as GameObject;
                    }
                }
                else
                {
                    placedTile = PrefabUtility.InstantiatePrefab(MAP_Editor.currentTile as GameObject) as GameObject;
                }
            }
            else
            {
                placedTile = PrefabUtility.InstantiatePrefab(MAP_Editor.currentTile as GameObject) as GameObject;
            }

            Undo.RegisterCreatedObjectUndo(placedTile, Define.MAP_PLACED_TILE);
            if (MAP_Editor.randomRotationMode)
            {
                placedTile.transform.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 4) * 90, UnityEngine.Random.Range(0, 4) * 90, UnityEngine.Random.Range(0, 4) * 90);
            }
            else
            {
                placedTile.transform.eulerAngles = new Vector3(MAP_Editor.tileRotationX, MAP_Editor.tileRotation, 0);
            }

            placedTile.transform.position = position;
            placedTile.transform.localScale = MAP_Editor.brushTile.transform.localScale;
            placedTile.transform.parent = MAP_Editor.mapLayers[MAP_Editor.currentLayer - 1].transform;

        }
        EditorSceneManager.MarkAllScenesDirty();
    }

    public static void eraseTile(Vector3 position)
    {
        if (MAP_Editor.findTileMapParent())
        {
            GameObject currentLayer = MAP_Editor.mapLayers[MAP_Editor.currentLayer - 1];
            Vector3 tempVec3;

            for (int i = 0; i < currentLayer.transform.childCount; i++)
            {
                tempVec3 = currentLayer.transform.GetChild(i).transform.position;
                if (tempVec3.x == position.x && tempVec3.z == position.z && tempVec3.y >= position.y && tempVec3.y < position.y + 1f)
                {
                    Undo.DestroyObjectImmediate(currentLayer.transform.GetChild(i).gameObject);
                    EditorSceneManager.MarkAllScenesDirty();
                    return;
                }
            }
        }
    }

    public static void pickTile(Vector3 position)
    {
        if (MAP_Editor.findTileMapParent())
        {
            GameObject currentLayer = MAP_Editor.mapLayers[MAP_Editor.currentLayer - 1];
            Vector3 tempVec3;
            for (int i = 0; i < currentLayer.transform.childCount; ++i)
            {
                tempVec3 = currentLayer.transform.GetChild(i).transform.position;

                if (position.x == tempVec3.x && position.z == tempVec3.z && tempVec3.y >= position.y && tempVec3.y < position.y + 1)
                {
                    MAP_Editor.currentTile = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(currentLayer.transform.GetChild(i).transform.gameObject)), typeof(GameObject)) as GameObject;

                    float pickRotation = 0;
                    float pickRotationX = 0;

                    Transform pickTileTransform = currentLayer.transform.GetChild(i).transform;
                    if (pickTileTransform.eulerAngles.y > 0)
                    {
                        pickRotation = pickTileTransform.eulerAngles.y;
                    }

                    if (pickTileTransform.eulerAngles.x > 0)
                    {
                        pickRotationX = pickTileTransform.eulerAngles.x;
                    }

                    MAP_Editor.currentBrushIndex = Array.IndexOf(MAP_Editor.currentTileSetObjects, MAP_Editor.currentTile);
                    MAP_Editor.tileRotation = pickRotation;
                    MAP_Editor.tileRotationX = pickRotationX;

                    tempVec3 = pickTileTransform.localScale;
                    tempVec3.x /= MAP_Editor.globalScale;
                    tempVec3.y /= MAP_Editor.globalScale;
                    tempVec3.z /= MAP_Editor.globalScale;

                    MAP_Editor.tileScale = tempVec3;

                    MAP_brushFunctions.updateBrushTile(pickTileTransform.localScale);
                    MAP_Editor.currentBrushType = eBrushTypes.standardBrush;
                    MAP_Editor.selectTool = eToolIcons.brushTool;
                    return;
                }
            }
        }
    }

    public static void flipHorizontal()
    {
        Undo.RecordObject(MAP_Editor.brushTile.transform, Define.FLIP_HORZONTAL);
        if (MAP_Editor.tileScale.x == 1)
        {
            MAP_Editor.tileScale.x = -1;
        }
        else
        {
            MAP_Editor.tileScale.x = 1;
        }
    }

    public static void flipVertical()
    {
        Undo.RecordObject(MAP_Editor.brushTile.transform, Define.FLIP_VERTICAL);
        if (MAP_Editor.tileScale.z == 1)
        {
            MAP_Editor.tileScale.z = -1;
        }
        else
        {
            MAP_Editor.tileScale.z = 1;
        }
    }


    public static void checkTileSelectionStatus()
    {
        if (MAP_Editor.selectTiles.Count == 0 && Selection.gameObjects.Length > 0)
        {
            MAP_Editor.selectTiles.Clear();
            foreach (GameObject item in Selection.gameObjects)
            {
                if (item.GetComponent<MAP_tileGizmo>() != null)
                {
                    MAP_Editor.selectTiles.Add(item);
                }
            }
        }
    }


    public static void selectTile(Vector3 position)
    {
        if (MAP_Editor.findTileMapParent())
        {
            GameObject currentLayer = MAP_Editor.mapLayers[MAP_Editor.currentLayer - 1];
            for (int i = 0; i < currentLayer.transform.childCount; i++)
            {
                float distanceToTile = Vector3.Distance(currentLayer.transform.GetChild(i).transform.position, position);
                if (distanceToTile < 0.1f && currentLayer.transform.GetChild(i).transform.name != Define.MAP_BRUSH_TILE)
                {
                    for (int t = 0; t < MAP_Editor.selectTiles.Count; t++)
                    {
                        if (currentLayer.transform.GetChild(i).transform.position == MAP_Editor.selectTiles[t].transform.position)
                        {
                            return;
                        }
                    }
                    MAP_Editor.selectTiles.Add(currentLayer.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }
    }

    public static void delSelectTile(Vector3 position)
    {
        if (MAP_Editor.findTileMapParent())
        {
            GameObject currentLayer = MAP_Editor.mapLayers[MAP_Editor.currentLayer - 1];
            for (int i = 0; i < currentLayer.transform.childCount; i++)
            {
                float distanceToTile = Vector3.Distance(currentLayer.transform.GetChild(i).transform.position, position);
                if (distanceToTile < 0.1f && currentLayer.transform.GetChild(i).transform.name != Define.MAP_BRUSH_TILE)
                {
                    for (int t = 0; t < MAP_Editor.selectTiles.Count; t++)
                    {
                        if (currentLayer.transform.GetChild(i).transform.position == MAP_Editor.selectTiles[i].transform.position)
                        {
                            MAP_Editor.selectTiles.RemoveAt(t);
                            return;
                        }
                    }
                }
            }

        }
    }

    public static void selectAllTiles()
    {
        if (MAP_Editor.findTileMapParent())
        {
            GameObject currentLayer = MAP_Editor.mapLayers[MAP_Editor.currentLayer - 1];
            if (MAP_Editor.selectTiles.Count > 0)
            {
                MAP_Editor.selectTiles.Clear();
            }
            else
            {
                for (int i = 0; i < currentLayer.transform.childCount; i++)
                {
                    MAP_Editor.selectTiles.Add(currentLayer.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    public static void trashTiles()
    {
        Undo.RegisterFullObjectHierarchyUndo(MAP_Editor.tileMapParent, Define.TRASH_TILES);
        checkTileSelectionStatus();
        foreach (GameObject tile in MAP_Editor.selectTiles)
        {
            DestroyImmediate(tile);
        }
        EditorSceneManager.MarkAllScenesDirty();
        MAP_Editor.selectTiles.Clear();
    }

    public static void isolateTilesToggle()
    {
        if (MAP_Editor.isolataTiles == false)
        {
            isolateGridTiles();
            MAP_Editor.isolataTiles = true;
        }
        else
        {
            restoreIsolatedGridTiles();
            MAP_Editor.isolataTiles = true;
        }
    }

    public static void isolateLayerToggle()
    {
        if (MAP_Editor.isolateLayer == false)
        {
            isolateLayerTiles();
            MAP_Editor.isolateLayer = true;
        }
        else
        {
            restoreIsolatedLayerTiles();
            MAP_Editor.isolateLayer = false;
        }
    }

    public static void isolateGridTiles()
    {
        restoreIsolatedGridTiles();
        if (MAP_Editor.findTileMapParent())
        {
            foreach (Transform layer in MAP_Editor.tileMapParent.transform)
            {
                foreach (Transform tile in layer)
                {
                    if (!MAP_Editor.editorPreferences.twoPointFiveDMode)
                    {
                        tile.gameObject.SetActive(false);
                        MAP_Editor.isolatedGridObjects.Add(tile.gameObject);
                    }
                    else
                    {
                        tile.gameObject.SetActive(false);
                        MAP_Editor.isolatedGridObjects.Add(tile.gameObject);
                    }
                }
            }
        }
    }

    public static void restoreIsolatedGridTiles()
    {
        if (MAP_Editor.isolatedGridObjects.Count > 0)
        {
            foreach (GameObject tile in MAP_Editor.isolatedGridObjects)
            {
                if (tile != null)
                {
                    tile.SetActive(true);
                }
            }
        }
        MAP_Editor.isolatedGridObjects.Clear();
    }

    public static void isolateLayerTiles()
    {
        restoreIsolatedLayerTiles();
        if (MAP_Editor.findTileMapParent())
        {
            foreach (Transform item in MAP_Editor.tileMapParent.transform)
            {
                if (item.name != string.Format("layer{0}", MAP_Editor.currentLayer))
                {
                    item.gameObject.SetActive(false);
                    MAP_Editor.isolataLayerObjects.Add(item.gameObject);
                }
            }
        }
    }

    public static void restoreIsolatedLayerTiles()
    {
        if (MAP_Editor.isolataLayerObjects.Count > 0)
        {
            foreach (GameObject tile in MAP_Editor.isolataLayerObjects)
            {
                if (tile != null)
                {
                    tile.SetActive(true);
                }
            }
        }
        MAP_Editor.isolataLayerObjects.Clear();
    }



}
