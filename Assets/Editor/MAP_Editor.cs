using System.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class MAP_Editor : EditorWindow
{
    public static MAP_EditorData editorData;
    public static MAP_editorPreferences editorPreferences;
    public static MAP_importerSettings userSettings;
    public static GameObject editorGameobject;
    public static GameObject tileMapParent;
    public static GameObject[] mapLayers = new GameObject[8];
    public static List<MAP_tilesetData> availableTileSets = new List<MAP_tilesetData>();
    public static List<string> availableTileSetsPath = new List<string>();
    public static List<GameObject> selectTiles = new List<GameObject>();

    //new
    public static MAP_MapManager ref_MapManager;
    public static int currentMapIndex = 0;
    private static int oldCurrentMapIndex = -99;
    public static string[] mapNames;
    public static bool mouseDown = false;
    private static bool oldCustonBrushWarning;

    //wnd new
    public static string[] tileSetNames;
    public static GameObject[] currentTileSetObjects;
    private static GameObject[] currentCustomBrushes;
    private static Vector2 _scrollPosition;
    private static eBrushOptions brushPallete = eBrushOptions.tilesetBrush;
    public static GameObject gridSceneObject;

    private static Color _gridColorNormal = Color.black;
    private static Color _gridColorBorder = Color.black;
    private static Color _gridColorFill = Color.black;
    private static Vector3 _brushSize = Vector3.zero;
    private static bool setupScene = false;

    public static bool eraseToolOverride = false;
    public static bool pickToolOverride = false;

    public static int currentBrushIndex = 0;
    public static eBrushTypes currentBrushType = eBrushTypes.standardBrush;
    public static bool showWireBrush = true;
    public static bool showGizmos = false;
    public static float globalScale = 1f;
    public static float _globalScale = 1f;
    private static int gridType = 0;
    private static string meshFolder;
    private static Canvas[] uiObjects;
    private static bool uiState = true;

    public static bool randomRotationMode = false;
    public static List<GameObject> isolatedGridObjects = new List<GameObject>();
    public static bool isolataTiles = false;
    public static List<GameObject> isolataLayerObjects = new List<GameObject>();
    public static bool isolateLayer = false;

    // /  editor tools variables
    public static eToolIcons selectTool = eToolIcons.brushTool;
    public static eToolIcons previousSelectTool;
    private static float _tileRotation = 0;
    private static float _tileRotationX = 0;
    private static bool allowTileRedraw = true;
    private static string currentScene;
    private static int _currentTileSetIndex;
    private static int _currentLayer = 1;
    private static bool openConfig = false;

    //scene tool variables
    public static Vector3 tilePosition = Vector3.zero;
    public static bool validTilePosition = false;
    private static bool _toolEnabled;
    private static Vector3 oldTilePosition = Vector3.zero;
    private static float quantizesGridHeight = 0;

    //brush and currnet tile variables
    public static GameObject brushTile;
    public static GameObject currentTile;
    public static List<GameObject> tileChildObjects = new List<GameObject>();

    //alt time variables
    public static bool useAltTiles = false;
    public static List<s_AltTiles> altTiles = new List<s_AltTiles>();
    private static int controlId;

    public static Vector3 pickTileScale = Vector3.zero;
    public static Vector3 tileScale = Vector3.one;

    [MenuItem("MAP/map_editor")]
    private static void Initialized()
    {
        MAP_Editor tileMapEditorWindow = EditorWindow.GetWindow<MAP_Editor>(false, "map editor");
        tileMapEditorWindow.titleContent.text = "Map Editor";
    }

    void OnEnable()
    {
        editorData = ScriptableObject.CreateInstance<MAP_EditorData>();
        AssetPreview.SetPreviewTextureCacheSize(1000);
        MAPTools_Utils.disableTileGizmo(showGizmos);
        MAPTools_Utils.addLayer("map_tile_map");

        MAP_brushFunctions.cleanSceneOfBrushObjects();
        string[] guids;
        guids = AssetDatabase.FindAssets("map_editor_Data");
        editorData = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(MAP_EditorData)) as MAP_EditorData;

        guids = AssetDatabase.FindAssets("preference_data");
        if (guids.Length == 0)
        {
            if (!AssetDatabase.IsValidFolder(MAPTools_Utils.getAssetsPath(editorData) + "ScriptableObject"))
                AssetDatabase.CreateFolder(MAPTools_Utils.removeLastFolderSlash(MAPTools_Utils.getAssetsPath(editorData)), "ScriptableObject");
            editorPreferences = CreateInstance("MAP_editorPreferences") as MAP_editorPreferences;

            for (int i = 1; i < 9; i++)
            {
                editorPreferences.layerNames.Add(Define.LAYER + i);
                editorPreferences.layerFreeze.Add(true);
                editorPreferences.layerStatic.Add(true);
            }

            AssetDatabase.CreateAsset(editorPreferences, MAPTools_Utils.getAssetsPath(editorData) + "ScriptableObject/" + "preference_data.asset");
        }
        else
        {
            editorPreferences = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(MAP_editorPreferences)) as MAP_editorPreferences;
        }

        globalScale = editorPreferences.gridScaleFactor;

        importTileSets(false);
        loadPreviewTiles();
        loadCustomBrushes();

        _toolEnabled = toolEnabled;

        if (editorPreferences.twoPointFiveDMode == false)
            gridType = 0;
        else
            gridType = 1;

        updateGridColors();
        updateGridScale();
        updateGridType();
        gridHeight = 0;
        //createbrushtile
        MAPTools_Utils.showUnityGrid(true);
        SceneView.RepaintAll();

        //setup scene delegates
        currentScene = EditorSceneManager.GetActiveScene().name;

        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;

        EditorApplication.hierarchyChanged -= OnSceneChanged;
        EditorApplication.hierarchyChanged += OnSceneChanged;

        findUI();
        if (toolEnabled)
        {
            showUI(false);
        }

    }

    void OnSelectionChange()
    {
        if (Selection.gameObjects.Length > 0 && selectTiles.Count > 0)
        {
            selectTiles.Clear();
        }
    }

    void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            Repaint();
        }
        else if (EditorApplication.isPlayingOrWillChangePlaymode && uiState == false)
        {
            showUI(true);
        }
    }

    //draw editor tool gui
    void OnGUI()
    {
        if (!Application.isPlaying)
        {
            if (GameObject.Find(Define.MAP_EDITOR_OBJECT) == null)
            {
                setupGUI();
            }
            else
            {
                if (!checkForForzonMap())
                {
                    mainGUI();
                }
                else
                {
                    unFreezeMap();
                }
            }
        }
    }

    private static void setupGUI()
    {
        toolEnabled = false;
        EditorGUILayout.Space();
        GUILayout.Label(editorData.mapEditorHeader);
        EditorGUILayout.BeginVertical(Define.BOX);

        setupScene = GUILayout.Toggle(setupScene, "Add Map Editor Objects", "Button", GUILayout.Height(30));
        if (setupScene)
        {
            string[] guids;
            guids = AssetDatabase.FindAssets(Define.MAP_EDITOR_OBJECT);
            GameObject tiltParentPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(GameObject)) as GameObject;

            tileMapParent = PrefabUtility.InstantiatePrefab(tiltParentPrefab as GameObject) as GameObject;
            tileMapParent.transform.position = editorPreferences.initialOffset;

            tileMapParent.layer = LayerMask.NameToLayer(Define.LAYER_MAP_TILTMAP);
            Map_mapManagerFunctions.buildNewMap(Define.DATA_MAP_MAPDATA);

            EditorSceneManager.MarkAllScenesDirty();
        }
        setupScene = false;
        EditorGUILayout.EndVertical();
    }

    void mainGUI()
    {
        if (Event.current != null)
        {
            MAP_keyboardShortcuts.checkKeyboardShortcuts(Event.current);
            MAP_mouseShorcuts.checkMouseShortcuts(Event.current);
            SceneView.RepaintAll();
        }
        EditorGUILayout.Space();
        GUILayout.Label(editorData.mapEditorHeader);

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.BeginHorizontal();

        toolEnabled = GUILayout.Toggle(toolEnabled, "Enable map", "Button", GUILayout.Height(30));

        if (_toolEnabled != toolEnabled)
        {
            if (!toolEnabled)
            {
                showUI(true);
                MAPTools_Utils.showUnityGrid(true);
                MAP_tileFunctions.restoreIsolatedGridTiles();
                MAP_tileFunctions.restoreIsolatedLayerTiles();
                MAP_brushFunctions.cleanSceneOfBrushObjects();
            }
            else
            {
                showUI(false);
                setTileBrush(0);
                MAPTools_Utils.showUnityGrid(false);
            }
            SceneView.RepaintAll();
        }
        _toolEnabled = toolEnabled;
        openConfig = GUILayout.Toggle(openConfig, editorData.configButton, "Button", GUILayout.Width(30), GUILayout.Height(30));

        if (openConfig == true)
        {
            MAP_editorConfig editorConfig = EditorWindow.GetWindow<MAP_editorConfig>(true, Define.EDITOR_CONFIG);
            editorConfig.titleContent.text = Define.EDITOR_CONFIG;
        }

        openConfig = false;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        displayMapList();

        EditorGUILayout.BeginVertical(Define.BOX);
        gridDimensions = EditorGUILayout.Vector2Field(Define.DRID_DEMIONSIONS, gridDimensions);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        string[] gridLayout = new string[] { "Flat Grid", "2.5D Grid" };
        gridType = GUILayout.SelectionGrid(gridType, gridLayout, 2, EditorStyles.toolbarButton);
        editorPreferences.twoPointFiveDMode = gridType != 0;
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.BeginHorizontal();
        quantizesGridHeight = gridHeight / globalScale;
        GUILayout.Label("Grid Height: " + quantizesGridHeight.ToString());
        GUILayout.Label(string.Format("Brush Size: ({0},{1},{2})", brushSize.x, brushSize.y, brushSize.z));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("pick the tile set to use", EditorStyles.boldLabel);
        currentTileSetIndex = EditorGUILayout.Popup("choose tileset", currentTileSetIndex, tileSetNames);

        if (GUILayout.Button("reload available tilesets", GUILayout.Height(30)))
        {
            reloadTileSets();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(Define.BOX);
        string[] buttonLabels = new string[] { "Tileset Brushes", "Custom Brushes" };
        brushPallete = (eBrushOptions)GUILayout.SelectionGrid((int)brushPallete, buttonLabels, 2, EditorStyles.toolbarButton);
        EditorGUILayout.EndVertical();

        drawTilePreviews();

        EditorGUILayout.BeginVertical(Define.BOX);
        EditorGUILayout.LabelField("Tile Previre Columns", EditorStyles.boldLabel);
        tilePreviewColumnWidth = EditorGUILayout.IntSlider(tilePreviewColumnWidth, 1, 10);
        EditorGUILayout.EndVertical();

        useAltTiles = GUILayout.Toggle(useAltTiles, "Use Alt Tiles", "Button", GUILayout.Height(20));
        bool freezeMap = false;
        freezeMap = GUILayout.Toggle(freezeMap, "Freeze Map", "Button", GUILayout.Height(20));

        if (freezeMap)
        {
            MAP_freezeMap.combineTiles();
        }

        updateGridColors();
        MAP_sceneGizmoFunctions.displayGizmoGrid();
        _currentTileSetIndex = currentTileSetIndex;
    }

    private static void drawTilePreviews()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        int horzomtalCounter = 0;
        EditorGUILayout.BeginHorizontal();
        if (brushPallete == eBrushOptions.tilesetBrush)
        {
            if (currentTileSetObjects != null)
            {
                for (int i = 0; i < currentTileSetObjects.Length; i++)
                {
                    if (currentTileSetObjects[i] != null)
                    {
                        EditorGUILayout.BeginVertical();

                        drawTileButtons(i);
                        EditorGUILayout.BeginHorizontal(Define.BOX);
                        EditorGUILayout.LabelField(currentTileSetObjects[i].name, EditorStyles.boldLabel);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();

                        horzomtalCounter++;

                        if (horzomtalCounter == tilePreviewColumnWidth)
                        {
                            horzomtalCounter = 0;
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                    }
                }
            }
        }
        else if (brushPallete == eBrushOptions.customBrush)
        {
            if (currentCustomBrushes != null)
            {
                for (int i = 0; i < currentCustomBrushes.Length; i++)
                {
                    drawcustomBrushButtons(i);
                    horzomtalCounter++;
                    if (horzomtalCounter == tilePreviewColumnWidth)
                    {
                        horzomtalCounter = 0;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }

    private static void drawTileButtons(int index)
    {
        if (currentTileSetObjects[index] != null)
        {
            Texture2D previewTmage = AssetPreview.GetAssetPreview(currentTileSetObjects[index]);
            GUIContent buttonguicontent = new GUIContent(previewTmage);
            bool isActive = false;
            if (currentTileSetObjects[index] != null && currentTile != null)
            {
                if (currentTile.name == currentTileSetObjects[index].name)
                {
                    isActive = true;
                }
            }
            bool isToggleDown = GUILayout.Toggle(isActive, buttonguicontent, GUI.skin.button);
            if (isToggleDown == true && isActive == false)
            {
                setTileBrush(index);
            }
        }
    }

    private static void reloadTileSets()
    {
        importTileSets(true);
        loadCustomBrushes();
        loadPreviewTiles();
    }

    public static void importTileSets(bool fullRescan)
    {
        string[] guids = AssetDatabase.FindAssets("t:MAP_tilesetData");
        if (guids.Length > 0)
        {
            availableTileSets = new List<MAP_tilesetData>();
            foreach (var item in guids)
            {
                MAP_tilesetData tempdata = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(item), typeof(MAP_tilesetData)) as MAP_tilesetData;
                if (fullRescan)
                {
                    EditorUtility.DisplayProgressBar("Reloading Tile Set: " + tempdata.name, "Note: Reimport can take some time to complete", 0f);
                    string path = MAPTools_Utils.getAssetsPath(tempdata);
                    string[] containerPrefabs = MAPTools_Utils.getDictoryContents(MAPTools_Utils.getAssetsPath(tempdata), "*.prefab");
                    if (containerPrefabs != null)
                    {
                        foreach (var prefab in containerPrefabs)
                        {
                            AssetDatabase.ImportAsset(path + prefab);
                        }
                    }
                    if (tempdata != null)
                    {
                        path = MAPTools_Utils.getAssetsPath(tempdata) + Define.CUSTOM_BRUSHFES + "/";
                        containerPrefabs = MAPTools_Utils.getDictoryContents(MAPTools_Utils.getAssetsPath(tempdata) + Define.CUSTOM_BRUSHFES + "/", "*.prefab");
                        if (containerPrefabs != null)
                        {
                            foreach (string prefab in containerPrefabs)
                            {
                                AssetDatabase.ImportAsset(path + prefab);
                            }
                        }
                    }
                }
                availableTileSets.Add(tempdata);
            }
            if (fullRescan)
            {
                EditorUtility.ClearProgressBar();
            }
            tileSetNames = new string[availableTileSets.Count];
            for (int i = 0; i < tileSetNames.Length; i++)
            {
                tileSetNames[i] = availableTileSets[i].tileSetName;
            }
            loadPreviewTiles();
        }
        else
        {
            Debug.Log("No tile sets have been created");
        }
    }
    private static void loadPreviewTiles()
    {
        try
        {
            string path = MAPTools_Utils.getAssetsPath(availableTileSets[currentTileSetIndex]);
            currentTileSetObjects = MAPTools_Utils.loadDirectoryContents(path, "*prefab");
            altTiles = new List<s_AltTiles>();

            for (int i = 0; i < currentTileSetObjects.Length; i++)
            {
                if (AssetDatabase.IsValidFolder(path + currentTileSetObjects[i].name))
                {
                    GameObject[] loadAltTiles = MAPTools_Utils.loadDirectoryContents(path + currentTileSetObjects[i].name, "*.prefab");

                    s_AltTiles newAltTiles;
                    newAltTiles.masterTile = currentTileSetObjects[i].name;
                    newAltTiles.altTileObjects = loadAltTiles;

                    altTiles.Add(newAltTiles);
                }
            }
            currentTile = currentTileSetObjects[0];
        }
        catch
        {
            Debug.Log("Tile Sets seem to be missing. Please reload the tile sets");
        }
    }
    public static void loadCustomBrushes()
    {
        try
        {
            string path = MAPTools_Utils.getAssetsPath(availableTileSets[currentMapIndex]) + Define.CUSTOM_BRUSHFES;
            if (path != null)
            {
                currentCustomBrushes = MAPTools_Utils.loadDirectoryContents(path, "*.prefab");
                if (currentCustomBrushes == null)
                {
                    createCustomBrushFolder(path);
                }
            }
        }
        catch
        {
            Debug.Log("Custom Brush Folder missing");
        }
    }

    public static bool findEditorGameObject()
    {
        editorGameobject = GameObject.Find(Define.MAP_EDITOR_OBJECT);
        if (editorGameobject != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool findTileMapParent()
    {
        if (ref_MapManager != null)
        {
            if (ref_MapManager.mapList.Count > 0)
            {
                tileMapParent = ref_MapManager.mapList[currentMapIndex];
            }
            else
            {
                Map_mapManagerFunctions.setDefaultMap();
            }
        }
        if (tileMapParent != null)
        {
            int i = 0;
            foreach (Transform child in tileMapParent.transform)
            {
                if (child.name.Contains(Define.LAYER))
                {
                    mapLayers[i] = child.gameObject;
                    i++;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private static void updateGridColors()
    {
        if (gridSceneObject == null)
        {
            gridSceneObject = GameObject.Find(Define.MAP_EDITOR_OBJECT);
        }
        else
        {
            if (_gridColorBorder != editorPreferences.gridColorBorder || _gridColorFill != editorPreferences.gridColorFill || _gridColorNormal != editorPreferences.gridColorNormal)
            {
                gridSceneObject.GetComponent<MAP_GizmoGrid>().gridColorNormal = editorPreferences.gridColorNormal;
                gridSceneObject.GetComponent<MAP_GizmoGrid>().gridColorFill = editorPreferences.gridColorFill;
                gridSceneObject.GetComponent<MAP_GizmoGrid>().gridColorBorder = editorPreferences.gridColorBorder;
                _gridColorBorder = editorPreferences.gridColorBorder;
                _gridColorFill = editorPreferences.gridColorFill;
                _gridColorNormal = editorPreferences.gridColorNormal;
                SceneView.RepaintAll();
            }
        }
    }

    public static float gridOffset
    {
        get
        {
            return editorPreferences.gridOffset;
        }
        set
        {
            GameObject gridTemp = GameObject.Find(Define.MAP_EDITOR_OBJECT);
            editorPreferences.gridOffset = value;
            if (gridTemp != null)
            {
                gridTemp.GetComponent<MAP_GizmoGrid>().gridOffset = value;
            }
        }
    }

    public static int tilePreviewColumnWidth
    {
        get
        {
            return EditorPrefs.GetInt("tilePreviewColumnWidth", 2);
        }
        set
        {
            EditorPrefs.SetInt("tilePreviewColumnWidth", value);
        }
    }

    private static void updateGridScale()
    {
        if (gridSceneObject == null)
        {
            gridSceneObject = GameObject.Find(Define.MAP_EDITOR_OBJECT);
        }
        else
        {
            try
            {
                gridSceneObject.GetComponent<MAP_GizmoGrid>().tileSize = globalScale;
                gridSceneObject.GetComponent<MAP_GizmoGrid>().centreGrid = editorPreferences.centreGrid;
            }
            catch
            {
                gridSceneObject.GetComponent<MAP_GizmoGrid>().tileSize = 1;
                gridSceneObject.GetComponent<MAP_GizmoGrid>().centreGrid = true;
            }
        }
        _globalScale = globalScale;
    }

    private static void updateGridType()
    {
        if (gridSceneObject == null)
        {
            gridSceneObject = GameObject.Find(Define.MAP_EDITOR_OBJECT);
        }
        else
        {
            gridSceneObject.GetComponent<MAP_GizmoGrid>().twoPointFiveDMode = editorPreferences.twoPointFiveDMode;
            SceneView.RepaintAll();
        }
    }
    private static void OnSceneGUI(SceneView sceneView)
    {
        if (toolEnabled)
        {
            //editor scenes  editor tool
            if (selectTool > 0)
            {
                controlId = GUIUtility.GetControlID(FocusType.Passive);
                updateSceneMousePosition();
                checkTilePositionIsValid(sceneView.position);
                MAP_sceneGizmoFunctions.drawBrushGizmo();
            }

            //绘制编辑器进度条
            MAP_editorSceneUI.drawToolUI(sceneView);

            MAP_keyboardShortcuts.checkKeyboardShortcuts(Event.current);
            MAP_mouseShorcuts.checkMouseShortcuts(Event.current);

            foreach (GameObject selected in selectTiles)
            {
                MAP_sceneGizmoFunctions.drawSceneGizmoCube(selected.transform.position, Vector3.one, Color.green);
            }

            switch (selectTool)
            {
                case eToolIcons.defaultTools:
                    MAP_brushFunctions.destoryBrushTile();
                    break;
                case eToolIcons.brushTool:
                    MAP_brushFunctions.createBrushTile();
                    selectTiles.Clear();
                    break;
                case eToolIcons.pickTool:
                    MAP_brushFunctions.destoryBrushTile();
                    selectTiles.Clear();
                    break;
                case eToolIcons.eraseTool:
                    MAP_brushFunctions.destoryBrushTile();
                    selectTiles.Clear();
                    break;
                case eToolIcons.selectTool:
                    MAP_brushFunctions.destoryBrushTile();
                    break;
                case eToolIcons.copyTool:
                    MAP_customBrushFunctions.createCopyBrush(false);
                    selectTool = eToolIcons.brushTool;
                    break;
                case eToolIcons.moveTool:
                    MAP_customBrushFunctions.createCopyBrush(true);
                    selectTool = eToolIcons.brushTool;
                    break;
                case eToolIcons.trashTool:
                    MAP_tileFunctions.trashTiles();
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.customBrushTool:
                    selectTool = previousSelectTool;
                    MAP_customBrushFunctions.createCustomBrush();
                    break;
                case eToolIcons.showGizmos:
                    showGizmos = !showGizmos;
                    MAPTools_Utils.disableTileGizmo(showGizmos);
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.gridUpTool:
                    if (Event.current.alt)
                    {
                        gridHeight += (globalScale * 0.25f);
                    }
                    else
                    {
                        gridHeight += globalScale * editorPreferences.gridLayerHeightScaler;
                    }
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.gridDownTool:
                    if (Event.current.alt)
                    {
                        gridHeight -= (globalScale * 0.25f);
                    }
                    else
                    {
                        gridHeight -= globalScale * editorPreferences.gridLayerHeightScaler;
                    }
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.rotateTool:
                    tileRotation += 90f;
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.rotateXTool:
                    tileRotationX += 90f;
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.flipHorizontalTool:
                    MAP_tileFunctions.flipHorizontal();
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.flipVerticalTool:
                    MAP_tileFunctions.flipVertical();
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.isolateTool:
                    MAP_tileFunctions.isolateTilesToggle();
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.isolateLayerTool:
                    MAP_tileFunctions.isolateLayerToggle();
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.layerUp:
                    currentLayer++;
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.layerDown:
                    currentLayer--;
                    selectTool = previousSelectTool;
                    break;
                case eToolIcons.refreshMap:
                    Map_mapManagerFunctions.refreshMap();
                    selectTool = previousSelectTool;
                    break;
            }

            //check scebe view input for drawing  pixking etc

            if (selectTool > eToolIcons.defaultTools)
            {
                if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) &&
                    Event.current.button == 0 &&
                    Event.current.alt == false &&
                    Event.current.shift == false &&
                    Event.current.control == false &&
                    allowTileRedraw)
                {
                    switch (selectTool)
                    {
                        case eToolIcons.brushTool:
                            switch (currentBrushType)
                            {
                                case eBrushTypes.standardBrush:
                                    addEraseTiles(false);
                                    break;
                                case eBrushTypes.customBrush:
                                    if (currentTile.GetComponent<MAP_tileGizmo>())
                                    {
                                        addEraseTiles(false); // Custom brushes now work the same as normal tiles
                                    }
                                    else
                                    {
                                        if (!oldCustonBrushWarning)
                                        {
                                            oldCustonBrushWarning = true;
                                            Debug.LogWarning("Please note: How custom brushes are created has been updated in YuME 1.1.2 and above. This is an old brush. We recommend recreating this using the new custom brush system for stability.");
                                            Debug.LogWarning("To increase stability, UNDO has been disabled on OLD custom brushes. New custom brushes use the same system as normal tiles and can be un-done.");
                                        }

                                        MAP_customBrushFunctions.pasteCustomBrush(tilePosition); // for legacy custom brushes
                                    }

                                    break;
                                case eBrushTypes.copyBrush:
                                    MAP_customBrushFunctions.pasteCopyBrush(tilePosition);
                                    break;
                            }
                            break;
                        case eToolIcons.pickTool:
                            MAP_tileFunctions.pickTile(tilePosition);
                            break;
                        case eToolIcons.eraseTool:
                            addEraseTiles(true);
                            break;
                        case eToolIcons.selectTool:
                            MAP_tileFunctions.selectTile(tilePosition);
                            break;
                    }

                    allowTileRedraw = false;
                }
                else if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) &&
                    Event.current.button == 0 &&
                    Event.current.alt == false &&
                    Event.current.shift == true &&
                    Event.current.control == false &&
                    allowTileRedraw)
                {
                    switch (selectTool)
                    {
                        case eToolIcons
                        .brushTool:
                            addEraseTiles(true);
                            break;
                    }

                    allowTileRedraw = false;
                }
                else if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) &&
                    Event.current.button == 0 &&
                    Event.current.alt == false &&
                    Event.current.shift == false &&
                    Event.current.control == true &&
                    allowTileRedraw)
                {
                    switch (selectTool)
                    {
                        case eToolIcons.brushTool:
                            MAP_tileFunctions.pickTile(tilePosition);
                            break;
                        case eToolIcons.selectTool:
                            MAP_tileFunctions.delSelectTile(tilePosition);
                            break;
                    }

                    allowTileRedraw = false;
                }
                HandleUtility.AddDefaultControl(controlId);
            }
        }
    }

    private static void updateSceneMousePosition()
    {
        if (Event.current == null)
        {
            return;
        }

        Vector2 mousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);

        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 1 << LayerMask.NameToLayer(Define.LAYER_MAP_TILTMAP)))
        {
            Vector3 shitOffset = gridSceneObject.transform.position;
            shitOffset.x = shitOffset.x - (int)shitOffset.x;
            shitOffset.y = shitOffset.y - (int)shitOffset.y;
            shitOffset.z = shitOffset.z - (int)shitOffset.z;
            if (!editorPreferences.twoPointFiveDMode)
            {
                tilePosition.x = Mathf.Round(((hitInfo.point.x + shitOffset.x) - hitInfo.normal.x * 0.001f) / globalScale) * globalScale - shitOffset.x;
                tilePosition.z = Mathf.Round(((hitInfo.point.z + shitOffset.z) - hitInfo.normal.z * 0.001f) / globalScale) * globalScale - shitOffset.z;
                tilePosition.y = gridHeight + gridSceneObject.transform.position.y;
            }
            else
            {
                tilePosition.x = Mathf.Round(((hitInfo.point.x + shitOffset.x) - hitInfo.normal.x * 0.001f) / globalScale) * globalScale - shitOffset.x;
                tilePosition.y = Mathf.Round(((hitInfo.point.y + shitOffset.y) - hitInfo.normal.y * 0.001f) / globalScale) * globalScale - shitOffset.y;
                tilePosition.z = gridHeight + gridSceneObject.transform.position.z;
            }
        }
    }

    private static void checkTilePositionIsValid(Rect rect)
    {
        bool isValidArea = Event.current.mousePosition.y < rect.height - 35;
        if (isValidArea != validTilePosition)
        {
            validTilePosition = isValidArea;
            SceneView.RepaintAll();
        }
    }

    private static void OnSceneChanged()
    {
        if (currentScene != EditorSceneManager.GetActiveScene().name)
        {
            toolEnabled = false;
            MAPTools_Utils.showUnityGrid(true);
            currentScene = EditorSceneManager.GetActiveScene().name;
        }
        MAP_sceneGizmoFunctions.displayGizmoGrid();
    }

    public static void createCustomBrushFolder(string path)
    {
        Debug.Log("Directory" + path + " is missing. Creating now.");
        string newPath = path.Replace("/CustomBrushes", "");
        AssetDatabase.CreateFolder(newPath, Define.CUSTOM_BRUSHFES);
    }


    static void findUI()
    {
        uiObjects = Resources.FindObjectsOfTypeAll<Canvas>();
        showUI(true);
    }

    static void showUI(bool showState)
    {
        if (editorPreferences.hideUIObjects)
        {
            uiState = showState;

            for (int i = 0; i < uiObjects.Length; i++)
            {
                if (uiObjects[i].gameObject == null)
                {
                    findUI();
                    break;
                }
                uiObjects[i].gameObject.SetActive(uiState);
            }
        }
    }

    public static bool toolEnabled
    {
        get
        {
            return EditorPrefs.GetBool("toolEnabled", true);
        }
        set
        {
            EditorPrefs.SetBool("toolEnabled", value);
        }
    }

    public static int currentTileSetIndex
    {
        get
        {
            return EditorPrefs.GetInt("currentTileSetIndex", 0);
        }
        set
        {
            EditorPrefs.SetInt("currentTileSetIndex", value);
        }
    }

    public static float tileRotation
    {
        get
        {
            return _tileRotation;
        }
        set
        {
            _tileRotation = value;

            if (_tileRotation >= 360)
            {
                _tileRotation = 0f;
            }
            else if (_tileRotation < 0f)
            {
                _tileRotation = 270f;
            }

        }
    }

    public static float tileRotationX
    {
        get
        {
            return _tileRotationX;
        }
        set
        {
            _tileRotationX = value;

            if (_tileRotationX >= 360)
            {
                _tileRotationX = 0f;
            }
            else if (_tileRotationX < 0f)
            {
                _tileRotationX = 270f;
            }

        }
    }


    public static float gridHeight
    {
        get
        {
            GameObject tempGrid = GameObject.Find(Define.MAP_EDITOR_OBJECT);
            if (tempGrid != null)
            {
                return tempGrid.GetComponent<MAP_GizmoGrid>().gridHeight;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            GameObject tempGrid = GameObject.Find(Define.MAP_EDITOR_OBJECT);
            if (tempGrid != null)
            {
                tempGrid.GetComponent<MAP_GizmoGrid>().gridHeight = value;
                tempGrid.GetComponent<MAP_GizmoGrid>().moveGrid();
            }
        }
    }

    public static Vector2 gridDimensions
    {
        get
        {
            return editorPreferences.gridDimensions;
        }
        set
        {
            editorPreferences.gridDimensions = value;
            GameObject gridTemp = GameObject.Find(Define.MAP_EDITOR_OBJECT);
            if (gridTemp != null)
            {
                gridTemp.GetComponent<MAP_GizmoGrid>().gridWidth = (int)value.x;
                gridTemp.GetComponent<MAP_GizmoGrid>().gridDepth = (int)value.y;
                Vector3 temGridSize;
                if (!editorPreferences.twoPointFiveDMode)
                {
                    temGridSize.x = (int)value.x * globalScale;
                    temGridSize.y = 0.1f;
                    temGridSize.z = (int)value.y * globalScale;
                }
                else
                {
                    temGridSize.x = (int)value.x * globalScale;
                    temGridSize.y = (int)value.y * globalScale;
                    temGridSize.z = 0.1f;
                }
                gridTemp.GetComponent<BoxCollider>().size = temGridSize;
            }
            EditorUtility.SetDirty(editorData);
        }
    }

    public static bool standardBrushSize
    {
        get
        {
            if (brushSize.x == 1f && brushSize.y == 1f && brushSize.z == 1f)
                return true;
            else
                return false;
        }
    }

    private static bool checkForForzonMap()
    {
        if (findTileMapParent())
        {
            foreach (Transform child in tileMapParent.transform)
            {
                if (child.gameObject.name == Define.FROZEN_MAP)
                {
                    toolEnabled = false;
                    MAPTools_Utils.showUnityGrid(true);
                    return true;
                }
            }
        }
        return false;
    }

    private void unFreezeMap()
    {
        toolEnabled = false;
        EditorGUILayout.Space();

        GUILayout.Label(editorData.mapEditorHeader);
        displayMapList();
        if (GUILayout.Button("save frozen object", GUILayout.Height(30)))
        {
            string meshFolder = EditorUtility.OpenFolderPanel("frozen map destanation folder", "", "");
            if (string.IsNullOrEmpty(meshFolder))
            {
                meshFolder = "Assets/";
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            MAP_freezeMap.saveFrozenMesh(meshFolder);
        }
        EditorGUILayout.BeginVertical(Define.BOX);
        if (GUILayout.Button("unfreeze map", GUILayout.Height(30)))
        {
            if (findTileMapParent())
            {
                foreach (Transform item in tileMapParent.transform)
                {
                    if (item.gameObject.name == Define.LAYER)
                    {
                        item.gameObject.SetActive(true);
                    }
                    else if (item.gameObject.name == Define.FROZEN_MAP)
                    {
                        DestroyImmediate(item.gameObject);
                    }
                }
            }
            MAP_brushFunctions.destoryBrushTile();
            currentBrushType = eBrushTypes.standardBrush;
            setTileBrush(0);
            showUI(false);
            toolEnabled = false;
        }
        if (!uiState)
        {
            showUI(true);
        }
        MAP_sceneGizmoFunctions.displayGizmoGrid();
        EditorGUILayout.EndVertical();
    }

    public static void cloneMap(GameObject sourcemap)
    {
        GameObject mainmap = Map_mapManagerFunctions.buildNewMap(sourcemap.name + " (clone)");
        Transform[] cloneLayers = mainmap.GetComponentsInChildren<Transform>();
        int cloneLayerIndex = 0;
        foreach (Transform layers in sourcemap.transform)
        {
            foreach (Transform tiles in layers)
            {
                GameObject clone = PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(tiles.gameObject)) as GameObject;
                if (clone != null)
                {
                    clone.transform.position = tiles.position;
                    clone.transform.eulerAngles = tiles.eulerAngles;
                    clone.transform.localScale = tiles.localScale;
                    clone.transform.SetParent(cloneLayers[cloneLayerIndex]);
                }
            }
            cloneLayerIndex++;
        }
    }

    private static void displayMapList()
    {
        if (ref_MapManager == null)
        {
            Map_mapManagerFunctions.getGridSceneObjectReference();
        }
        mapNames = new string[ref_MapManager.mapList.Count];
        if (mapNames.Length > 0)
        {
            for (int i = 0; i < ref_MapManager.mapList.Count; i++)
            {
                if (ref_MapManager.mapList[i] != null)
                {
                    mapNames[i] = ref_MapManager.mapList[i].name;
                }
            }
            EditorGUILayout.BeginVertical(Define.BOX);
            EditorGUILayout.BeginHorizontal();
            currentMapIndex = EditorGUILayout.Popup(currentMapIndex, mapNames);

            if (oldCurrentMapIndex != currentMapIndex)
            {
                Map_mapManagerFunctions.setActiveMap();
            }
            oldCurrentMapIndex = currentMapIndex;
            openConfig = GUILayout.Toggle(openConfig, "+", "Button", GUILayout.Width(30), GUILayout.Height(15));
            if (openConfig == true)
            {
                EditorWindow.GetWindow<MAP_MapManagerUI>(true, "Map Manager");
            }
            openConfig = false;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        else
        {
            Map_mapManagerFunctions.setDefaultMap();
        }
    }


    public static Vector3 brushSize
    {
        get { return _brushSize; }
        set
        {
            _brushSize = value;
            if (_brushSize.x < 1)
                _brushSize.x = 1;
            if (_brushSize.y < 1)
                _brushSize.y = 1;
            if (_brushSize.z < 1)
                _brushSize.z = 1;
        }
    }

    public static int currentLayer
    {
        get
        {
            return _currentLayer;
        }
        set
        {
            _currentLayer = value;
            if (_currentLayer > 8)
            {
                _currentLayer = 8;
            }
            else if (_currentLayer < 1)
            {
                _currentLayer = 1;
            }
        }
    }

    public static void setTileBrush(int index)
    {

        if (currentTileSetObjects[index] != null)
        {
            currentBrushIndex = index;
            currentBrushType = eBrushTypes.standardBrush;
            currentTile = currentTileSetObjects[index];
            tileRotation = 0;
            tileRotationX = 0;
            MAP_brushFunctions.updateBrushTile();
            selectTool = eToolIcons.brushTool;
        }

    }

    private static void drawcustomBrushButtons(int index)
    {
        if (currentCustomBrushes[index] != null)
        {
            Texture2D previewImage = AssetPreview.GetAssetPreview(currentCustomBrushes[index]);
            GUIContent buttonContent = new GUIContent(previewImage);

            bool isActive = false;
            if (currentTile != null && currentTile.name == currentCustomBrushes[index].name)
            {
                isActive = true;
            }
            EditorGUILayout.BeginVertical();
            bool isToggleDown = GUILayout.Toggle(isActive, buttonContent, GUI.skin.button);
            if (isToggleDown == true && isActive == false)
            {
                currentTile = currentCustomBrushes[index];
                currentBrushType = eBrushTypes.customBrush;
                _tileRotation = 0f;
                tileRotationX = 0f;
                MAP_brushFunctions.updateBrushTile();
                selectTool = eToolIcons.brushTool;
            }

            if (GUILayout.Button(Define.DELETE_BRUSH))
            {
                if (EditorUtility.DisplayDialog("delete custom brush?", "are you sure want to delete the custom brush from the project", "Delete", "No"))
                {
                    string destationPath = availableTileSets[currentBrushIndex].customBrushDestinationFolder + "/";
                    if (currentCustomBrushes[index].GetComponent<MAP_tileGizmo>())
                    {
                        List<string> meshsTileDelete = currentCustomBrushes[index].GetComponent<MAP_tileGizmo>().customBrushMeshName;
                        foreach (string item in meshsTileDelete)
                        {
                            AssetDatabase.DeleteAsset(destationPath + item + ".asset");
                        }
                    }
                    AssetDatabase.DeleteAsset(destationPath + currentCustomBrushes[index].name + ".prefab");
                    loadCustomBrushes();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }

    public static void addEraseTiles(bool eraseOnly)
    {
        if (standardBrushSize)
        {
            MAP_tileFunctions.eraseTile(tilePosition);
            if (!eraseOnly)
                MAP_tileFunctions.addTile(tilePosition);
        }
        else
        {
            Vector3 newTilePos = tilePosition;
            for (int y = 0; y < brushSize.y; y++)
            {
                newTilePos.y = tilePosition.y + (globalScale * y);
                newTilePos.z = tilePosition.z - ((brushSize.z - 1) * globalScale) * 0.5f;

                for (int z = 0; z < (int)brushSize.z; z++)
                {
                    newTilePos.x = tilePosition.x - ((brushSize.x - 1) * globalScale) * 0.5f;
                    for (int i = 0; i < (int)brushSize.x; i++)
                    {
                        MAP_tileFunctions.eraseTile(newTilePos);
                        if (!eraseOnly)
                            MAP_tileFunctions.addTile(newTilePos);
                        newTilePos.x += globalScale;
                    }
                    newTilePos.z += globalScale;
                }
            }
        }
    }

    private static void repaintSceneView()
    {
        if (tilePosition != oldTilePosition)
        {
            SceneView.RepaintAll();
            allowTileRedraw = true;
            oldTilePosition = tilePosition;
        }
    }
}
public struct s_AltTiles
{
    public string masterTile;
    public GameObject[] altTileObjects;
}

public enum eToolIcons
{
    defaultTools,
    brushTool,
    pickTool,
    eraseTool,
    selectTool,
    showGizmos,
    isolateTool,
    gridUpTool,
    gridDownTool,
    rotateTool,
    rotateXTool,
    flipVerticalTool,
    flipHorizontalTool,
    refreshMap,
    copyTool,
    moveTool,
    customBrushTool,
    trashTool,
    isolateLayerTool,
    layerUp,
    layerDown,
    none,
}

public enum eBrushOptions
{
    tilesetBrush,
    customBrush,
}
public enum eBrushTypes
{
    standardBrush,
    customBrush,
    copyBrush,

}