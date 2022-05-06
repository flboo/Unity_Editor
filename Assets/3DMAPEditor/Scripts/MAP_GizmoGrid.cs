using UnityEngine;

public class MAP_GizmoGrid : MonoBehaviour
{
    [HideInInspector] public float tileSize = 1;

    public int gridWidth = 40;
    public int gridDepth = 40;

    [HideInInspector] public bool twoPointFiveDMode;

    [HideInInspector] public bool centreGrid = true;

    public float gridOffset = 0.01f;

    [HideInInspector] public Color gridColorNormal = Color.white;

    [HideInInspector] public Color gridColorBorder = Color.green;

    [HideInInspector] public Color gridColorFill = new(1, 0, 0, 0.5f);

    private Vector3 gridColliderPosition;
    private float gridDepthOffset;
    private Vector3 gridMax;
    private Vector3 gridMin;
    private float gridWidthOffset;
    private float tileOffset = 0.5f;

    [HideInInspector] public float gridHeight { get; set; }

    [HideInInspector] public bool toolEnable { get; set; } = true;

    private void OnEnable()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (toolEnable)
        {
            if (twoPointFiveDMode)
            {
                tileOffset = tileSize / 2;
                if (centreGrid)
                {
                    gridWidthOffset = gridWidth * tileSize / 2;
                    gridDepthOffset = gridDepth * tileSize / 2;
                }
                else
                {
                    gridWidthOffset = 0;
                    gridDepthOffset = 0;
                }

                gridMin.x = gameObject.transform.position.x - gridWidthOffset - tileOffset;
                gridMin.z = gameObject.transform.position.z + gridHeight - gridOffset + tileOffset;
                gridMin.y = gameObject.transform.position.y - gridDepthOffset - tileOffset;
                gridMax.x = gridMin.x + tileSize * gridWidth;
                gridMax.y = gridMin.y + tileSize * gridDepth;
                gridMax.z = gridMin.z;
            }
            else
            {
                tileOffset = tileSize / 2;
                if (centreGrid)
                {
                    gridWidthOffset = gridWidth * tileSize / 2;
                    gridDepthOffset = gridDepth * tileSize / 2;
                }
                else
                {
                    gridWidthOffset = 0;
                    gridDepthOffset = 0;
                }

                gridMin.x = gameObject.transform.position.x - gridWidthOffset - tileOffset;
                gridMin.y = gameObject.transform.position.y + gridHeight - tileOffset - gridOffset;
                gridMin.z = gameObject.transform.position.z - gridDepthOffset - tileOffset;
                gridMax.x = gridMin.x + tileSize * gridWidth;
                gridMax.z = gridMin.z + tileSize * gridDepth;
                gridMax.y = gridMin.y;
            }

            drawGridBase();
            drawMainGrid();
            drawGridBorder();
            moveGrid();
        }
    }

    private void drawGridBase()
    {
        Gizmos.color = gridColorFill;
        if (twoPointFiveDMode)
        {
            if (centreGrid)
                Gizmos.DrawCube(
                    new Vector3(gameObject.transform.position.x - tileOffset,
                        gameObject.transform.position.y - tileOffset - gridOffset,
                        gameObject.transform.position.z + gridHeight - gridOffset + tileOffset),
                    new Vector3(gridWidth * tileSize, gridDepth * tileSize, 0.01f));
            else
                Gizmos.DrawCube(new Vector3(gameObject.transform.position.x + gridWidth / 2 * tileSize - tileOffset,
                        gameObject.transform.position.y + gridDepth / 2 * tileSize - tileOffset - gridOffset,
                        gameObject.transform.position.z + gridHeight - gridOffset + tileOffset),
                    new Vector3(gridWidth * tileSize, gridDepth * tileSize, 0.01f));
        }
        else
        {
            if (centreGrid)
                Gizmos.DrawCube(new Vector3(gameObject.transform.position.x - tileOffset,
                        gameObject.transform.position.y + gridHeight - tileOffset - gridOffset,
                        gameObject.transform.position.z - tileOffset),
                    new Vector3(gridWidth * tileSize, 0.01f, gridDepth * tileSize));
            else
                Gizmos.DrawCube(new Vector3(gameObject.transform.position.x + gridWidth / 2 * tileSize - tileOffset,
                        gameObject.transform.position.y + gridHeight - tileOffset - gridOffset,
                        gameObject.transform.position.z + gridDepth / 2 * tileSize - tileOffset),
                    new Vector3(gridWidth * tileSize, 0.01f, gridDepth * tileSize));
        }
    }

    public void drawMainGrid() //fixed for scale
    {
        Gizmos.color = gridColorNormal;
        if (tileSize != 0)
        {
            if (!twoPointFiveDMode)
                for (var i = tileSize; i < gridWidth * tileSize; i += tileSize)
                    Gizmos.DrawLine(
                        new Vector3(i + gridMin.x, gridMin.y, gridMin.z),
                        new Vector3(i + gridMin.x, gridMin.y, gridMax.z));
            else
                for (var i = tileSize; i < gridWidth * tileSize; i += tileSize)
                    Gizmos.DrawLine(
                        new Vector3(i + gridMin.x, gridMin.y, gridMin.z),
                        new Vector3(i + gridMin.x, gridMax.y, gridMin.z)
                    );
            //横竖皆画
            if (!twoPointFiveDMode)
                for (var j = tileSize; j < gridDepth * tileSize; j += tileSize)
                    Gizmos.DrawLine(
                        new Vector3(gridMin.x, gridMin.y, j + gridMin.z),
                        new Vector3(gridMax.x, gridMin.y, j + gridMin.z)
                    );
            else
                for (var j = tileSize; j < gridDepth * tileSize; j += tileSize)
                    Gizmos.DrawLine(
                        new Vector3(gridMin.x, j + gridMin.y, gridMin.z),
                        new Vector3(gridMax.x, j + gridMin.y, gridMin.z)
                    );
        }
    }

    private void drawGridBorder()
    {
        Gizmos.color = gridColorBorder;
        if (!twoPointFiveDMode)
        {
            // left side
            Gizmos.DrawLine(new Vector3(gridMin.x, gridMin.y, gridMin.z), new Vector3(gridMin.x, gridMin.y, gridMax.z));

            //bottom
            Gizmos.DrawLine(new Vector3(gridMin.x, gridMin.y, gridMin.z), new Vector3(gridMax.x, gridMin.y, gridMin.z));

            // Right side
            Gizmos.DrawLine(new Vector3(gridMax.x, gridMin.y, gridMin.z), new Vector3(gridMax.x, gridMin.y, gridMax.z));

            //top
            Gizmos.DrawLine(new Vector3(gridMin.x, gridMin.y, gridMax.z), new Vector3(gridMax.x, gridMin.y, gridMax.z));
        }
        else
        {
            // left side
            Gizmos.DrawLine(new Vector3(gridMin.x, gridMin.y, gridMin.z), new Vector3(gridMin.x, gridMax.y, gridMin.z));

            //bottom
            Gizmos.DrawLine(new Vector3(gridMin.x, gridMin.y, gridMin.z), new Vector3(gridMax.x, gridMin.y, gridMin.z));

            // Right side
            Gizmos.DrawLine(new Vector3(gridMax.x, gridMin.y, gridMin.z), new Vector3(gridMax.x, gridMax.y, gridMin.z));

            //top
            Gizmos.DrawLine(new Vector3(gridMin.x, gridMax.y, gridMin.z), new Vector3(gridMax.x, gridMax.y, gridMin.z));
        }
    }

    public void moveGrid()
    {
        var box = gameObject.GetComponent<BoxCollider>();
        box.enabled = toolEnable;
        gridColliderPosition = box.center;
        if (twoPointFiveDMode)
        {
            if (centreGrid)
            {
                gridColliderPosition.x = -tileOffset;
                gridColliderPosition.y = -tileOffset;
                gridColliderPosition.z = gridHeight + tileOffset;
            }
            else
            {
                gridColliderPosition.x = gridWidth / 2 * tileSize - tileOffset;
                gridColliderPosition.y = gridWidth / 2 * tileSize - tileOffset;
                gridColliderPosition.z = gridHeight + tileOffset;
            }
        }
        else
        {
            if (centreGrid)
            {
                gridColliderPosition.x = -tileOffset;
                gridColliderPosition.y = gridHeight - tileOffset;
                gridColliderPosition.z = -tileOffset;
            }
            else
            {
                gridColliderPosition.x = gridWidth / 2 * tileSize - tileOffset;
                gridColliderPosition.y = gridHeight - tileOffset;
                gridColliderPosition.z = gridDepth / 2 * tileSize - tileOffset;
            }
        }

        box.center = gridColliderPosition;
    }
}