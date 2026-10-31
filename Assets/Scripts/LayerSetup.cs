using System.Collections.Generic;
using UnityEngine;

public class LayerSetup : MonoBehaviour
{
    public static LayerSetup _instance;
    public static LayerSetup Instance { get { return _instance; } }

    public List<GameObject> depthLayers;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {

        depthLayers = new List<GameObject>();

        foreach (GameObject layer in GameObject.FindGameObjectsWithTag("SceneDepthLayer"))
        {
            depthLayers.Add(layer);
        }



    }
}

/*

vrstvy (složky)
-gameobjecty
--collidery



*/