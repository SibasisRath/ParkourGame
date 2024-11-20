using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    private Vector3 climbLedgeRayGap = new Vector3(0, 0.18f, 0); // gap between rays 
    private const float ClimbLedgeOriginHeightOffset = 1.5f;
    private const int NumberOfRaysForClimbLedgeCheck = 10;


    private const float DropLedgeDownOffset = 0.1f;
    private const float DropeLedgeForwardOffset = 2f;
    private const int DropLedgeMaxDistance = 3;


    private const float ObstacleLedgeOriginOffset = 0.5f;
    private const float ObstacleLedgeCheckSpacing = 0.25f;
    private const float ObstacleLedgeYOffset = 0.1f;
    private const int ObstacleLedgeMaxDistance = 2;


    [SerializeField] Vector3 forwardRayOffset = new (0, 2.5f, 0);
    [SerializeField] float forwardRayLength = 0.8f;
    [SerializeField] float heightRayLength = 5;
    [SerializeField] float ledgeRayLength = 10;
    [SerializeField] float climbLedgeRayLength = 1.5f;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask climbLedgeLayer;
    [SerializeField] float ledgeHeightThreshold = 0.75f;

    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = transform;
    }

    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();

        var forwardOrigin = playerTransform.position + forwardRayOffset;
        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, playerTransform.forward, 
            out hitData.forwardHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.red : Color.white);

        if (hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down,
                out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
        }
        
        return hitData;
    }

    public bool ClimbLedgeCheck(Vector3 dir, out RaycastHit ledgeHit, string tag = "")
    {
        ledgeHit = new RaycastHit();

        if (dir == Vector3.zero)
            return false;

        var origin = playerTransform.position + Vector3.up * ClimbLedgeOriginHeightOffset;
        

        for (int i = 0; i < NumberOfRaysForClimbLedgeCheck; i++)
        {
            Debug.DrawRay(origin + climbLedgeRayGap * i, dir);
            if (Physics.Raycast(origin + climbLedgeRayGap * i, dir, out RaycastHit hit, climbLedgeRayLength, climbLedgeLayer)
                && hit.transform.CompareTag(tag))
            {
                ledgeHit = hit;
                return true;
            }
        }

        return false;
    }

    public bool DropLedgeCheck(out RaycastHit ledgeHit)
    {
        ledgeHit = new RaycastHit();

        var origin = playerTransform.position + Vector3.down * DropLedgeDownOffset + playerTransform.forward * DropeLedgeForwardOffset;
        if (Physics.Raycast(origin, -playerTransform.forward, out RaycastHit hit, DropLedgeMaxDistance, climbLedgeLayer))
        {
            ledgeHit = hit;
            return true;
        }


        return false;
    }

    public bool ObstacleLedgeCheck(Vector3 moveDir, out LedgeData ledgeData)
    {
        ledgeData = new LedgeData();

        if (moveDir == Vector3.zero)
            return false;

        
        var origin = playerTransform.position + moveDir * ObstacleLedgeOriginOffset + Vector3.up;

        if (PhysicsUtil.ThreeRaycasts(origin, Vector3.down, ObstacleLedgeCheckSpacing, transform, 
            out List<RaycastHit> hits, ledgeRayLength, obstacleLayer, true))
        {
            var validHits = hits.Where(h => playerTransform.position.y - h.point.y > ledgeHeightThreshold).ToList();

            if (validHits.Count > 0)
            {
                var surfaceRayOrigin = validHits[0].point;
                surfaceRayOrigin.y = playerTransform.position.y - ObstacleLedgeYOffset;

                if (Physics.Raycast(surfaceRayOrigin, playerTransform.position - surfaceRayOrigin, out RaycastHit surfaceHit, ObstacleLedgeMaxDistance, obstacleLayer))
                {
                    Debug.DrawLine(surfaceRayOrigin, playerTransform.position, Color.cyan);

                    float height = playerTransform.position.y - validHits[0].point.y;

                    ledgeData.angle = Vector3.Angle(playerTransform.forward, surfaceHit.normal);
                    ledgeData.height = height;
                    ledgeData.surfaceHit = surfaceHit;

                    return true;
                }
            }
        }

        return false;
    }
}

public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
}

public struct LedgeData
{
    public float height;
    public float angle;
    public RaycastHit surfaceHit;
}
