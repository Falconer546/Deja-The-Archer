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
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();

        foreach (GameObject layer in GameObject.FindGameObjectsWithTag("DoorLayer"))
        {
            doorsLayers.Add(layer);
        }

        foreach (GameObject dLayer in doorsLayers)
        {
            Collider2D[] allColliders = dLayer.GetComponentsInChildren<Collider2D>(true);

            foreach (Collider2D doorCollider in allColliders)
            {
                if (doorCollider.CompareTag("DoorObject") && !doorZones.Contains(doorCollider))
                {
                    doorZones.Add(doorCollider);
                }
            }   
        }
    }

    private void Update()
    {
        GetDoorsForPlayer();
    }

    private void GetDoorsForPlayer()
    {
        foreach (Collider2D door in doorZones)
        {
            ColliderDistance2D distance = Physics2D.Distance(playerCollider, door);

            //Debug.Log("Distance: " + distance.distance + " Overlapped: " + distance.isOverlapped);
            if (distance.isOverlapped)
            {
                useableDoor = door.gameObject;
                break;
            }
            else
                useableDoor = null;
            
        }
    }
}