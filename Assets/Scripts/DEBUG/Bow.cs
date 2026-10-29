using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrow;
    public float launchForce = 10f;
    public Transform shootPoint;
    public Transform aimPoint;
    private Vector2 bowPosition;

    void Update()
    {
        bowPosition = transform.position;
        Vector2 direction = (aimPoint.position - transform.position).normalized;

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg); 
    }

    public void ShootArrow(Vector2 direction)
    {
        GameObject newArrow = Instantiate(arrow, shootPoint.position, shootPoint.rotation);
        Rigidbody2D rb = newArrow.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * launchForce, ForceMode2D.Impulse);
    }
}
