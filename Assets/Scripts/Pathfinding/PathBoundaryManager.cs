using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridPathfinding;

public class PathBoundaryManager : MonoBehaviour
{
    private static PathBoundaryManager Instance { get; set; }
    public MeshFilter[] meshes;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public static void SetupBoundaries(PathNode[] oneMove, PathNode[] maxMove) {
        Vector3[] vertices = new Vector3[oneMove.Length];
        List<Vector3> triangles = new List<Vector3>();

        for(int i = 0; i < oneMove.Length; i++) {
            vertices[i] = oneMove[i].position.ToVector3Int();

            Mesh mesh = new Mesh();
            mesh.name = "OneMove";
            mesh.vertices = vertices;

            
        }
    }


}
