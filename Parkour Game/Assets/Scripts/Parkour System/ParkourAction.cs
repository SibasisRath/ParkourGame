using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New parkour action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;
    [SerializeField] string obstacleTag;

    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;

    [SerializeField] bool rotateToObstacle;
    [SerializeField] float postActionDelay;

    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] protected AvatarTarget matchBodyPart;
    [SerializeField] float matchStartTime;
    [SerializeField] float matchTargetTime;
    [SerializeField] Vector3 matchPosWeight = new (0, 1, 0);

    public Quaternion TargetRotation { get; set; }
    public Vector3 MatchPos { get; set; }
    public bool Mirror { get; set; }

    public virtual bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        // Check Tag
        if (!string.IsNullOrEmpty(obstacleTag) && !hitData.forwardHit.transform.CompareTag(obstacleTag))
            return false;

        // Height Tag
        float height = hitData.heightHit.point.y - player.position.y;
        if (height < minHeight || height > maxHeight)
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (enableTargetMatching)
            MatchPos = hitData.heightHit.point;

        return true;
    }

    public AvatarTarget MirroringTargetBodyParts(AvatarTarget bp)
    {
        if (bp == AvatarTarget.LeftHand)
        {
            return AvatarTarget.RightHand;
        }

        else if (bp == AvatarTarget.RightHand)
        {
            return AvatarTarget.LeftHand;
        }

        if (bp == AvatarTarget.LeftFoot)
        {
            return AvatarTarget.RightFoot;
        }

        else if (bp == AvatarTarget.RightFoot)
        {
            return AvatarTarget.LeftFoot;
        }
        else
        {
            return bp;
        }
    }

    public string AnimName => animName;
    public bool RotateToObstacle => rotateToObstacle;
    public float PostActionDelay => postActionDelay;

    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public Vector3 MatchPosWeight => matchPosWeight;
}
