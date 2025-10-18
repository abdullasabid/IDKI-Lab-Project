using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverPanel;
    public UnityEngine.UI.Button restartButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI clickToStartText; // 👈 Assign in Inspector

    public static int numberOfCoins;
    public static bool isGameStarted;
    private static bool hasGameStartedOnce = false; // 👈 NEW: Tracks if game started before

    private bool isGameOver = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
        restartButton.gameObject.SetActive(false);
        numberOfCoins = 0;
        isGameStarted = false;

        // Only show "Click to Start" on very first launch
        if (!hasGameStartedOnce)
        {
            if (clickToStartText != null)
                clickToStartText.gameObject.SetActive(true);

            Time.timeScale = 0f; // Pause until player clicks
        }
        else
        {
            // Skip start screen on restart
            if (clickToStartText != null)
                clickToStartText.gameObject.SetActive(false);

            isGameStarted = true;
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        scoreText.text = "Score: " + numberOfCoins;

        // Wait for first click only if not started before
        if (!isGameStarted && !hasGameStartedOnce && Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        isGameStarted = true;
        hasGameStartedOnce = true; // ✅ Remember that the game has started before
        Time.timeScale = 1f;

        if (clickToStartText != null)
            clickToStartText.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverPanel.SetActive(true);
        restartButton.gameObject.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Game Over!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
