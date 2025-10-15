using UnityEngine;
using UnityEngine.UI; // Needed for UI
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Assign in Inspector
    public Button restartButton;

    private bool isGameOver = false;

    void Start()
    {
        gameOverPanel.SetActive(false); // Hide panel at start
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverPanel.SetActive(true); // Show Game Over screen
        restartButton.gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause game
        Debug.Log("Game Over!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
