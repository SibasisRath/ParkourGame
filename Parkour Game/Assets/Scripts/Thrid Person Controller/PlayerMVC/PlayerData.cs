using UnityEngine;

[CreateAssetMenu(menuName = "Player SO")]
public class PlayerData : ScriptableObject
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 500f;
    public float dashSpeedMultiplier = 2f;
    public float dashCooldown = 1f;
    public float groundCheckRadius = 0.2f;
    public Vector3 groundCheckOffset;
    public LayerMask groundLayer;
}