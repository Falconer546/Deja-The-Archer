using UnityEngine;

public class Door : MonoBehaviour
{   
    public Collider2D doorColider;

    void Start()
    {
        doorColider = GetComponent<Collider2D>();
    }
}
