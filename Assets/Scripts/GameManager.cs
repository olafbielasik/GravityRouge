using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int energyCollected = 0;
    public int energyToWin = 10;

    public TextMeshProUGUI energyText; 
    public GameObject winPanel;
    public TextMeshProUGUI winTimerText;

    private float elapsedTime = 0f;
    private bool gameEnded = false;

    void Awake()
    {
        instance = this;
        if (winTimerText != null) winTimerText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameEnded)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void AddEnergy()
    {
        energyCollected++;
        energyText.text = "Energy: " + energyCollected + " / " + energyToWin;

        if (energyCollected >= energyToWin)
        {
            winPanel.SetActive(true);
            Time.timeScale = 0f;
            gameEnded = true;

            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            winTimerText.text = $"Time: {minutes:00}:{seconds:00}";
            winTimerText.gameObject.SetActive(true);
        }
    }
}

