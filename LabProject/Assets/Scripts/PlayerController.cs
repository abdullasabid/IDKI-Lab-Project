using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public ParticleSystem dirtParticle;

    [Header("UI")]
    public TextMeshProUGUI speedText;

    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float sideSpeed = 7f;
    public float maxX = 4f;

    [Header("Speed Increase Settings")]
    public float speedIncreaseRate = 0.1f;
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

        if (isGrounded && !dirtParticle.isPlaying)
            dirtParticle.Play();
    }

    void Update()
    {
        // Don’t move or jump until the game starts
        if (!GameManager.isGameStarted) return;

        HandleMovementInput();
        UpdateSpeedDisplay();

        speedTimer += Time.deltaTime;
        if (speedTimer >= 1f)
        {
            IncreaseSpeed();
            speedTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.isGameStarted) return;

        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = rb.position + forwardMove;

        newPosition.x = Mathf.Clamp(rb.position.x, -maxX, maxX);
        rb.MovePosition(newPosition);
    }

    private void HandleMovementInput()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.RightArrow))
            horizontalInput = 1f;
        else if (Input.GetKey(KeyCode.LeftArrow))
            horizontalInput = -1f;

        Vector3 movement = new Vector3(horizontalInput * sideSpeed * Time.deltaTime, 0f, 0f);
        rb.MovePosition(rb.position + movement);

        Vector3 clampedPos = rb.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -maxX, maxX);
        rb.position = clampedPos;
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;

        if (dirtParticle.isPlaying)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            if (!dirtParticle.isPlaying)
                dirtParticle.Play();
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (gameManager != null)
            {
                gameManager.GameOver();
                FindObjectOfType<AudioManager>().PlaySound("GameOver");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;

            if (dirtParticle.isPlaying)
                dirtParticle.Stop();
        }
    }
}
