using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] platformPrefabs;
    public GameObject enemyPrefab;
    public GameObject powerUpPrefab;
    public int numberOfPlatforms = 100;
    public float startY = 0f;
    public float minYStep = 4f;
    public float maxYStep = 10f;
    public float minX = -12f, maxX = 12f;
    public float minXDistance = 8f;
    public float enemySpawnChance = 0.3f;
    public float powerUpSpawnChance = 0.2f;

    private Vector2[] platformPositions;
    private List<GameObject> platformPool = new List<GameObject>();

    void Start()
    {
        ConfigureBackground();
        Debug.Log("Rozpoczynam generowanie poziomu...");
        platformPositions = new Vector2[numberOfPlatforms];
        GenerateLevel();
        Debug.Log("Zakoñczono generowanie poziomu.");
    }

    void ConfigureBackground()
    {
        GameObject background = GameObject.Find("Background");
        if (background == null)
        {
            Debug.LogWarning("Nie znaleziono obiektu Background.");
            return;
        }

        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("Obiekt Background nie posiada SpriteRenderer.");
            return;
        }

        sr.sortingLayerName = "Background";
        sr.sortingOrder = -100;
        background.transform.position = new Vector3(0f, 0f, 10f);
        Debug.Log("Ustawiono t³o na Sorting Layer: Background, Order: -100, Z: 10");
    }

    void GenerateLevel()
    {
        if (platformPrefabs == null || platformPrefabs.Length == 0)
        {
            Debug.LogError("Brak przypisanych prefabów platform w polu Platform Prefabs!");
            return;
        }

        for (int i = 0; i < numberOfPlatforms; i++)
        {
            GameObject platform = Instantiate(platformPrefabs[Random.Range(0, platformPrefabs.Length)]);
            platform.SetActive(false);
            platformPool.Add(platform);
        }

        float currentY = startY;

        Vector2 startPosition = new Vector2(0f, currentY);
        GameObject startPlatform = platformPool[0];
        startPlatform.transform.position = startPosition;
        float startTilt = Random.Range(-20f, 20f);
        startPlatform.transform.rotation = Quaternion.Euler(0f, 0f, startTilt);
        startPlatform.transform.localScale = new Vector3(5f, 1f, 1f);
        GenerateAsteroidShape(startPlatform);
        startPlatform.SetActive(true);
        platformPositions[0] = startPosition;

        for (int i = 1; i < numberOfPlatforms; i++)
        {
            Vector2 newPosition;
            bool positionValid;
            int attempts = 0;
            const int maxAttempts = 50;

            do
            {
                float newX = Random.Range(minX, maxX);
                newPosition = new Vector2(newX, currentY);
                positionValid = true;

                float distanceX = Mathf.Abs(newPosition.x - platformPositions[i - 1].x);
                if (distanceX < minXDistance)
                {
                    positionValid = false;
                }

                attempts++;
                if (attempts > maxAttempts)
                {
                    Debug.LogWarning($"Nie uda³o siê znaleŸæ odpowiedniej pozycji dla platformy {i + 1} po {maxAttempts} próbach.");
                    break;
                }
            } while (!positionValid);

            platformPositions[i] = newPosition;
            Debug.Log($"Generujê platformê {i + 1} w pozycji: {newPosition}");

            GameObject platform = platformPool[i];
            platform.transform.position = newPosition;
            float tilt = Random.Range(-20f, 20f);
            platform.transform.rotation = Quaternion.Euler(0f, 0f, tilt);
            float randomScaleX = Random.Range(2f, 5f);
            platform.transform.localScale = new Vector3(randomScaleX, 1f, 1f);
            GenerateAsteroidShape(platform);
            platform.SetActive(true);

            if (Random.value < enemySpawnChance && enemyPrefab != null)
            {
                Vector2 enemyPosition = new Vector2(newPosition.x, newPosition.y + 1f);
                Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
            }

            if (Random.value < powerUpSpawnChance && powerUpPrefab != null)
            {
                Vector2 powerUpPosition = new Vector2(newPosition.x, newPosition.y + 1.5f);
                Instantiate(powerUpPrefab, powerUpPosition, Quaternion.identity);
            }

            float yStep = Random.Range(minYStep, maxYStep);
            Debug.Log($"Odstêp w pionie dla platformy {i + 1}: {yStep}");
            currentY += yStep;
        }

        PlacePlayerOnLowestPlatform();
    }

    void GenerateAsteroidShape(GameObject platform)
    {
        if (platform == null)
        {
            Debug.LogError("Platforma przekazana do GenerateAsteroidShape jest null!");
            return;
        }

        SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            DestroyImmediate(spriteRenderer);
        }

        PolygonCollider2D polyCollider = platform.GetComponent<PolygonCollider2D>();
        if (polyCollider == null)
        {
            polyCollider = platform.AddComponent<PolygonCollider2D>();
        }

        Vector2[] points2D = GenerateAsteroidPoints(8, 0.5f, 1f);
        polyCollider.points = points2D;

        Vector3[] vertices3D = new Vector3[points2D.Length];
        for (int i = 0; i < points2D.Length; i++)
            vertices3D[i] = points2D[i];

        Triangulator triangulator = new Triangulator(points2D);
        int[] triangles = triangulator.Triangulate();

        Vector2[] uvs = new Vector2[vertices3D.Length];
        for (int i = 0; i < vertices3D.Length; i++)
        {
            uvs[i] = new Vector2(vertices3D[i].x * 2f, vertices3D[i].y * 2f);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices3D;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshFilter meshFilter = platform.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = platform.AddComponent<MeshRenderer>();
        Material mat = Resources.Load<Material>("PlatformMaterial");
        if (mat != null)
        {
            meshRenderer.material = new Material(mat);
        }
        else
        {
            Debug.LogError("Nie znaleziono materia³u PlatformMaterial w Resources!");
        }

        LineRenderer lineRenderer = platform.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = platform.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = points2D.Length + 1;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0.3f, 0.15f, 0.05f, 1f); 
        lineRenderer.endColor = new Color(0.3f, 0.15f, 0.05f, 1f);
        lineRenderer.sortingOrder = 10;

        for (int i = 0; i < points2D.Length; i++)
        {
            lineRenderer.SetPosition(i, points2D[i]);
        }
        lineRenderer.SetPosition(points2D.Length, points2D[0]);

        if (platform.GetComponent<GravityAttractor>() == null)
        {
            platform.AddComponent<GravityAttractor>();
        }
    }

    Vector2[] GenerateAsteroidPoints(int numPoints, float minRadius, float maxRadius)
    {
        Vector2[] points = new Vector2[numPoints];
        float angleStep = 360f / numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float radius = Random.Range(minRadius, maxRadius);
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            points[i] = new Vector2(x, y);
        }

        return points;
    }

    void PlacePlayerOnLowestPlatform()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Nie znaleziono gracza z tagiem Player!");
            return;
        }

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
        if (platforms.Length == 0)
        {
            Debug.LogError("Nie znaleziono platform z tagiem Ground!");
            return;
        }

        GameObject lowestPlatform = platforms[0];
        foreach (GameObject platform in platforms)
        {
            if (platform.transform.position.y < lowestPlatform.transform.position.y)
            {
                lowestPlatform = platform;
            }
        }

        Vector2 playerPosition = new Vector2(lowestPlatform.transform.position.x, lowestPlatform.transform.position.y + 1f);
        player.transform.position = playerPosition;
        Debug.Log($"Ustawiono gracza na pozycji: {playerPosition}");
    }
}



