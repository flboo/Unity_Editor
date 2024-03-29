using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MAP_customBrushFunctions : EditorWindow
{
    private static int subMeshCount;

    public static void createCustomBrush()
    {
        var customBrushDialog = GetWindow<YuME_customBrushNameDialog>(true, Define.CUSTOM_BTUSH_NAME);
        customBrushDialog.titleContent.text = Define.CUSTOM_BTUSH_NAME;
    }

    public static void createCustomBrush(string customBrushName)
    {
        subMeshCount = 0;

        MAP_tileFunctions.checkTileSelectionStatus();

        var tileParentObject = new GameObject();
        tileParentObject.AddComponent<MAP_tileGizmo>();
        tileParentObject.GetComponent<MAP_tileGizmo>().customBrushMeshName = new List<string>();

        var customBrushGUID = Guid.NewGuid().ToString("N");

        tileParentObject.name = customBrushName + Define.MAP_PREFAB;

        var destinationPath =
            MAPTools_Utils.getAssetsPath(MAP_Editor.availableTileSets[MAP_Editor.currentTileSetIndex]) +
            Define.CUSTOM_BRUSHFES + "/";

        if (MAP_Editor.selectTiles.Count > 0)
        {
            // When creating a custom brush we need to find the lowest Z transform in the selection to become the pivot transform
            var bottomLevelOfSelection = MAP_Editor.selectTiles[0];

            foreach (var checkObjects in MAP_Editor.selectTiles)
                if (checkObjects.transform.position.z < bottomLevelOfSelection.transform.position.z)
                    bottomLevelOfSelection = checkObjects;

            // center the parent object around the lowest block to make sure all the selected brushes are centered around the parent
            tileParentObject.transform.position = bottomLevelOfSelection.transform.position;

            // New Custom Brush implementation. Uses a technique similar to the freeze map, except for selected tils

            var tilesToCombine = new List<GameObject>();
            var _freezeTiles = new List<Transform>();
            var freezeTiles = new List<Transform>();

            foreach (var tile in MAP_Editor.selectTiles)
            {
                tile.GetComponentsInChildren(false, _freezeTiles);
                freezeTiles.AddRange(_freezeTiles);
            }

            foreach (var tile in freezeTiles)
                if (tile.GetComponent<MeshRenderer>())
                    tilesToCombine.Add(tile.gameObject);

            tilesToCombine = tilesToCombine.OrderBy(x => x.GetComponent<MeshRenderer>().sharedMaterial.name).ToList();

            var previousMaterial = tilesToCombine[0].GetComponent<MeshRenderer>().sharedMaterial;

            var combine = new List<CombineInstance>();
            var tempCombine = new CombineInstance();

            var vertexCount = 0;

            foreach (var mesh in tilesToCombine)
            {
                vertexCount += mesh.GetComponent<MeshFilter>().sharedMesh.vertexCount;

                if (vertexCount > 60000)
                {
                    vertexCount = 0;
                    newSubMesh(combine, mesh.GetComponent<MeshRenderer>().sharedMaterial, tileParentObject,
                        destinationPath, customBrushGUID);
                    combine = new List<CombineInstance>();
                }

                if (mesh.GetComponent<MeshRenderer>().sharedMaterial.name != previousMaterial.name)
                {
                    newSubMesh(combine, previousMaterial, tileParentObject, destinationPath, customBrushGUID);
                    combine = new List<CombineInstance>();
                }

                tempCombine.mesh = mesh.GetComponent<MeshFilter>().sharedMesh;
                tempCombine.transform = mesh.GetComponent<MeshFilter>().transform.localToWorldMatrix;
                combine.Add(tempCombine);
                previousMaterial = mesh.GetComponent<MeshRenderer>().sharedMaterial;
            }

            newSubMesh(combine, previousMaterial, tileParentObject, destinationPath, customBrushGUID);

            tileParentObject.transform.position = Vector3.zero;

            // Add the prefab to the project

#if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(tileParentObject, destinationPath + tileParentObject.name);
#else
            PrefabUtility.CreatePrefab(destinationPath + tileParentObject.name, tileParentObject);
#endif
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); // refesh the asset database to tell unity changes have been made

            // remove our temporary builder object

            Debug.Log("Custom Brush " + tileParentObject.name + " created.");

            DestroyImmediate(tileParentObject);
            MAP_Editor.selectTiles.Clear();

            // reload the custom brushes
            MAP_Editor.loadCustomBrushes();
        }
    }

    private static void newSubMesh(List<CombineInstance> combine, Material mat, GameObject tileParentObject,
        string destinationPath, string customBrushGUID)
    {
        var subMesh = new GameObject();
        subMesh.transform.parent = tileParentObject.transform;
        subMesh.name = tileParentObject.name.Replace(Define.MAP_PREFAB, "") + "SubMesh" + subMeshCount +
                       customBrushGUID;
        subMesh.isStatic = false;

        var subMeshFilter = subMesh.AddComponent<MeshFilter>();
        subMeshFilter.sharedMesh = new Mesh();
        subMeshFilter.sharedMesh.CombineMeshes(combine.ToArray());

        var meshRenderer = subMesh.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        subMesh.AddComponent<MeshCollider>().sharedMesh = subMeshFilter.sharedMesh;
        subMesh.SetActive(true);

        tileParentObject.GetComponent<MAP_tileGizmo>().customBrushMeshName.Add(subMesh.name);

        Debug.Log(destinationPath + subMesh.name + ".asset");
        AssetDatabase.CreateAsset(subMeshFilter.sharedMesh, destinationPath + subMesh.name + ".asset");

        subMeshCount++;
    }

    public static GameObject getPrefabFromCurrentTiles(GameObject source)
    {
        foreach (var tile in MAP_Editor.currentTileSetObjects)
            if (tile.name == source.name)
                return tile;

        return null;
    }

    // Need to keep this for legacy custom brushes
    // NOTE: I have removed the UNDO. This is for stability issues. 

    public static void pasteCustomBrush(Vector3 position)
    {
        if (MAP_Editor.brushTile != null)
            if (MAP_Editor.findTileMapParent())
            {
                var badTileCount = 0;

                foreach (Transform child in MAP_Editor.brushTile.transform)
                {
                    var pasteTile =
                        PrefabUtility.InstantiatePrefab(getPrefabFromCurrentTiles(child.gameObject)) as GameObject;

                    if (pasteTile != null)
                    {
                        child.position = normalizePosition(child.position);
                        MAP_tileFunctions.eraseTile(child.position);
                        pasteTile.transform.eulerAngles = child.eulerAngles;
                        pasteTile.transform.position = child.position;
                        pasteTile.transform.localScale = child.transform.lossyScale;
                        pasteTile.transform.SetParent(MAP_Editor.mapLayers[MAP_Editor.currentLayer - 1].transform);
                    }
                    else
                    {
                        badTileCount++;
                    }
                }

                if (badTileCount > 0)
                    Debug.Log(
                        "Custom Brush includes tiles from a different tile set. These tiles will not appear in the scene due to the lack of nested prefabs in Unity.");

                EditorSceneManager.MarkAllScenesDirty();
            }
    }


    public static void createCopyBrush(bool destroySourceTiles)
    {
        MAP_tileFunctions.checkTileSelectionStatus();

        if (MAP_Editor.currentBrushType != eBrushTypes.copyBrush && MAP_Editor.selectTiles.Count > 0)
        {
            MAP_Editor.currentBrushType = eBrushTypes.copyBrush;

            if (MAP_Editor.brushTile != null) DestroyImmediate(MAP_Editor.brushTile);

            if (MAP_Editor.findTileMapParent())
            {
                Undo.RegisterFullObjectHierarchyUndo(MAP_Editor.tileMapParent, "Create Brush");

                MAP_Editor.brushTile = new GameObject();
                MAP_Editor.brushTile.transform.eulerAngles =
                    new Vector3(MAP_Editor.tileRotationX, MAP_Editor.tileRotation, 0f);
                MAP_Editor.brushTile.transform.parent = MAP_Editor.tileMapParent.transform;
                MAP_Editor.brushTile.name = "YuME_brushTile";

                MAP_Editor.brushTile.transform.position = MAP_Editor.selectTiles[0].transform.position;

                MAP_Editor.tileChildObjects.Clear();

                foreach (var tile in MAP_Editor.selectTiles)
                {
#if UNITY_2018_3_OR_NEWER
                    var tempTile =
                        (GameObject)PrefabUtility.InstantiatePrefab(
                            PrefabUtility.GetCorrespondingObjectFromSource(tile));
#else
                    GameObject tempTile =
 (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.GetPrefabParent(tile) as GameObject);
#endif
                    tempTile.transform.parent = MAP_Editor.brushTile.transform;
                    tempTile.transform.position = tile.transform.position;
                    tempTile.transform.eulerAngles = tile.transform.eulerAngles;
                    tempTile.transform.localScale = tile.transform.localScale;

                    MAP_Editor.tileChildObjects.Add(tempTile);

                    if (destroySourceTiles) DestroyImmediate(tile);
                }

                MAP_Editor.selectTiles.Clear();

                MAP_Editor.showWireBrush = false;
            }
        }
    }

    public static void pasteCopyBrush(Vector3 position)
    {
        if (MAP_Editor.brushTile != null)
            if (MAP_Editor.findTileMapParent())
            {
                Undo.RegisterFullObjectHierarchyUndo(MAP_Editor.tileMapParent, Define.PAINT_BRUSH);

                foreach (Transform child in MAP_Editor.brushTile.transform)
                {
                    var pasteTile =
                        (GameObject)PrefabUtility.InstantiatePrefab(
                            PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject));
                    MAP_tileFunctions.eraseTile(child.position);
                    pasteTile.transform.eulerAngles = child.eulerAngles;
                    pasteTile.transform.position = normalizePosition(child.position);
                    pasteTile.transform.localScale = child.transform.lossyScale;
                    pasteTile.transform.parent = MAP_Editor.mapLayers[MAP_Editor.currentLayer - 1].transform;
                }

                EditorSceneManager.MarkAllScenesDirty();
            }
    }

    private static Vector3 normalizePosition(Vector3 position)
    {
        position.x = (float)Math.Round(position.x * 4, MidpointRounding.ToEven) / 4;
        position.y = (float)Math.Round(position.y * 4, MidpointRounding.ToEven) / 4;
        position.z = (float)Math.Round(position.z * 4, MidpointRounding.ToEven) / 4;

        return position;
    }
}