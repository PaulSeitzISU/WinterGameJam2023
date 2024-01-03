using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the object the camera will follow
    public float smoothSpeed = 0.125f; // Speed at which the camera follows the target
    public Vector3 offset; // Offset between the camera and the target

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
