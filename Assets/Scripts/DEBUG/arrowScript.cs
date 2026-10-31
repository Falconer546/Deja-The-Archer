using UnityEngine;

public class arrowScript : MonoBehaviour
{
    public Vector2 direction;
    public float rayLength = 0.1f;
    public Rigidbody2D rb;

    public float linDamp = 0f;
    public float angDamp = 0f;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        direction = transform.right;
    }

    void FixedUpdate()
    {        
        if (rb.linearVelocity.magnitude > 0.1f)
            direction = rb.linearVelocity.normalized; 
        
        HitDetection();
    }

    public void HitDetection()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayLength, LayerMask.GetMask("Ground", "Enemy"));
        if (hit.collider != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * rayLength);
    }
}
