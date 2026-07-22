using System;
using UnityEngine;

public class CamFollowTarget : MonoBehaviour
{
    public float distanceMultiply = 1f;
    [SerializeField] private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 _moveComp = rb.linearVelocity * distanceMultiply;
        transform.localPosition = new Vector3(_moveComp.x, _moveComp.y, transform.position.z);
    }
}
