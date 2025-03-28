using UnityEngine;

public class RandomBackground : MonoBehaviour
{
    public Sprite[] backgroundSprites; 
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer nie znaleziony na obiekcie Background!");
            return;
        }

        if (backgroundSprites == null || backgroundSprites.Length == 0)
        {
            Debug.LogError("Brak przypisanych sprite'ów t³a w tablicy backgroundSprites!");
            return;
        }

        int randomIndex = Random.Range(0, backgroundSprites.Length);
        spriteRenderer.sprite = backgroundSprites[randomIndex];
    }
}