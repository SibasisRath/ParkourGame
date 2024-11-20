using System.Collections;
using UnityEngine;

public class PlayerController
{
    public PlayerModel PlayerModel { get; set; }
    public PlayerView PlayerView { get; set; }
    public PlayerData PlayerData { get; set; }

    private const float GroundedYSpeed = -0.5f;
    private float moveAmount;
    private float horizontalInput;
    private float verticalInput;
    private float ySpeed;

    private const int dashCountUpperLimit = 10;

    private CameraController cameraController;
    public EventService EventService {  get; private set; }



    public PlayerController(PlayerView view, PlayerData data)
    {
        PlayerModel = new ();
        PlayerView = view;
        PlayerData = data;
        PlayerView.PlayerController = this;
        PlayerView.GetComponent<ParkourController>().SetPlayerController(this);
        PlayerView.GetComponent<ClimbController>().SetPlayerController(this);
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.Init(PlayerView);
    }

    public void Init(EventService eventService)
    {
        this.EventService = eventService;
    }

    public void Update()
    {
        HandelMovement();

        if (!PlayerModel.HasControl) return;

        if (PlayerModel.IsHanging) return;

        PlayerModel.Velocity = Vector3.zero;

        GroundCheck();
        PlayerView.Animator.SetBool("isGrounded", PlayerModel.IsGrounded);

        if (PlayerModel.IsGrounded) { OnGroundPlayerValues(); }
        else { OffGroundPlayerValues(); }


        PlayerModel.Velocity = new(PlayerModel.Velocity.x, PlayerModel.YSpeed, PlayerModel.Velocity.z);

        if (PlayerView.CharacterController.enabled) PlayerView.CharacterController.Move(PlayerModel.Velocity * Time.deltaTime);

        RotateTowardsMoveDirection();
    }

    private void HandelMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        var moveInput = (new Vector3(horizontalInput, 0, verticalInput)).normalized;

        PlayerModel.DesiredMoveDir = cameraController.PlanarRotation * moveInput;
        PlayerModel.MoveDir = PlayerModel.DesiredMoveDir;
    }

    private void OffGroundPlayerValues()
    {
        ySpeed += Physics.gravity.y * Time.deltaTime;

        PlayerModel.Velocity = PlayerView.transform.forward * PlayerData.moveSpeed / 2;
    }

    private void OnGroundPlayerValues()
    {
        if (Input.GetMouseButtonDown(1) && !PlayerModel.IsDashing && (Time.time >= PlayerModel.LastDashTime + PlayerData.dashCooldown))
        {
            PlayerView.StartCoroutine(DashCoroutine());
        }

        ySpeed = GroundedYSpeed;
        PlayerModel.Velocity = PlayerModel.DesiredMoveDir * PlayerData.moveSpeed;

        PlayerModel.IsOnLedge = PlayerView.EnvironmentScanner.ObstacleLedgeCheck(PlayerModel.DesiredMoveDir, out LedgeData ledgeData);
        if (PlayerModel.IsOnLedge)
        {
            PlayerModel.LedgeData = ledgeData;
            LedgeMovement();
        }
        PlayerView.Animator.SetFloat("moveAmount", PlayerModel.Velocity.magnitude / PlayerData.moveSpeed, 0.2f, Time.deltaTime);
    }

   

    private void GroundCheck()
    {
        PlayerModel.IsGrounded = Physics.CheckSphere(PlayerView.transform.TransformPoint(PlayerData.groundCheckOffset), PlayerData.groundCheckRadius, PlayerData.groundLayer);
        PlayerView.SetGrounded(PlayerModel.IsGrounded);
    }

    private IEnumerator DashCoroutine()
    {
        PlayerModel.IsDashing = true;
        PlayerModel.DashCounter++;
        PlayerModel.LastDashTime = Time.time;

        float originalSpeed = PlayerData.moveSpeed;
        PlayerData.moveSpeed *= PlayerData.dashSpeedMultiplier;

        PlayerView.CrossFadeAnimation("Dash", 0.2f);

        yield return new WaitForSeconds(PlayerView.Animator.GetCurrentAnimatorStateInfo(0).length);

        PlayerData.moveSpeed = originalSpeed;
        PlayerModel.IsDashing = false;
    }

    private void RotateTowardsMoveDirection()
    {
        if (moveAmount > 0 && PlayerModel.MoveDir.magnitude > 0.2f)
        {
            PlayerModel.TargetRotation = Quaternion.LookRotation(PlayerModel.MoveDir);          
        }
        PlayerView.transform.rotation = Quaternion.RotateTowards(PlayerView.transform.rotation, PlayerModel.TargetRotation, PlayerData.rotationSpeed * Time.deltaTime);
    }

    private void LedgeMovement()
    {
        float signedAngle = Vector3.SignedAngle(PlayerModel.LedgeData.surfaceHit.normal, PlayerModel.DesiredMoveDir, Vector3.up);
        float angle = Mathf.Abs(signedAngle);

        if (Vector3.Angle(PlayerModel.DesiredMoveDir, PlayerView.transform.forward) >= 80)
        {
            // Don't move, but rotate
            PlayerModel.Velocity = Vector3.zero;
            return;
        }

        if (angle < 60)
        {
            PlayerModel.Velocity = Vector3.zero;
            PlayerModel.MoveDir = Vector3.zero;
        }
        else if (angle < 90)
        {
            // Angle is b/w 60 and 90, so limit the velocity to horizontal direction

            var left = Vector3.Cross(Vector3.up, PlayerModel.LedgeData.surfaceHit.normal);
            var dir = left * Mathf.Sign(signedAngle);

            PlayerModel.Velocity = PlayerModel.Velocity.magnitude * dir;
            PlayerModel.MoveDir = dir;
        }
    }

    public IEnumerator DoAction(string animName, MatchTargetParams matchParams = null,
       Quaternion targetRotation = new Quaternion(), bool rotate = false,
       float postDelay = 0f, bool mirror = false)
    {
        PlayerModel.InAction = true;
        PlayerModel.parkourCounter++;
        PlayerView.SetMirrorAction(mirror);
        PlayerView.CrossFadeAnimation(animName, 0.2f);
        yield return null;

        var animState = PlayerView.Animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(animName))
            Debug.LogError("The parkour animation is incorrect!");

        float rotateStartTime = matchParams != null ? matchParams.startTime : 0f;
        float timer = 0f;

        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            if (rotate && normalizedTime > rotateStartTime)
                PlayerView.transform.rotation = Quaternion.RotateTowards(PlayerView.transform.rotation, targetRotation, PlayerData.rotationSpeed * Time.deltaTime);

            if (matchParams != null)
                MatchTarget(matchParams);

            if (PlayerView.Animator.IsInTransition(0) && timer > 0.5f) // transition offset
                break;

            yield return null;
        }

        yield return new WaitForSeconds(postDelay);
        PlayerModel.InAction = false;
    }

    private void MatchTarget(MatchTargetParams matchParams)
    {
        if (PlayerView.Animator.isMatchingTarget || PlayerView.Animator.IsInTransition(0)) return;

        PlayerView.Animator.MatchTarget(matchParams.pos, PlayerView.transform.rotation, matchParams.bodyPart, new MatchTargetWeightMask(matchParams.posWeight, 0), matchParams.startTime, matchParams.targetTime);
    }

    public void SetControl(bool hasControl)
    {
        PlayerModel.HasControl = hasControl;
        PlayerView.SetCharacterControllerEnabled(hasControl);

        if (!hasControl)
        {
            PlayerView.SetMoveAmount(0f, 0f);
            PlayerModel.TargetRotation = PlayerView.transform.rotation;
        }
    }

    public void ResetTargetRotation()
    {
        // Set the TargetRotation in the model to match the current rotation of the player
        PlayerModel.TargetRotation = PlayerView.transform.rotation;

        // Apply this rotation immediately to the player's transform through PlayerView
        PlayerView.SetRotation(PlayerModel.TargetRotation);
    }

    public bool WasPlayerInHurry()
    {
        return dashCountUpperLimit < PlayerModel.DashCounter;
    }
}