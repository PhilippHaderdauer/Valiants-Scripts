using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    [SerializeField]
    private Mesh meshToDraw;

    private void OnDrawGizmos()
    {
        Gizmos.DrawMesh(meshToDraw, transform.position);
    }
}
