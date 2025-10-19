using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro UI

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Particles")]
    public ParticleSystem dirtParticle;

    [Header("UI")]
    public TextMeshProUGUI speedText;

    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float sideSpeed = 10f;
    public float maxX = 4f; // Road boundary

    [Header("Smooth Movement Settings")]
    public float acceleration = 10f;   // How fast player speeds up when moving sideways
    public float deceleration = 15f;   // How fast player stops when key is released
    private float currentHorizontalSpeed = 0f;

    [Header("Speed Increase Settings")]
    public float speedIncreaseRate = 0.5f;
    public float maxSpeed = 25f;
    private float speedTimer = 0f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    private bool isGrounded = true;

    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.freezeRotation = true;

        gameManager = FindObjectOfType<GameManager>();
        UpdateSpeedDisplay();
        
        // Ensure particle is STOPPED at Start, regardless of GameManager's state
        if (dirtParticle != null) 
        {
            dirtParticle.Stop();
        }
    }

    void Update()
    {
        // Now only runs game logic if the game has started
        if (!GameManager.isGameStarted) return; 
        
        // Removed the particle check from here; GameManager will start it.

        HandleMovementInput();
        UpdateSpeedDisplay();

        // Gradually increase speed every second
        speedTimer += Time.deltaTime;
        if (speedTimer >= 1f)
        {
            IncreaseSpeed();
            speedTimer = 0f;
        }

        // Jump input
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.isGameStarted) return;

        // Constant forward movement
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;
        Vector3 horizontalMove = Vector3.right * currentHorizontalSpeed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + forwardMove + horizontalMove;
        newPosition.x = Mathf.Clamp(newPosition.x, -maxX, maxX);
        rb.MovePosition(newPosition);
    }

    private void HandleMovementInput()
    {
        float targetSpeed = 0f;

        if (Input.GetKey(KeyCode.RightArrow))
            targetSpeed = sideSpeed;
        else if (Input.GetKey(KeyCode.LeftArrow))
            targetSpeed = -sideSpeed;

        // Smooth speed transition
        if (Mathf.Abs(targetSpeed) > 0.01f)
            currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, targetSpeed, acceleration * Time.deltaTime);
        else
            currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, 0f, deceleration * Time.deltaTime);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;

        // Stop dirt particle while jumping
        if (dirtParticle != null && dirtParticle.isPlaying)
            dirtParticle.Stop();
    }

    private void IncreaseSpeed()
    {
        forwardSpeed = Mathf.Min(forwardSpeed + speedIncreaseRate, maxSpeed);
    }

    private void UpdateSpeedDisplay()
    {
        if (speedText != null)
            speedText.text = "Speed: " + forwardSpeed.ToString("F1");
    }

    // NEW PUBLIC METHOD: Called by GameManager when the game officially starts
    public void StartDirtParticle()
    {
        if (dirtParticle != null && !dirtParticle.isPlaying && isGrounded)
            dirtParticle.Play();
    }
    
    // NEW PUBLIC METHOD: Called by GameManager when the game officially ends
    public void StopDirtParticle()
    {
        if (dirtParticle != null && dirtParticle.isPlaying)
            dirtParticle.Stop();
    }


    // Detect ground contact — restart dirt on landing
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.7f)
                {
                    // Check if player just landed and game is running
                    if (!isGrounded && GameManager.isGameStarted)
                    {
                        isGrounded = true;

                        // Restart dirt particle after landing
                        if (dirtParticle != null && !dirtParticle.isPlaying)
                            dirtParticle.Play();
                    }
                    return;
                }
            }
        }
    }

    // Stop dirt when player leaves the ground
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            
            // Note: Particle is already stopped in Jump(), but this handles falling off a ledge.
            if (dirtParticle != null && dirtParticle.isPlaying)
                dirtParticle.Stop();
        }
    }

    // Handle collision with obstacles
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (gameManager != null)
            {
                gameManager.GameOver();
                FindObjectOfType<AudioManager>()?.PlaySound("GameOver");
                
                // The dirt particle stop is now handled inside GameManager.GameOver()
            }
        }
    }
}