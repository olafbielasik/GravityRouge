using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public float spawnInterval = 1.5f;
    public float yRange = 8f;
    public float xOffset = 12f;
    public float yDirectionSpread = 0.5f; 

    void Start()
    {
        InvokeRepeating(nameof(SpawnLeft), 0.5f, 1.5f);
        InvokeRepeating(nameof(SpawnRight), 1f, 2f);

    }

    void SpawnLeft()
    {
        Vector3 pos = new Vector3(-xOffset, Random.Range(-yRange, yRange), 0);
        GameObject a = Instantiate(asteroidPrefab, pos, Quaternion.identity);

        Vector2 dir = new Vector2(1f, Random.Range(-yDirectionSpread, yDirectionSpread));
        a.GetComponent<AsteroidHazard>().SetDirection(dir);
    }

    void SpawnRight()
    {
        Vector3 pos = new Vector3(xOffset, Random.Range(-yRange, yRange), 0);
        GameObject a = Instantiate(asteroidPrefab, pos, Quaternion.identity);

        Vector2 dir = new Vector2(-1f, Random.Range(-yDirectionSpread, yDirectionSpread));
        a.GetComponent<AsteroidHazard>().SetDirection(dir);
    }
}







