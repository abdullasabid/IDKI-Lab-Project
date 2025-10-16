using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float laneDistance = 4f;
    public float laneChangeSpeed = 10f;

    [Header("Speed Increase Settings")]
    public float speedIncreaseRate = 0.1f;
    public float maxSpeed = 25f;
    private float speedTimer = 0f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    private bool isGrounded = true;

    private int desiredLane = 1;
    private float targetX;

    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.freezeRotation = true;
        targetX = 0f;

        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane = Mathf.Clamp(desiredLane + 1, 0, 2);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane = Mathf.Clamp(desiredLane - 1, 0, 2);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            Jump();
        }

        speedTimer += Time.deltaTime;
        if (speedTimer >= 1f)
        {
            IncreaseSpeed();
            speedTimer = 0f;
        }

        targetX = (desiredLane - 1) * laneDistance;
    }

    void FixedUpdate()
    {
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.fixedDeltaTime);

        Vector3 newPosition = new Vector3(newX, rb.position.y, rb.position.z) + forwardMove;

        rb.MovePosition(newPosition);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void IncreaseSpeed()
    {
        forwardSpeed = Mathf.Min(forwardSpeed + speedIncreaseRate, maxSpeed);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.7f)
                {
                    isGrounded = true;
                    return;
                }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (gameManager != null)
            {
                gameManager.GameOver();
                FindObjectOfType<AudioManager>().PlaySound("GameOver");
            }
        }
    }
}