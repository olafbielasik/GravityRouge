using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    public float gravityStrength = 9.8f;

    public void Attract(Transform target)
    {
        Vector3 direction = (transform.position - target.position).normalized;
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(direction * gravityStrength);
        }
    }
}

