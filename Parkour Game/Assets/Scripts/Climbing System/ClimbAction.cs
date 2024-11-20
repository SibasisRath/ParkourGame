using UnityEngine;

[CreateAssetMenu(menuName = "Climb System/New Climb Action")]
public class ClimbAction : ScriptableObject
{
    public string animationName;
    public float matchStartTime;
    public float matchTargetTime;
    public AvatarTarget hand = AvatarTarget.RightHand;
    public Vector3 handOffset = new Vector3(0.25f, 0.1f, 0.1f);

    public Quaternion GetTargetRotation(Transform ledge)
    {
        return Quaternion.LookRotation(-ledge.forward);
    }
}