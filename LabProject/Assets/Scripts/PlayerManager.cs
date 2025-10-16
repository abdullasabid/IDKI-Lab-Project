using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public UnityEngine.UI.Button restartButton;
    public static int numberOfCoins;
    public TextMeshProUGUI scoreText;

    private bool isGameOver = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
        numberOfCoins = 0;
    }

    void Update()
    {
        scoreText.text = "Score: " + numberOfCoins;
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
