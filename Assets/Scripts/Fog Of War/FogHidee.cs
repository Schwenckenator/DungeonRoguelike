using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogHidee : MonoBehaviour
{

    public Renderer[] renderers;
    public GameObject ui;

    private void Start() {
        FogOfWar.Instance.AddHidee(this);
    }

    public void SetVisible(bool visible) {
        foreach(var renderer in renderers) {
            renderer.enabled = visible;
        }
        ui.SetActive(visible);
    }
}
