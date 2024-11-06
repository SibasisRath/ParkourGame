using UnityEngine;

public class WaterHeightScaller : MonoBehaviour
{
    [SerializeField] private float maxHeight = 1f;   // Target height to move to
    [SerializeField] private float maxScale = 2f;    // Target scale to reach
    [SerializeField] private float duration = 1f;    // Duration over which to scale and move

    private Vector3 initialPosition;                 // Initial position of the object
    private Vector3 targetPosition;                  // Target position with max height
    private Vector3 initialScale;                    // Initial scale of the object
    private Vector3 targetScale;                     // Target scale
    private float elapsedTime;                       // Time elapsed since scaling started

    private void Start()
    {
        // Clamp input values to ensure they are non-negative
        maxHeight = Mathf.Max(0, maxHeight);
        maxScale = Mathf.Max(0, maxScale);
        duration = Mathf.Max(0.01f, duration); // Ensure duration is not zero

        // Set initial position and target position (height adjustment)
        initialPosition = transform.position;
        targetPosition = new Vector3(initialPosition.x, maxHeight, initialPosition.z);

        // Set initial scale and target scale
        initialScale = transform.localScale;
        targetScale = new Vector3(initialScale.x, maxScale, initialScale.z);
    }

    private void Update()
    {
        if (elapsedTime >= duration)
        {
            return;
        }

        // Update elapsed time and normalized time value t
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        // Interpolate position and scale over time
        transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
        transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
    }
}