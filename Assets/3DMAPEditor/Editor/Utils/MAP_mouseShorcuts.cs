using UnityEditor;
using UnityEngine;

public class MAP_mouseShorcuts : EditorWindow
{
    public static void checkMouseShortcuts(Event mouseEvent)
    {
        if (mouseEvent.type == EventType.ScrollWheel && mouseEvent.shift && !mouseEvent.control && !mouseEvent.alt)
        {
            mouseEvent.Use();
            if (!MAP_Editor.editorPreferences.invertMouseWheel)
            {
                if (!MAP_Editor.editorPreferences.twoPointFiveDMode)
                {
                    if (Event.current.delta.y - Event.current.delta.x >= 0f)
                        MAP_Editor.gridHeight +=
                            MAP_Editor.globalScale * MAP_Editor.editorPreferences.gridLayerHeightScaler;
                    else
                        MAP_Editor.gridHeight -=
                            MAP_Editor.globalScale * MAP_Editor.editorPreferences.gridLayerHeightScaler;
                }
                else
                {
                    if (Event.current.delta.y - Event.current.delta.x >= 0f)
                        MAP_Editor.gridHeight -=
                            MAP_Editor.globalScale * MAP_Editor.editorPreferences.gridLayerHeightScaler;
                    else
                        MAP_Editor.gridHeight +=
                            MAP_Editor.globalScale * MAP_Editor.editorPreferences.gridLayerHeightScaler;
                }
            }
            else
            {
                if (!MAP_Editor.editorPreferences.twoPointFiveDMode)
                {
                    if (Event.current.delta.y - Event.current.delta.x >= 0f)
                        MAP_Editor.gridHeight -=
                            MAP_Editor.globalScale * MAP_Editor.editorPreferences.gridLayerHeightScaler;
                    else
                        MAP_Editor.gridHeight +=
                            MAP_Editor.globalScale * MAP_Editor.editorPreferences.gridLayerHeightScaler;
                }
                else
                {
                    if (Event.current.delta.y - Event.current.delta.x >= 0f)
                        MAP_Editor.gridHeight +=
                            MAP_Editor.globalScale * MAP_Editor.editorPreferences.gridLayerHeightScaler;
                    else
                        MAP_Editor.gridHeight -=
                            MAP_Editor.globalScale * MAP_Editor.editorPreferences.gridLayerHeightScaler;
                }
            }
        }

        if (mouseEvent.type == EventType.ScrollWheel && mouseEvent.shift && mouseEvent.alt && !mouseEvent.control)
        {
            mouseEvent.Use();

            if (!MAP_Editor.editorPreferences.invertMouseWheel)
            {
                if (Event.current.delta.y - Event.current.delta.x >= 0f)
                    MAP_Editor.gridHeight += MAP_Editor.globalScale * 0.25f;
                else
                    MAP_Editor.gridHeight -= MAP_Editor.globalScale * 0.25f;
            }
            else
            {
                if (Event.current.delta.y - Event.current.delta.x >= 0f)
                    MAP_Editor.gridHeight -= MAP_Editor.globalScale * 0.25f;
                else
                    MAP_Editor.gridHeight += MAP_Editor.globalScale * 0.25f;
            }
        }
        else if (mouseEvent.type == EventType.ScrollWheel && mouseEvent.control && mouseEvent.alt &&
                 MAP_Editor.selectTool == eToolIcons.brushTool)
        {
            mouseEvent.Use();

            if (Event.current.delta.y - Event.current.delta.x >= 0f)
                MAP_Editor.tileRotation += 90f;
            else
                MAP_Editor.tileRotation -= 90f;
        }
        else if (mouseEvent.type == EventType.ScrollWheel && mouseEvent.control && mouseEvent.shift &&
                 MAP_Editor.selectTool == eToolIcons.brushTool)
        {
            mouseEvent.Use();

            MAP_Editor.currentBrushType = eBrushTypes.standardBrush;

            if (!MAP_Editor.editorPreferences.invertMouseWheel)
            {
                if (Event.current.delta.y - Event.current.delta.x > 0f)
                {
                    MAP_Editor.currentBrushIndex++;

                    if (MAP_Editor.currentBrushIndex >= MAP_Editor.currentTileSetObjects.Length)
                        MAP_Editor.currentBrushIndex = MAP_Editor.currentTileSetObjects.Length - 1;

                    MAP_Editor.currentTile = MAP_Editor.currentTileSetObjects[MAP_Editor.currentBrushIndex];
                    MAP_Editor.currentTile.transform.eulerAngles =
                        new Vector3(MAP_Editor.tileRotationX, MAP_Editor.tileRotation, 0f);
                    MAP_brushFunctions.updateBrushTile();
                }
                else
                {
                    MAP_Editor.currentBrushIndex--;

                    if (MAP_Editor.currentBrushIndex < 0) MAP_Editor.currentBrushIndex = 0;

                    MAP_Editor.currentTile = MAP_Editor.currentTileSetObjects[MAP_Editor.currentBrushIndex];
                    MAP_Editor.currentTile.transform.eulerAngles =
                        new Vector3(MAP_Editor.tileRotationX, MAP_Editor.tileRotation, 0f);
                    MAP_brushFunctions.updateBrushTile();
                }
            }
            else
            {
                if (Event.current.delta.y - Event.current.delta.x < 0f)
                {
                    MAP_Editor.currentBrushIndex++;

                    if (MAP_Editor.currentBrushIndex >= MAP_Editor.currentTileSetObjects.Length)
                        MAP_Editor.currentBrushIndex = MAP_Editor.currentTileSetObjects.Length - 1;

                    MAP_Editor.currentTile = MAP_Editor.currentTileSetObjects[MAP_Editor.currentBrushIndex];
                    MAP_Editor.currentTile.transform.eulerAngles =
                        new Vector3(MAP_Editor.tileRotationX, MAP_Editor.tileRotation, 0f);
                    MAP_brushFunctions.updateBrushTile();
                }
                else
                {
                    MAP_Editor.currentBrushIndex--;

                    if (MAP_Editor.currentBrushIndex < 0) MAP_Editor.currentBrushIndex = 0;

                    MAP_Editor.currentTile = MAP_Editor.currentTileSetObjects[MAP_Editor.currentBrushIndex];
                    MAP_Editor.currentTile.transform.eulerAngles =
                        new Vector3(MAP_Editor.tileRotationX, MAP_Editor.tileRotation, 0f);
                    MAP_brushFunctions.updateBrushTile();
                }
            }
        }
    }
}