using UnityEngine;

public class PlayerModel
{
    public int DashCounter { get; set; } = 0;
    public int parkourCounter { get; set; } = 0;
    public bool IsGrounded { get; set; }
    public bool HasControl { get; set; } = true;
    public bool InAction { get; set; }
    public bool IsHanging { get; set; }
    public bool IsOnLedge { get; set; }
    public bool IsDashing { get; set; }
    public bool IsDead { get; set; } = false;

    public Vector3 DesiredMoveDir { get; set; }
    public Vector3 MoveDir { get; set; }
    public Vector3 Velocity { get; set; }
    public float YSpeed { get; set; }
    public Quaternion TargetRotation { get; set; }
    public LedgeData LedgeData { get; set; }
    public float LastDashTime { get; set; } = -Mathf.Infinity;
}
