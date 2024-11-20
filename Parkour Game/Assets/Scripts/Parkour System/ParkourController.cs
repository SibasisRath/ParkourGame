using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] private List<ParkourAction> parkourActions;
    [SerializeField] private ParkourAction jumpDownAction;
    [SerializeField] private float autoDropHeightLimit = 1f;

    [SerializeField] private EnvironmentScanner environmentScanner;
    //[SerializeField] private PlayerView playerView;
    private PlayerController playerController;

    public void SetPlayerController(PlayerController playerController) => this.playerController = playerController;
    private void Awake()
    {
        //playerController = playerView.PlayerController;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;
        }
        var hitData = environmentScanner.ObstacleCheck();

        if (Input.GetButton("Jump") && !playerController.PlayerModel.InAction && !playerController.PlayerModel.IsHanging)
        {
            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }
        }

        if (playerController == null)
        {
            Debug.Log("no player controller. park");
        }

        if (playerController.PlayerModel == null)
        {
            Debug.Log("no player model. park");
        }

        if (playerController.PlayerModel.IsOnLedge && !playerController.PlayerModel.InAction && !hitData.forwardHitFound)
        {
            bool shouldJump = true;
            if (playerController.PlayerModel.LedgeData.height > autoDropHeightLimit && !Input.GetButton("Jump"))
                shouldJump = false;

            if (shouldJump && playerController.PlayerModel.LedgeData.angle <= 50)
            {
                playerController.PlayerModel.IsOnLedge = false;
                StartCoroutine(DoParkourAction(jumpDownAction));
            }
        }
    }

    private IEnumerator DoParkourAction(ParkourAction action)
    {
        playerController.SetControl(false);
        MatchTargetParams matchParams = null;
        if (action.EnableTargetMatching)
        {
            matchParams = new MatchTargetParams
            (
            position: action.MatchPos,
            part: (action.Mirror) ? action.MirroringTargetBodyParts(action.MatchBodyPart) : action.MatchBodyPart,
            weight: action.MatchPosWeight,
            start: action.MatchStartTime,
            target: action.MatchTargetTime
            );
        }

        yield return playerController.DoAction(action.AnimName, matchParams, action.TargetRotation,
            action.RotateToObstacle, action.PostActionDelay, action.Mirror);

        playerController.SetControl(true);
    }
}
