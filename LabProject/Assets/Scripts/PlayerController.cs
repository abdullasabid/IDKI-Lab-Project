using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveVector;  // Combined movement vector
    public float forwardSpeed;
    public float laneDistance = 4f;
    public float laneChangeSpeed = 5f;

    private int desiredLane = 1; // 0:left, 1:middle, 2:right
    private float targetX;       // Target x position for lane

    void Start()
    {
        controller = GetComponent<CharacterController>();
        forwardSpeed = 5f; // Set your forward speed here
        targetX = 0f;
    }

    void Update()
    {
        // Input to change lanes
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

        // Calculate target X position based on lane
        targetX = (desiredLane - 1) * laneDistance;
    }

    void FixedUpdate()
    {
        // Calculate horizontal movement toward target lane smoothly
        float deltaX = targetX - transform.position.x;
        float horizontalMove = Mathf.Clamp(deltaX, -laneChangeSpeed * Time.fixedDeltaTime, laneChangeSpeed * Time.fixedDeltaTime);

        // Build move vector: horizontal + forward
        moveVector = new Vector3(horizontalMove, 0, forwardSpeed * Time.fixedDeltaTime);

        // Move using CharacterController
        controller.Move(moveVector);
    }
}
