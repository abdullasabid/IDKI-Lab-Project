using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverPanel;
    public UnityEngine.UI.Button restartButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI clickToStartText; // Assign in Inspector
    public TextMeshProUGUI instructionText;  // NEW: Assign in Inspector

    public static int numberOfCoins;
    public static bool isGameStarted;
    private static bool hasGameStartedOnce = false; 

    private bool isGameOver = false;
    
    // NEW: Reference to the PlayerController
    private PlayerController playerController; 

    void Start()
    {
        // Get PlayerController reference
        playerController = FindObjectOfType<PlayerController>();

        gameOverPanel.SetActive(false);
        restartButton.gameObject.SetActive(false);
        numberOfCoins = 0;
        isGameStarted = false;

        // Only show "Click to Start" and "Instruction" on first launch
        if (!hasGameStartedOnce)
        {
            if (clickToStartText != null)
                clickToStartText.gameObject.SetActive(true);

            if (instructionText != null)
                instructionText.gameObject.SetActive(true); 

            Time.timeScale = 0f; // Pause until player clicks
            
            // Ensure particle is stopped while paused
            playerController?.StopDirtParticle();
        }
        else
        {
            // Skip start screen on restart
            if (clickToStartText != null)
                clickToStartText.gameObject.SetActive(false);

            if (instructionText != null)
                instructionText.gameObject.SetActive(false); 

            isGameStarted = true;
            Time.timeScale = 1f;
            
            // Start particle immediately on restart
            playerController?.StartDirtParticle();
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
        hasGameStartedOnce = true;
        Time.timeScale = 1f;
        
        // ACTION: Tell the PlayerController to start the dirt particle
        playerController?.StartDirtParticle(); 

        if (clickToStartText != null)
            clickToStartText.gameObject.SetActive(false);

        if (instructionText != null)
            instructionText.gameObject.SetActive(false); 
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        
        // ACTION: Tell the PlayerController to stop the dirt particle
        playerController?.StopDirtParticle(); 
        
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