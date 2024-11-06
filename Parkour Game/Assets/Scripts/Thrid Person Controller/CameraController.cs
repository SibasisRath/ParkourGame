using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;

    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float distance = 5f;
    [SerializeField] float minDistance = 1f;

    [SerializeField] Vector2 framingOffset;

    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    [SerializeField] LayerMask wallLayer;
    [SerializeField] Image overlayImage;

    float rotationX;
    float rotationY;

    float invertXVal;
    float invertYVal;

    private void Awake()
    {
        invertXVal = (invertX) ? -1 : 1;
        invertYVal = (invertY) ? -1 : 1;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        overlayImage.enabled = true;
    }

    private void Update()
    {
        // Update rotation based on input
        rotationX += Input.GetAxis("Camera Y") * invertYVal * rotationSpeed;
        rotationY += Input.GetAxis("Camera X") * invertXVal * rotationSpeed;

        // Calculate target rotation and position
        Quaternion targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 targetPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

        // Adjust camera distance to prevent clipping through walls
        Vector3 desiredCameraPosition = targetPosition - (targetRotation * Vector3.forward * distance);
        Vector3 adjustedCameraPosition = CheckWallCollision(targetPosition, desiredCameraPosition);

        // Set camera position and rotation
        transform.SetPositionAndRotation(adjustedCameraPosition, targetRotation);
    }

    private Vector3 CheckWallCollision(Vector3 targetPosition, Vector3 desiredCameraPosition)
    {
        // Cast a ray from the follow target to the desired camera position
        Ray ray = new Ray(targetPosition, desiredCameraPosition - targetPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, distance, wallLayer))
        {
            // If there’s a wall in the way, set the camera's position to the hit point, a bit closer to the player
            float adjustedDistance = Mathf.Clamp(hit.distance, minDistance, distance);
            return targetPosition - (ray.direction * adjustedDistance);
        }
        else
        {
            // No wall detected, return the desired camera position
            return desiredCameraPosition;
        }
    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
    public void ToggleBlueOverlay(bool enable)
    {
        if (overlayImage != null)
        {
            overlayImage.enabled = enable;
        }
    }
}
