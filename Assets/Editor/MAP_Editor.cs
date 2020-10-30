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
    private static eBrushOptions brushOptions = eBrushOptions.tilesetBrush;
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

    [MenuItem("windows/MAP/map_editor")]
    private static void Initialized()
    {
        MAP_Editor tileMapEditorWindow = EditorWindow.GetWindow<MAP_Editor>(false, "map editor");
        tileMapEditorWindow.titleContent.text = "map editor";
    }

    void OnEnable()
    {
        editorData = ScriptableObject.CreateInstance<MAP_EditorData>();
        AssetPreview.SetPreviewTextureCacheSize(1000);
        MAPTools_Utils.disableTileGizmo(showGizmos);
        MAPTools_Utils.addLayer("map_tile_map");

        MAP_tileFunctions.cleanSceneOfBrushObjects();
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
                editorPreferences.layerNames.Add("layer" + i);
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

#if UNITY_2019_1_OR_NEWER
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
#else
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif 
#if UNITY_2017
        EditorApplication.hierarchyWindowChanged -= OnSceneChanged;
        EditorApplication.hierarchyWindowChanged += OnSceneChanged;
#else
        EditorApplication.hierarchyChanged -= OnSceneChanged;
        EditorApplication.hierarchyChanged += OnSceneChanged;
#endif

        findUI();
        if (toolEnabled)
        {
            showUI(false);
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

    private static void setupGUI()
    {
        toolEnabled = false;
        EditorGUILayout.Space();
        GUILayout.Label(editorData.mapEditorHeader);
        EditorGUILayout.BeginVertical("box");

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
                        path = MAPTools_Utils.getAssetsPath(tempdata) + "CustomBrushes/";
                        containerPrefabs = MAPTools_Utils.getDictoryContents(MAPTools_Utils.getAssetsPath(tempdata) + "CustomBrushes/", "*.prefab");
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
            string path = MAPTools_Utils.getAssetsPath(availableTileSets[currentMapIndex]) + "CustomBrushes";
            if (path != null)
            {
                currentCustomBrushes = MAPTools_Utils.loadDirectoryContents(path, "*_MAP.prefab");
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
                if (child.name.Contains("layer"))
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
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer(Define.LAYER_MAP_TILTMAP)) == true)
        {
            Vector3 shiftOffset = gridSceneObject.transform.position;
        }

    }

    private static void checkTilePositionIsValid(Rect rect)
    {

    }

    private static void OnSceneChanged()
    {

    }

    public static void createCustomBrushFolder(string path)
    {
        Debug.Log("Directory" + path + " is missing. Creating now.");
        string newPath = path.Replace("/CustomBrushes", "");
        AssetDatabase.CreateFolder(newPath, "CustomBrushes");
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