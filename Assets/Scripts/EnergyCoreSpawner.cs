using UnityEngine;
using System.Collections.Generic;

public class EnergyCoreSpawner : MonoBehaviour
{
    public GameObject energyCorePrefab;
    public int numberOfCores = 10;
    public float safeDistance = 4f; 
    public LayerMask asteroidLayer;

    public float verticalSpacing = 40f; 

    void Start()
    {
        SpawnEnergyCores();
    }

    void SpawnEnergyCores()
    {
        int spawned = 0;
        float currentY = transform.position.y;

        while (spawned < numberOfCores)
        {
            Vector2 spawnPos = new Vector2(Random.Range(-5f, 5f), currentY);

            Collider2D hit = Physics2D.OverlapCircle(spawnPos, safeDistance, asteroidLayer);

            if (hit == null)
            {
                Instantiate(energyCorePrefab, spawnPos, Quaternion.identity);
                currentY += verticalSpacing;
                spawned++;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    spawnPos = new Vector2(Random.Range(-5f, 5f), currentY);
                    hit = Physics2D.OverlapCircle(spawnPos, safeDistance, asteroidLayer);
                    if (hit == null)
                    {
                        Instantiate(energyCorePrefab, spawnPos, Quaternion.identity);
                        currentY += verticalSpacing;
                        spawned++;
                        break;
                    }
                }
            }
        }
    }
}


