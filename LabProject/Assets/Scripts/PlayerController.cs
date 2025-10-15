using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement Settings")]
    public float forwardSpeed = 10f; 	  // Forward speed
    public float laneDistance = 4f; 	  // Distance between lanes
    public float laneChangeSpeed = 10f;   // Speed of lane change
    
    [Header("Boundary Settings")]
    public float maxLaneOffset = 3.8f; 
    

    [Header("Jump Settings")]
    public float jumpForce = 7f; 		  // How strong the jump is
    private bool isGrounded = true; 	  // Check if player is on the ground

    private int desiredLane = 1; 		  // 0 = Left, 1 = Middle, 2 = Right
    private float targetX; 				  // Target X position for lane

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; 			  // Enable gravity for jumping
        rb.freezeRotation = true;
        targetX = 0f; 					  // Start in middle lane
    }

    void Update()
    {
        // --- Lane Switching ---
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++;
            if (desiredLane > 2)
                desiredLane = 2;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if (desiredLane < 0)
                desiredLane = 0;
        }

        // --- Jump ---
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            Jump();
        }

        // Calculate target X position based on the desired lane
        float calculatedTargetX = (desiredLane - 1) * laneDistance;
        
        // Clamp the target X position to prevent going past the road edge.
        targetX = Mathf.Clamp(calculatedTargetX, -maxLaneOffset, maxLaneOffset);
    }

    void FixedUpdate()
    {
        // --- Forward movement ---
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        // --- Horizontal (lane) movement ---
        float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.fixedDeltaTime);

        // Combine movement vectors
        Vector3 newPosition = new Vector3(newX, rb.position.y, rb.position.z) + forwardMove;

        rb.MovePosition(newPosition);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // Check if the collision surface normal is pointing mostly upwards (reliable ground check)
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
}