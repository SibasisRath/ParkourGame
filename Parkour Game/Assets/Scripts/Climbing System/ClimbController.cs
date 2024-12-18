using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    [SerializeField] private List<ClimbAction> climbActions;

    private const float MountFromHangTimeDelay = 0.5f;
    private ClimbPoint currentPoint;
    private PlayerController playerController;
    [SerializeField] private EnvironmentScanner envScanner;
    private string currentClimbObjectTag = string.Empty;

    public void SetPlayerController(PlayerController playerController) => this.playerController = playerController;

    private void Update()
    {
        if (playerController == null)
        {
            Debug.Log("no player controller. climb");
        }

        if (playerController.PlayerModel == null)
        {
            Debug.Log("no player model. climb");
        }
        if (!playerController.PlayerModel.IsHanging)
        {
            if (Input.GetButton("Jump") && !playerController.PlayerModel.InAction)
            {
                if (envScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit, "Ledge"))
                {
                    currentClimbObjectTag = "Ledge";
                    currentPoint = GetNearestClimbPoint(ledgeHit.transform, ledgeHit.point);

                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge(GetClimbAction("IdleToHang")));
                }
                else if (envScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ropeHit, "Rope"))
                {
                    currentClimbObjectTag = "Rope";
                    currentPoint = GetNearestClimbPoint(ropeHit.transform, ropeHit.point);
                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge(GetClimbAction("StandToFreehang")));
                }
            }

            if (Input.GetButtonDown("Drop") && !playerController.PlayerModel.InAction)
            {
                if (envScanner.DropLedgeCheck(out RaycastHit ledgeHit))
                {
                    currentClimbObjectTag = "Ledge";
                    currentPoint = GetNearestClimbPoint(ledgeHit.transform, ledgeHit.point);

                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge(GetClimbAction("DropToHang")));
                }
            }
        }
        else
        {

            if (Input.GetButton("Drop") && !playerController.PlayerModel.InAction)
            {
                StartCoroutine(JumpFromHang());
            }

            float h = Mathf.Round(Input.GetAxisRaw("Horizontal"));
            float v = Mathf.Round(Input.GetAxisRaw("Vertical"));
            var inputDir = new Vector2(h, v);

            if (playerController.PlayerModel.InAction || inputDir == Vector2.zero) return;

            // Mount from the hanging state
            if (currentPoint.MountPoint && inputDir.y == 1)
            {
                StartCoroutine(MountFromHang());
                return;
            }

            // Ledge to Ledge Jump
            var neighbour = currentPoint.GetNeighbour(inputDir);
            if (neighbour == null) return;

            if (neighbour.connectionType == ConnectionType.Jump && Input.GetButton("Jump"))
            {
                currentPoint = neighbour.point;

                if (neighbour.direction.y == 1)
                    StartCoroutine(JumpToLedge(GetClimbAction("HangHopUp")));
                else if (neighbour.direction.y == -1)
                    StartCoroutine(JumpToLedge(GetClimbAction("HangHopDown")));
                else if (neighbour.direction.x == 1)
                    StartCoroutine(JumpToLedge(GetClimbAction("HangHopRight")));
                else if (neighbour.direction.x == -1)
                    StartCoroutine(JumpToLedge(GetClimbAction("HangHopLeft")));
            }
            else if (neighbour.connectionType == ConnectionType.Move)
            {
                currentPoint = neighbour.point;
                if (currentClimbObjectTag == "Ledge")
                {
                    if (neighbour.direction.x == 1)
                        StartCoroutine(JumpToLedge(GetClimbAction("ShimmyRight")));
                    else if (neighbour.direction.x == -1)
                        StartCoroutine(JumpToLedge(GetClimbAction("ShimmyLeft")));
                }
                else if (currentClimbObjectTag == "Rope")
                {
                    if (neighbour.direction.x == 1)
                        StartCoroutine(JumpToLedge(GetClimbAction("FreeRightShimmy")));
                    else if (neighbour.direction.x == -1)
                        StartCoroutine(JumpToLedge(GetClimbAction("FreeLeftShimmy")));
                }

            }
        }
    }

    private ClimbAction GetClimbAction(string anim)
    {
        foreach (var action in climbActions)
        {
            if (action.animationName == anim)
            {
                return action;
            }
        }
        Debug.LogWarning("No matching LedgeJumpAction found for the given conditions.");
        return null;
    }

    private IEnumerator JumpToLedge(ClimbAction climbAction)
    {
        var matchParams = new MatchTargetParams
            (
            position: GetHandPos(currentPoint.transform, climbAction.hand, climbAction.handOffset),
            part: climbAction.hand,
            start: climbAction.matchStartTime,
            target: climbAction.matchTargetTime,
            weight: Vector3.one
            );

        var targetRot = Quaternion.LookRotation(-currentPoint.transform.forward);

        yield return playerController.DoAction(climbAction.animationName, matchParams, targetRot, true);

        playerController.PlayerModel.IsHanging = true;
    }

    Vector3 GetHandPos(Transform ledge, AvatarTarget hand, Vector3? handOffset)
    {
        var offVal = handOffset.Value;

        var hDir = (hand == AvatarTarget.RightHand) ? ledge.right : -ledge.right;
        return ledge.position + ledge.forward * offVal.z + Vector3.up * offVal.y - hDir * offVal.x;
    }

    private IEnumerator JumpFromHang()
    {
        playerController.PlayerModel.IsHanging = false;
        if (currentClimbObjectTag == "Rope")
        {
            yield return playerController.DoAction("FreehangDrop");
        }
        else
        {
            yield return playerController.DoAction("JumpFromHang");
        }


        playerController.ResetTargetRotation();
        playerController.SetControl(true);
    }

    IEnumerator MountFromHang()
    {
        playerController.PlayerModel.IsHanging = false;
        yield return playerController.DoAction("MountFromHang");

        playerController.PlayerView.SetCharacterControllerEnabled(true);

        yield return new WaitForSeconds(MountFromHangTimeDelay);

        playerController.ResetTargetRotation();
        playerController.SetControl(true);
    }

    ClimbPoint GetNearestClimbPoint(Transform ledge, Vector3 hitPoint)
    {
        var points = ledge.GetComponentsInChildren<ClimbPoint>();

        ClimbPoint nearestPoint = null;
        float nearestPointDistance = Mathf.Infinity;

        foreach (var point in points)
        {
            float distance = Vector3.Distance(point.transform.position, hitPoint);

            if (distance < nearestPointDistance)
            {
                nearestPoint = point;
                nearestPointDistance = distance;
            }
        }

        return nearestPoint;
    }
}
