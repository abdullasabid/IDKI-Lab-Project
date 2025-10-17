using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public ParticleSystem dirtParticle;

    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float sideSpeed = 10f;
    public float maxX = 4f; // Road boundary

    [Header("Smooth Movement Settings")]
    public float acceleration = 10f;   // How fast player speeds up when moving sideways
    public float deceleration = 15f;   // How fast player stops when key is released
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
    }

    void Update()
    {
        HandleMovementInput();

        // Increase forward speed gradually
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
        // Constant forward movement
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        // Horizontal movement (smooth)
        Vector3 horizontalMove = Vector3.right * currentHorizontalSpeed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + forwardMove + horizontalMove;

        // Prevent going off-road
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
        else
            targetSpeed = 0f;

        // Smoothly change speed toward target speed
        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, 0f, deceleration * Time.deltaTime);
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;

        // Stop dirt when jumping
        if (dirtParticle.isPlaying)
            dirtParticle.Stop();
    }

    private void IncreaseSpeed()
    {
        forwardSpeed = Mathf.Min(forwardSpeed + speedIncreaseRate, maxSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.7f)
                {
                    isGrounded = true;

                    // Play dirt only after landing
                    if (!dirtParticle.isPlaying)
                        dirtParticle.Play();
                }
            }
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
        }
    }
}
