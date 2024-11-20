using UnityEngine;

public class MatchTargetParams
{
    public Vector3 pos;
    public AvatarTarget bodyPart;
    public Vector3 posWeight;
    public float startTime;
    public float targetTime;

    public MatchTargetParams(Vector3 position, AvatarTarget part, Vector3 weight, float start, float target)
    {
        pos = position;
        bodyPart = part;
        posWeight = weight;
        startTime = start;
        targetTime = target;
    }
}