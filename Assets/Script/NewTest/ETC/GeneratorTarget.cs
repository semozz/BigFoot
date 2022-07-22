using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorTarget : MonoBehaviour 
{
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Light Gizmo.tiff");
    }
#endif
}

