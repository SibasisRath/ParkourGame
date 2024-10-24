using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset;
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] private float rotationUpLimit = 45f;
    [SerializeField] private float rotationDownLimit = -45f;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    private float rotationY = 0f;
    private float rotationX = 0f;
    float invertXVal;
    float invertYVal;

    public Quaternion PlannerRotation => Quaternion.Euler(0, rotationY, 0);

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        invertXVal = (invertX) ? -1 : 1;
        invertYVal = (invertY) ? -1 : 1;

        rotationX += Input.GetAxis("Mouse Y") * invertYVal * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, rotationDownLimit, rotationUpLimit);

        rotationY += Input.GetAxis("Mouse X") * invertXVal * mouseSensitivity;


        Quaternion cameraRotation = Quaternion.Euler(-rotationX, rotationY, 0);

        transform.SetPositionAndRotation(followTarget.position - (cameraRotation * offset), cameraRotation);
    }
}
