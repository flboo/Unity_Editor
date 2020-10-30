using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.SceneManagement;
using System.Linq;
using UnityEditor;


public class MAP_tileFunctions : EditorWindow
{
    public List<GameObject> brushTilesInParent = new List<GameObject>();
    public static void updateBrushPosition()
    {
        if (MAP_Editor.brushTile != null)
        {
            MAP_Editor.brushTile.transform.position = MAP_Editor.tilePosition;
            //    MAP_Editor.brushTile.transform.eulerAngles = new Vector3(MAP_Editor.tile );



        }
    }


    public static void cleanSceneOfBrushObjects()
    {
        // if (MAP_Editor.findTileMapParent())
        // {
        //     List<GameObject> destroyList = new List<GameObject>();

        //     foreach (Transform child in MAP_Editor.tileMapParent.transform)
        //     {
        //         if (child.gameObject.name == "YuME_brushTile")
        //         {
        //             destroyList.Add(child.gameObject);
        //         }
        //     }

        //     foreach (GameObject tileToDestory in destroyList)
        //     {
        //         DestroyImmediate(tileToDestory);
        //     }
        // }
    }

}
