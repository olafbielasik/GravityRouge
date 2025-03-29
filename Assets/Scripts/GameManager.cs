using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int energyCollected = 0;
    public int energyToWin = 10;

    public TextMeshProUGUI energyText;
    public GameObject winPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI winTimerText;

    private float elapsedTime = 0f;
    private bool gameEnded = false;

    void Awake()
    {
        instance = this;

        if (winTimerText != null) winTimerText.gameObject.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
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
            WinGame();
        }
    }

    public void WinGame()
    {
        Debug.Log("🏆 Gracz zebrał wszystkie Energy Core - WIN!");

        if (winPanel != null) winPanel.SetActive(true);
        if (winTimerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            winTimerText.text = $"Time: {minutes:00}:{seconds:00}";
            winTimerText.gameObject.SetActive(true);
        }

        gameEnded = true;
        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        if (gameEnded) return;

        Debug.Log("☠️ GameOver() was called!");

        gameEnded = true;

        if (gameOverPanel != null)
        {
            Debug.Log("🎬 GameOverPanel set active!");
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("❗ GameOverPanel is NOT assigned in Inspector!");
        }

        Time.timeScale = 0f;
    }
}



