using UnityEngine;

public class AsteroidHazard : MonoBehaviour
{
    public float speed = 6f;
    private Vector2 direction = Vector2.left; 

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("☠️ Asteroida trafiła gracza!");
            GameManager.instance.GameOver();
            Destroy(gameObject);
        }
    }
}









