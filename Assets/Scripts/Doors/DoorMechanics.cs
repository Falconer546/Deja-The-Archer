using System.Collections.Generic;
using UnityEngine;

public class DoorMechanics : MonoBehaviour
{
    public static DoorMechanics _instance;
    public static DoorMechanics Instance { get { return _instance; } }

    public List<Collider2D> doorZones = new List<Collider2D>();
    public List<GameObject> doorsLayers; 

    public LayerMask doorLayer;
    public Collider2D playerCollider;

    public static GameObject useableDoor;
    public static int p_activeLayer;

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
        foreach (GameObject dLayer in doorsLayers)
        {
            Collider2D[] allColliders = dLayer.GetComponentsInChildren<Collider2D>(true);

            foreach (Collider2D doorCollider in allColliders)
            {
                if ((doorLayer.value & (1 << doorCollider.gameObject.layer)) != 0)
                    doorZones.Add(doorCollider);
            }
        }
    }

    private void Update()
    {
        //p_activeLayer = 
        GetDoorsForPlayer();
    }

    private void GetDoorsForPlayer()
    {
        foreach (Collider2D door in doorZones)
        {
            ColliderDistance2D distance = Physics2D.Distance(playerCollider, door);

            if (distance.isOverlapped)
            {
                useableDoor = door.gameObject;
                break;
            }
            else
                useableDoor = null;
            
        }
    }

    private void HideLayers()
    {
        
    }
}

/*

mizející vrstvy?

*/