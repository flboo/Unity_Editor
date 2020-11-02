using System.Net;
using UnityEngine;
using UnityEditor;
public class MAP_keyboardShortcuts : EditorWindow
{
    public static void checkKeyboardShortcuts(Event keyEvent)
    {
        MAP_Editor.eraseToolOverride = false;
        MAP_Editor.pickToolOverride = false;

        if (keyEvent.shift && !keyEvent.control && !keyEvent.alt)
        {
            if (MAP_Editor.selectTool == eToolIcons.brushTool)
            {
                MAP_Editor.eraseToolOverride = true;
            }
        }
        else if (keyEvent.control && !keyEvent.alt && !keyEvent.shift)
        {
            if (MAP_Editor.selectTool == eToolIcons.brushTool)
            {
                MAP_Editor.pickToolOverride = false;
            }
        }
        if (keyEvent.type == EventType.KeyDown)
        {
            switch (keyEvent.keyCode)
            {
                case KeyCode.Escape:
                    Event.current.Use();
                    MAP_Editor.selectTool = eToolIcons.defaultTools;
                    MAP_Editor.currentBrushType = eBrushTypes.standardBrush;
                    break;
                case KeyCode.Equals:
                    Event.current.Use();
                    MAP_Editor.gridHeight += MAP_Editor.globalScale;
                    break;
                case KeyCode.Minus:
                    Event.current.Use();
                    MAP_Editor.gridHeight -= MAP_Editor.globalScale;
                    break;
                case KeyCode.LeftBracket:
                    if (MAP_Editor.selectTool == eToolIcons.brushTool || MAP_Editor.selectTool == eToolIcons.eraseTool)
                    {
                        Event.current.Use();
                        Vector3 newBrushSize = MAP_Editor.brushSize;
                        newBrushSize.x -= 2;
                        newBrushSize.z -= 2;
                        MAP_Editor.brushSize = newBrushSize;
                        SceneView.RepaintAll();
                    }
                    break;
                case KeyCode.RightBracket:
                    if (MAP_Editor.selectTool == eToolIcons.brushTool || MAP_Editor.selectTool == eToolIcons.eraseTool)
                    {
                        Event.current.Use();
                        Vector3 newBrushSize = MAP_Editor.brushSize;
                        newBrushSize.x += 2;
                        newBrushSize.z += 2;
                        MAP_Editor.brushSize = newBrushSize;
                        SceneView.RepaintAll();
                    }
                    break;
                case KeyCode.LeftArrow:
                    if (MAP_Editor.selectTool == eToolIcons.brushTool || MAP_Editor.selectTool == eToolIcons.eraseTool)
                    {
                        Event.current.Use();
                        Vector3 newBrushSize = MAP_Editor.brushSize;
                        newBrushSize.x -= 2;
                        MAP_Editor.brushSize = newBrushSize;
                        SceneView.RepaintAll();
                    }
                    break;
                case KeyCode.RightArrow:
                    if (MAP_Editor.selectTool == eToolIcons.brushTool || MAP_Editor.selectTool == eToolIcons.eraseTool)
                    {
                        Event.current.Use();
                        Vector3 newBrushSize = MAP_Editor.brushSize;
                        newBrushSize.x += 2;
                        MAP_Editor.brushSize = newBrushSize;
                        SceneView.RepaintAll();
                    }
                    break;
                case KeyCode.DownArrow:
                    if (MAP_Editor.selectTool == eToolIcons.brushTool || MAP_Editor.selectTool == eToolIcons.eraseTool)
                    {
                        if (Event.current.shift)
                        {
                            Event.current.Use();
                            Vector3 newBrushSize = MAP_Editor.brushSize;
                            newBrushSize.y -= 1;
                            MAP_Editor.brushSize = newBrushSize;
                            SceneView.RepaintAll();
                        }
                        else
                        {
                            Event.current.Use();
                            Vector3 newBrushSize = MAP_Editor.brushSize;
                            newBrushSize.z -= 2;
                            MAP_Editor.brushSize = newBrushSize;
                            SceneView.RepaintAll();
                        }
                    }
                    break;
                case KeyCode.UpArrow:
                    if (MAP_Editor.selectTool == eToolIcons.brushTool || MAP_Editor.selectTool == eToolIcons.eraseTool)
                    {
                        if (Event.current.shift)
                        {
                            Event.current.Use();
                            Vector3 newBrushSize = MAP_Editor.brushSize;
                            newBrushSize.y += 1;
                            MAP_Editor.brushSize = newBrushSize;
                            SceneView.RepaintAll();
                        }
                        else
                        {
                            Event.current.Use();
                            Vector3 newBrushSize = MAP_Editor.brushSize;
                            newBrushSize.z += 2;
                            MAP_Editor.brushSize = newBrushSize;
                            SceneView.RepaintAll();
                        }
                    }
                    break;
                case KeyCode.Return:
                    if (MAP_Editor.selectTool == eToolIcons.brushTool || MAP_Editor.selectTool == eToolIcons.eraseTool)
                    {
                        Event.current.Use();
                        MAP_Editor.setTileBrush(0);
                        MAP_Editor.brushSize = Vector3.one;
                        MAP_Editor.selectTiles.Clear();
                        SceneView.RepaintAll();
                    }
                    else
                    {
                        Event.current.Use();
                        MAP_Editor.selectTiles.Clear();
                        SceneView.RepaintAll();
                    }
                    break;
                case KeyCode.Z:
                    Event.current.Use();
                    MAP_Editor.tileRotation -= 90f;
                    break;
                case KeyCode.X:
                    Event.current.Use();
                    MAP_Editor.tileRotation += 90f;
                    break;
                case KeyCode.I:
                    Event.current.Use();
                    MAP_tileFunctions.isolateTilesToggle();
                    break;
                case KeyCode.C:
                    Event.current.Use();
                    MAP_tileFunctions.flipVertical();
                    break;
                case KeyCode.V:
                    Event.current.Use();
                    MAP_tileFunctions.flipHorizontal();
                    break;
                case KeyCode.B:
                    Event.current.Use();
                    MAP_Editor.tileRotationX -= 90f;
                    break;
                case KeyCode.N:
                    Event.current.Use();
                    MAP_Editor.tileRotationX += 90f;
                    break;
                case KeyCode.Space:
                    if (MAP_Editor.selectTool == eToolIcons.selectTool || MAP_Editor.selectTool == eToolIcons.defaultTools)
                    {
                        Event.current.Use();
                        MAP_tileFunctions.selectAllTiles();
                    }
                    break;
                case KeyCode.R:
                    if (MAP_Editor.selectTool == eToolIcons.brushTool)
                    {
                        Event.current.Use();
                        MAP_Editor.randomRotationMode = !MAP_Editor.randomRotationMode;
                    }
                    break;
            }
        }
    }


}
