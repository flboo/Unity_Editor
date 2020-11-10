using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_mapManagerFunctions
{
    public static void setDefaultMap()
    {
        GameObject mainmap = GameObject.Find(Define.CURRENT_MAP_NAME);
        if (mainmap == null)
        {
            Debug.LogError("cannot find the defaul map. Please ensure you have a  Map named map_MapData");
        }
        else
        {
            getGridSceneObjectReference(mainmap);
        }

    }

    public static void setActiveMap()
    {
        for (int i = 0; i < MAP_Editor.ref_MapManager.mapList.Count; i++)
        {
            if (MAP_Editor.ref_MapManager.mapList[i] != null)
            {
                MAP_Editor.ref_MapManager.mapList[i].SetActive(true);
            }
        }
        MAP_Editor.ref_MapManager.mapList[MAP_Editor.currentMapIndex].SetActive(true);
        MAP_brushFunctions.updateBrushTile();
    }

    public static GameObject buildNewMap(string mapName)
    {
        GameObject mainmap = new GameObject(mapName);
        for (int i = 0; i < 9; i++)
        {
            GameObject layer = new GameObject(Define.LAYER + i);
            layer.transform.parent = mainmap.transform;
            layer.transform.position = Vector3.zero;
        }
        getGridSceneObjectReference(mainmap);
        return mainmap;
    }

    public static void getGridSceneObjectReference()
    {
        if (MAP_Editor.gridSceneObject == null)
        {
            MAP_Editor.gridSceneObject = GameObject.Find(Define.MAP_EDITOR_OBJECT);
        }
        if (MAP_Editor.gridSceneObject != null)
        {
            MAP_Editor.ref_MapManager = MAP_Editor.gridSceneObject.GetComponent<MAP_MapManager>();
        }
        else
        {
            Debug.LogError("unable to find refrence to the map manager.");
        }
    }

    public static void getGridSceneObjectReference(string mapName)
    {
        if (MAP_Editor.gridSceneObject == null)
        {
            MAP_Editor.gridSceneObject = GameObject.Find(Define.MAP_EDITOR_OBJECT);
        }
        if (MAP_Editor.gridSceneObject != null)
        {
            MAP_Editor.ref_MapManager = MAP_Editor.gridSceneObject.GetComponent<MAP_MapManager>();
            if (MAP_Editor.ref_MapManager != null)
            {
                GameObject mainmap = new GameObject(mapName);

                if (mainmap != null)
                {
                    MAP_Editor.ref_MapManager.mapList.Add(mainmap);
                    setActiveMap();
                }
                else
                {
                    Debug.Log("Unable to find a reference to the selected map.");
                }
            }
            else
            {
                Debug.Log("Unable to find a reference to the map manager.");
            }
        }
        else
        {
            Debug.Log("Unable to find a reference to the map_MapEditorObject");
        }
    }

    public static void getGridSceneObjectReference(GameObject mapObj)
    {
        if (MAP_Editor.gridSceneObject == null)
        {
            MAP_Editor.gridSceneObject = GameObject.Find(Define.MAP_EDITOR_OBJECT);
        }
        if (MAP_Editor.gridSceneObject != null)
        {
            MAP_Editor.ref_MapManager = MAP_Editor.gridSceneObject.GetComponent<MAP_MapManager>();
            if (MAP_Editor.ref_MapManager != null)
            {
                MAP_Editor.ref_MapManager.mapList.Add(mapObj);
                setActiveMap();
            }
            else
            {
                Debug.Log("Unable to find a reference to the selected map.");
            }
        }
        else
        {
            Debug.Log("Unable to find a reference to the map manager.");
        }
    }

    public static void refreshMap()
    {

        Vector3 tempVec3;
        if (MAP_Editor.findTileMapParent())
        {
            foreach (Transform layer in MAP_Editor.tileMapParent.transform)
            {
                foreach (Transform tile in layer)
                {
                    tempVec3 = tile.localScale;
                    if (tempVec3.x != MAP_Editor.editorPreferences.gridScaleFactor)
                    {
                        if (tempVec3.x < 0)
                            tempVec3.x = MAP_Editor.editorPreferences.gridScaleFactor * -1;
                        else
                            tempVec3.x = MAP_Editor.editorPreferences.gridScaleFactor;
                    }

                    if (tempVec3.y != MAP_Editor.editorPreferences.gridScaleFactor)
                    {
                        if (tempVec3.y < 0)
                            tempVec3.y = MAP_Editor.editorPreferences.gridScaleFactor * -1;
                        else
                            tempVec3.y = MAP_Editor.editorPreferences.gridScaleFactor;
                    }

                    if (tempVec3.z != MAP_Editor.editorPreferences.gridScaleFactor)
                    {
                        if (tempVec3.z < 0)
                            tempVec3.z = MAP_Editor.editorPreferences.gridScaleFactor * -1;
                        else
                            tempVec3.z = MAP_Editor.editorPreferences.gridScaleFactor;
                    }

                    tile.localScale = tempVec3;
                }
            }


        }

    }




}
