﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public class MAP_freezeMap : EditorWindow
{
    private static GameObject frozenMap;

    public static void combineTiles()
    {
        if (MAP_Editor.tileMapParent)
        {
            frozenMap = new GameObject();
            frozenMap.transform.SetParent(MAP_Editor.tileMapParent.transform);
            frozenMap.name = Define.FROZEN_MAP;

            List<GameObject> tilesToCombine = new List<GameObject>();
            List<GameObject> lightsToCreate = new List<GameObject>();
            List<Transform> freezeTiles = new List<Transform>();
            List<Transform> _freezeTiles = new List<Transform>();

            EditorUtility.DisplayProgressBar("building frozen map", "finding tiles to freeze", 0f);

            for (int i = 0; i < 8; i++)
            {
                if (MAP_Editor.editorPreferences.layerFreeze[i])
                {
                    MAP_Editor.mapLayers[i].GetComponentsInChildren<Transform>(false, _freezeTiles);
                    freezeTiles.AddRange(_freezeTiles);
                }
                else
                {
                    GameObject layerCopy = Instantiate(MAP_Editor.mapLayers[i], MAP_Editor.mapLayers[i].transform.position, Quaternion.identity) as GameObject;
                    layerCopy.transform.SetParent(frozenMap.transform);
                    if (!MAP_Editor.editorPreferences.layerStatic[i])
                    {
                        Transform[] tempTiles = layerCopy.GetComponentsInChildren<Transform>();
                        foreach (Transform item in tempTiles)
                        {
                            item.gameObject.isStatic = false;
                        }
                    }
                }
            }

            foreach (Transform tile in freezeTiles)
            {
                if (tile.GetComponent<MeshRenderer>())
                {
                    if (MAP_Editor.editorPreferences.convexCollision)
                    {
                        MeshCollider _tempCol = tile.GetComponent<MeshCollider>();
                        if (_tempCol != null)
                        {
                            _tempCol.convex = true;
                        }
                    }
                    tilesToCombine.Add(tile.gameObject);
                }

                if (tile.GetComponent<Light>())
                {
                    lightsToCreate.Add(tile.gameObject);
                }
            }

            foreach (GameObject light in lightsToCreate)
            {
                GameObject newLight = GameObject.Instantiate(light);
                newLight.isStatic = true;
                newLight.transform.position = light.transform.position;
                newLight.transform.eulerAngles = light.transform.eulerAngles;
                newLight.transform.localScale = frozenMap.transform.localScale;
            }

            tilesToCombine = tilesToCombine.OrderBy(x => x.GetComponent<MeshRenderer>().sharedMaterial.name).ToList();

            Material previousMaterial = tilesToCombine[0].GetComponent<MeshRenderer>().sharedMaterial;

            List<CombineInstance> combine = new List<CombineInstance>();
            CombineInstance tempCombine = new CombineInstance();
            int vertexCount = 0;

            foreach (GameObject mesh in tilesToCombine)
            {
                vertexCount += mesh.GetComponent<MeshFilter>().sharedMesh.vertexCount;
                if (vertexCount > 60000)
                {
                    vertexCount = 0;
                    newSubMesh(combine, mesh.GetComponent<MeshRenderer>().sharedMaterial);
                    combine = new List<CombineInstance>();
                }
                if (mesh.GetComponent<MeshRenderer>().name != previousMaterial.name)
                {
                    newSubMesh(combine, previousMaterial);
                    combine = new List<CombineInstance>();
                }
                tempCombine.mesh = mesh.GetComponent<MeshFilter>().sharedMesh;
                tempCombine.transform = mesh.GetComponent<MeshFilter>().transform.localToWorldMatrix;
                combine.Add(tempCombine);
                previousMaterial = mesh.GetComponent<MeshRenderer>().sharedMaterial;
            }

            newSubMesh(combine, previousMaterial);

            foreach (Transform layer in MAP_Editor.tileMapParent.transform)
            {
                if (layer.name.Contains(Define.LAYER))
                {
                    layer.gameObject.SetActive(false);
                }
            }

            MAP_brushFunctions.cleanSceneOfBrushObjects();

            EditorUtility.ClearProgressBar();
            MAP_brushFunctions.destoryBrushTile();
            MAP_brushFunctions.cleanSceneOfBrushObjects();
            MAP_Editor.selectTool = eToolIcons.defaultTools;
        }

    }

    private static void newSubMesh(List<CombineInstance> combine, Material mat)
    {
        GameObject subMesh = new GameObject();
        subMesh.transform.SetParent(frozenMap.transform);
        subMesh.name = Define.SUB_MESH;

        MeshFilter subMeshFilter = subMesh.AddComponent<MeshFilter>();
        subMeshFilter.sharedMesh = new Mesh();
        subMeshFilter.sharedMesh.name = Define.SUB_MESH;

        subMesh.isStatic = true;
        subMeshFilter.sharedMesh.CombineMeshes(combine.ToArray());
        MeshRenderer meshRenderer = subMesh.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;
        subMesh.AddComponent<MeshCollider>().sharedMesh = subMeshFilter.sharedMesh;
        if (MAP_Editor.editorPreferences.convexCollision)
        {
            subMesh.GetComponent<MeshCollider>().convex = true;
        }
        subMesh.SetActive(true);
    }



    public static void saveFrozenMesh(string path)
    {
        path = MAPTools_Utils.shortenAssetPath(path);
        int counter = 1;

        if (MAP_Editor.findTileMapParent())
        {
            foreach (Transform child in MAP_Editor.tileMapParent.transform)
            {
                if (child.gameObject.name == "frozenMap")
                {
                    GameObject saveMap = Instantiate(child.gameObject);
                    saveMap.name = MAP_Editor.tileMapParent.name;

                    if (!AssetDatabase.IsValidFolder(path + "/" + saveMap.name + "Meshes"))
                    {
                        AssetDatabase.CreateFolder(path, saveMap.name + "Meshes");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                    }

                    EditorUtility.ClearProgressBar();

                    foreach (Transform frozenMesh in saveMap.transform)
                    {
                        EditorUtility.DisplayProgressBar("Saving Meshes", "Saving Mesh " + (counter) + ". This might take some time", 1);
                        Mesh saveMesh = Object.Instantiate(frozenMesh.GetComponent<MeshFilter>().sharedMesh) as Mesh;
                        //Unwrapping.GenerateSecondaryUVSet(saveMesh);
                        try
                        {
                            AssetDatabase.CreateAsset(saveMesh, path + "/" + saveMap.name + "Meshes/" + frozenMesh.name + counter + ".asset");
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to create saved map. This is likely due to a new folder being created and Unity not refreshing the asset database. Please retry saving the map.");
                            EditorUtility.ClearProgressBar();
                            return;
                        }
                        frozenMesh.GetComponent<MeshFilter>().mesh = saveMesh;
                        counter++;
                    }

                    EditorUtility.ClearProgressBar();

                    Object prefabAlreadyCreated = AssetDatabase.LoadAssetAtPath(path + "/" + saveMap.name + ".prefab", typeof(GameObject));

                    if (prefabAlreadyCreated != null)
                        PrefabUtility.SaveAsPrefabAssetAndConnect(saveMap, path + "/" + saveMap.name + ".prefab", InteractionMode.AutomatedAction);
                    else
                        PrefabUtility.SaveAsPrefabAsset(saveMap, path + "/" + saveMap.name + ".prefab");
                    AssetDatabase.SaveAssets();
                    if (saveMap != null)
                        DestroyImmediate(saveMap);
                }
            }

            AssetDatabase.Refresh();
        }









    }





}