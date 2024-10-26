using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ParkourSystem/NewParkourAction")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] private string animationName;
    [SerializeField] private string obstacleTag;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private float postActionDelay = 0f;

    [Header("Target Matching")]
    [SerializeField] private bool enableTargetMatching = true;
    [SerializeField] protected List<TargetMatchingBodyPart> targetMatchingBodyParts;

    public bool Mirror;

    protected Transform CurrentObstacleTransform { get; set; }
    protected Vector3 MatchPositionHeight { get; set; }
    protected Vector3 MatchPositionForward { get; set; }

    public Vector3 MatchPosition(TargetMatchingBodyPart targetMatchingBodyPart)
    {
        Vector3 position;

        // Get the base position first
        if (targetMatchingBodyPart.shouldMatchHeight)
        {
            position = MatchPositionHeight;
        }
        else if (targetMatchingBodyPart.shouldMatchForward)
        {
            position = MatchPositionForward;
        }
        else
        {
            return Vector3.one;
        }


        // If mirroring is needed and we have a valid transform
        if (Mirror && CurrentObstacleTransform != null)
        {
            // Transform to obstacle's local space
            Vector3 localPos = CurrentObstacleTransform.InverseTransformPoint(position);

            // Mirror only the X coordinate in local space
            // Keep Z coordinate unchanged to maintain proper forward/backward positioning
            localPos = new Vector3(-localPos.x, localPos.y, localPos.z);

            // Transform back to world space
            return CurrentObstacleTransform.TransformPoint(localPos);
        }

        return position;
    }

    public virtual bool CheckIfPossible(ObstacleHitData obstacleHitData, Transform player)
    {

        if (!string.IsNullOrEmpty(obstacleTag) && !obstacleHitData.forwardHit.transform.CompareTag(obstacleTag))
        {
            return false;
        }

        float height = obstacleHitData.heightHit.point.y - player.position.y;

        if (enableTargetMatching) 
        {
            CurrentObstacleTransform = obstacleHitData.forwardHit.transform;
            MatchPositionHeight = obstacleHitData.heightHit.point;
            MatchPositionForward = obstacleHitData.forwardHit.point;
        }

        return height > minHeight && height < maxHeight;
    }

    public void MirroringTargetBodyParts()
    {
        foreach (var bp in targetMatchingBodyParts)
        {
            if (bp.matchBodyPart == AvatarTarget.LeftHand)
            {
                bp.matchBodyPart = AvatarTarget.RightHand;
            }

            else if (bp.matchBodyPart == AvatarTarget.RightHand)
            {
                bp.matchBodyPart = AvatarTarget.LeftHand;
            }

            if (bp.matchBodyPart == AvatarTarget.LeftFoot)
            {
                bp.matchBodyPart = AvatarTarget.RightFoot;
            }

            else if (bp.matchBodyPart == AvatarTarget.RightFoot)
            {
                bp.matchBodyPart = AvatarTarget.LeftFoot;
            }

        }
    }
    public string AnimationName => animationName;


    public bool EnableTargetMatching => enableTargetMatching;
    public List<TargetMatchingBodyPart> TargetMatchingBodyParts => targetMatchingBodyParts;
    public float PostActionDelay => postActionDelay;
}

[System.Serializable]
public class TargetMatchingBodyPart
{
    public AvatarTarget matchBodyPart; // Made fields public to be accessible in the Inspector
    public float matchStartTime;
    public float matchTargetTime;
    public Vector3 matchPositionWeight = new (0,1,0);
    public bool shouldMatchHeight = true;
    public bool shouldMatchForward = false;
}