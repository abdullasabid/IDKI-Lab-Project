using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;          // Reference to the player
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Camera offset (adjust as needed)
    public float smoothSpeed = 5f;    // Smooth follow speed

    void LateUpdate()
    {
        if (player == null) return;

        // Desired position with offset
        Vector3 desiredPosition = player.position + offset;

        // Smoothly move camera to target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Apply position
        transform.position = smoothedPosition;

        // Keep looking at player (optional)
        transform.LookAt(player);
    }
}
