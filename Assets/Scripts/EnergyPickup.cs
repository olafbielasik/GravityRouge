using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.AddEnergy();
            Destroy(gameObject);
        }
    }
}

