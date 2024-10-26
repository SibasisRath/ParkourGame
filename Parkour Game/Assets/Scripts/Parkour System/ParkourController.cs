using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] private List<ParkourAction> actionList;

    [SerializeField] private EnvironmentScanner envScanner;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;

    private ObstacleHitData obstacleHitData;
    private bool isInAction;

    private void FixedUpdate()
    {
        envScanner.ObstacleRayCast();
        obstacleHitData = envScanner.GetObstacleHitData();

        if (obstacleHitData.forwardHitFound && playerController.IsMovingForward() && !isInAction)
        {
            foreach (ParkourAction act in actionList)
            {
                if (act.CheckIfPossible(obstacleHitData, transform))
                {
                    StartCoroutine(DoParkourInAction(act));
                    break;
                }
            }
        }
    }

    private IEnumerator DoParkourInAction(ParkourAction parkourAction)
    {
        isInAction = true;
        playerController.SetControl(false);

        animator.SetBool("MirrorAction", parkourAction.Mirror);

        // Start the animation and yield for one frame
        animator.CrossFadeInFixedTime(parkourAction.AnimationName, 0.2f);
        yield return null;

        // Get the length of the current animation
        AnimatorStateInfo animation = animator.GetCurrentAnimatorStateInfo(0);

        // Determine when to match targets
        float animationTime = 0f;

        while (animationTime < animation.length)
        {
            animationTime += Time.deltaTime;

            // Check if target matching should happen based on body part's matchStartTime and matchTargetTime
            foreach (var bodyPart in parkourAction.TargetMatchingBodyParts)
            {
                if (animationTime >= bodyPart.matchStartTime * animation.length &&
                    animationTime <= bodyPart.matchTargetTime * animation.length)
                {
                    MatchTarget(parkourAction, bodyPart);
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(parkourAction.PostActionDelay);

        if (parkourAction.Mirror)
        {
            parkourAction.MirroringTargetBodyParts();
        }

        playerController.SetControl(true);
        isInAction = false;
    }

    private void MatchTarget(ParkourAction parkourAction, TargetMatchingBodyPart bodyPart)
    {
        if (!animator.isMatchingTarget && !animator.IsInTransition(0))
        {
            animator.MatchTarget(
                parkourAction.MatchPosition(bodyPart),
                transform.rotation,
                bodyPart.matchBodyPart,
                new MatchTargetWeightMask(bodyPart.matchPositionWeight, 0),
                bodyPart.matchStartTime,
                bodyPart.matchTargetTime
            );
        }
    }
}