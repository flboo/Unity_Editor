using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MAP_sceneGizmoFunctions : EditorWindow
{

    public static void drawBrushGizmo()
    {

        if (MAP_Editor.validTilePosition == false)
        {
            return;
        }
        if (MAP_Editor.pickToolOverride || MAP_Editor.selectTool == eToolIcons.pickTool)
        {
            drawSceneGizmoCube(MAP_Editor.tilePosition, Vector3.one, MAP_Editor.editorPreferences.pickCursorColor);
        }
        else if (MAP_Editor.eraseToolOverride || MAP_Editor.selectTool == eToolIcons.eraseTool)
        {
            drawSceneGizmoCube(MAP_Editor.tilePosition, MAP_Editor.brushSize, MAP_Editor.editorPreferences.eraseCursorColor);
        }
        else
        {
            drawSceneGizmoCube(MAP_Editor.tilePosition, MAP_Editor.brushSize, MAP_Editor.editorPreferences.brushCursorColor);
        }
    }

    public static void drawSceneGizmoCube(Vector3 position, Vector3 brushSize, Color gizmoColor)
    {
        Handles.color = gizmoColor;
        var full = brushSize * MAP_Editor.globalScale;
        var half = full * 0.5f;
        var scale = MAP_Editor.globalScale * 0.5f;//

        // draw front
        Handles.DrawLine(position + new Vector3(-half.x, -scale, half.z), position + new Vector3(half.x, -scale, half.z));
        Handles.DrawLine(position + new Vector3(-half.x, -scale, half.z), position + new Vector3(-half.x, full.y - scale, half.z));
        Handles.DrawLine(position + new Vector3(half.x, full.y - scale, half.z), position + new Vector3(half.x, -scale, half.z));
        Handles.DrawLine(position + new Vector3(half.x, full.y - scale, half.z), position + new Vector3(-half.x, full.y - scale, half.z));

        // draw back
        Handles.DrawLine(position + new Vector3(-half.x, -scale, -half.z), position + new Vector3(half.x, -scale, -half.z));
        Handles.DrawLine(position + new Vector3(-half.x, -scale, -half.z), position + new Vector3(-half.x, full.y - scale, -half.z));
        Handles.DrawLine(position + new Vector3(half.x, full.y - scale, -half.z), position + new Vector3(half.x, -scale, -half.z));
        Handles.DrawLine(position + new Vector3(half.x, full.y - scale, -half.z), position + new Vector3(-half.x, full.y - scale, -half.z));

        // draw corners
        Handles.DrawLine(position + new Vector3(-half.x, -scale, -half.z), position + new Vector3(-half.x, -scale, half.z));
        Handles.DrawLine(position + new Vector3(half.x, -scale, -half.z), position + new Vector3(half.x, -scale, half.z));
        Handles.DrawLine(position + new Vector3(-half.x, full.y - scale, -half.z), position + new Vector3(-half.x, full.y - scale, half.z));
        Handles.DrawLine(position + new Vector3(half.x, full.y - scale, -half.z), position + new Vector3(half.x, full.y - scale, half.z));

        /*
        // draw front

        
        Handles.DrawLine(position + new Vector3(-half.x, -0.5f, half.z), position + new Vector3(half.x, -0.5f, half.z));
        Handles.DrawLine(position + new Vector3(-half.x, -0.5f, half.z), position + new Vector3(-half.x, brushSize.y - 0.5f, half.z));
        Handles.DrawLine(position + new Vector3(half.x, brushSize.y - 0.5f, half.z), position + new Vector3(half.x, -0.5f, half.z));
        Handles.DrawLine(position + new Vector3(half.x, brushSize.y - 0.5f, half.z), position + new Vector3(-half.x, brushSize.y - 0.5f, half.z));
        // draw back
        Handles.DrawLine(position + new Vector3(-half.x, -0.5f, -half.z), position + new Vector3(half.x, -0.5f, -half.z));
        Handles.DrawLine(position + new Vector3(-half.x, -0.5f, -half.z), position + new Vector3(-half.x, brushSize.y - 0.5f, -half.z));
        Handles.DrawLine(position + new Vector3(half.x, brushSize.y - 0.5f, -half.z), position + new Vector3(half.x, -0.5f, -half.z));
        Handles.DrawLine(position + new Vector3(half.x, brushSize.y - 0.5f, -half.z), position + new Vector3(-half.x, brushSize.y - 0.5f, -half.z));
        // draw corners
        Handles.DrawLine(position + new Vector3(-half.x, -0.5f, -half.z), position + new Vector3(-half.x, -0.5f, half.z));
        Handles.DrawLine(position + new Vector3(half.x, -0.5f, -half.z), position + new Vector3(half.x, -0.5f, half.z));
        Handles.DrawLine(position + new Vector3(-half.x, brushSize.y - 0.5f, -half.z), position + new Vector3(-half.x, brushSize.y - 0.5f, half.z));
        Handles.DrawLine(position + new Vector3(half.x, brushSize.y - 0.5f, -half.z), position + new Vector3(half.x, brushSize.y - 0.5f, half.z));
        */
    }

    public static void displayGizmoGrid()
    {
        if (MAP_Editor.findEditorGameObject())
        {
            MAP_Editor.editorGameobject.GetComponent<MAP_GizmoGrid>().gridOffset = MAP_Editor.editorPreferences.gridOffset;
            MAP_Editor.editorGameobject.GetComponent<MAP_GizmoGrid>().toolEnable = MAP_Editor.toolEnabled;
        }
    }

    public static void drawTileInfo(Vector3 position, handleInfo info)
    {
        Handles.color = Color.white;
        Handles.Label(position, info.tileName);
        position.y -= 0.15f;
        Handles.Label(position, "Grid Height: " + info.grid);
        position.y -= 0.15f;
        Handles.Label(position, "Layer: " + info.layer);
    }

}
public struct handleInfo
{
    public string tileName;
    public float grid;
    public string layer;
}