using System;
using Unity.VisualScripting;
using UnityEngine;

public class CamPlayerFollower : MonoBehaviour
{
    public Transform playerTransform;
    public float xTreshold = 1f, yTreshold = 1f, smoothTime;
    public Vector3 targetPosition;

    void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
        targetPosition = playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float xDiff = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float yDiff = Mathf.Abs(playerTransform.position.y - transform.position.y);
        Vector3 velocity = Vector3.zero;

        if(xDiff > xTreshold || yDiff > yTreshold)
        {
            targetPosition = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                transform.position.z
            );
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
