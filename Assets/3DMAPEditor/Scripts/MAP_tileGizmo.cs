using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAP_tileGizmo : MonoBehaviour
{
    [HideInInspector]
    public List<string> customBrushMeshName;

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "YuME_tileGizmo", true);
    }

}