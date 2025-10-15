using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float forwardSpeed = 10f;     // Speed moving forward
    public float laneDistance = 4f;      // Distance between lanes
    public float laneChangeSpeed = 10f;  // How fast player slides to target lane

    private int desiredLane = 1;         // 0 = Left, 1 = Middle, 2 = Right
    private float targetX;               // X position target for lane

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;           // Prevent falling
        rb.freezeRotation = true;        // Keep upright
        targetX = 0f;
    }

    void Update()
    {
        // Handle lane input
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++;
            if (desiredLane > 2) desiredLane = 2;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if (desiredLane < 0) desiredLane = 0;
        }

        // Calculate target X position based on desired lane
        targetX = (desiredLane - 1) * laneDistance;
    }

    void FixedUpdate()
    {
        // Move forward constantly
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        // Move sideways smoothly toward target lane
        float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.fixedDeltaTime);

        // Combine both moves (keep Y constant)
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z) + forwardMove;

        rb.MovePosition(newPosition);
    }
}