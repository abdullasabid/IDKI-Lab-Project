using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f; // Degrees per second

    void Start()
    {
        // Optional: Add random Y rotation for variety at the start
        transform.Rotate(0, Random.Range(0f, 360f), 0, Space.World);
    }

    void Update()
    {
        // Smooth Y-axis rotation (Yaw)
        // Correct calculation: Speed * Time.deltaTime on the desired axis (Y)
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Play pickup sound
            // Use null-conditional operator (?) for safety
            FindObjectOfType<AudioManager>()?.PlaySound("PickUpCoin");

            // Check the tag of the collected object and add points
            if (gameObject.CompareTag("Coin"))
            {
                GameManager.numberOfCoins += 1;
                Debug.Log("Collected Coin! +1 | Total: " + GameManager.numberOfCoins);
            }
            else if (gameObject.CompareTag("Star"))
            {
                GameManager.numberOfCoins += 10;
                Debug.Log("Collected Star! +10 | Total: " + GameManager.numberOfCoins);
            }

            // Destroy the collected object
            Destroy(gameObject);
        }
    }
}