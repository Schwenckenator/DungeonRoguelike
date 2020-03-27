using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBoundaryManager : MonoBehaviour
{
    private static PathBoundaryManager Instance { get; set; }
    public LineRenderer[] lines;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }


}
