using UnityEngine;

[CreateAssetMenu(menuName = "ParkourSystem/CustomAction/NewVaultAction")]
public class VaultAction : ParkourAction
{
    public override bool CheckIfPossible(ObstacleHitData obstacleHitData, Transform player)
    {
        if (!base.CheckIfPossible(obstacleHitData, player))
        {
            return false;
        }

        // Get hit point in local space
        var hitPoint = obstacleHitData.forwardHit.transform.InverseTransformPoint(obstacleHitData.forwardHit.point);

        // Determine if we need to mirror based on which quadrant the hit point is in
        Mirror = hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0;

        if (Mirror)
        {
            MirroringTargetBodyParts();
        }

        return true;
    }
}
