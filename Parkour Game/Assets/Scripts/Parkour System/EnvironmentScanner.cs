using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] private Vector3 forwardRayOffset = new (0,2.5f,0);
    [SerializeField] private Vector3 upwardRayOffset = new(0, 2.5f, 0);
    [SerializeField] private float forwardRayLength = 0.8f;
    [SerializeField] private float heightRayLength = 5f;
    [SerializeField] private LayerMask obstacleLayer;
    private ObstacleHitData obstacleHitData;
    private Vector3 forwardOrigin;
    private Vector3 heightOrigin;
    private Vector3 upwardOrigin;

    public ObstacleHitData GetObstacleHitData() { return obstacleHitData; }

    public void ObstacleRayCast()
    {
        forwardOrigin = transform.position + forwardRayOffset;
        
        obstacleHitData.forwardHitFound = Physics.Raycast(forwardOrigin,
            transform.forward, out obstacleHitData.forwardHit, 
            forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (obstacleHitData.forwardHitFound) ? Color.green : Color.red);

        if (obstacleHitData.forwardHitFound)
        {
            heightOrigin = obstacleHitData.forwardHit.point + Vector3.up * heightRayLength;
            
            obstacleHitData.heightHitFound = Physics.Raycast(heightOrigin,
                Vector3.down, out obstacleHitData.heightHit,
                heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (obstacleHitData.heightHitFound) ? Color.green : Color.red);
        }
        else
        {
            upwardOrigin = transform.position + upwardRayOffset;
            obstacleHitData.upwardHitFound = Physics.Raycast(upwardOrigin,
                Vector3.up, out obstacleHitData.upwardHit,
                heightRayLength, obstacleLayer);

            Debug.DrawRay(upwardOrigin, Vector3.up * heightRayLength, (obstacleHitData.upwardHitFound) ? Color.green : Color.red);
        }
    }
}

public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public bool upwardHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
    public RaycastHit upwardHit;
}