using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float GroundedYSpeed = -0.5f;
    private const float FixedTransitionDampDuration = 0.2f;
    private const float TransitionTimerOffSet = 0.5f;
    private const int FallTimeVelocityReducer = 2;
    private const float MaxMoveValueRequiredForTragetRotation = 0.2f;
    private const int LedgeMovementAngleOffset = 80;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private bool hasControl = true;
    public bool InAction { get; private set; }
    public bool IsHanging { get; set; }

    private Vector3 desiredMoveDir;
    private Vector3 moveDir;
    private Vector3 velocity;

    public bool IsOnLedge { get; set; }
    public LedgeData LedgeData { get; set; }

    private float ySpeed;
    private Quaternion targetRotation;

    [SerializeField] private float dashSpeedMultiplier = 2f;
    [SerializeField] private float dashCooldown = 1f; // Cooldown in seconds


    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private EnvironmentScanner environmentScanner;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        var moveInput = (new Vector3(horizontalInput, 0, verticalInput)).normalized;

        desiredMoveDir = cameraController.PlanarRotation * moveInput;
        moveDir = desiredMoveDir;

        if (!hasControl) return;

        if (IsHanging) return;

        velocity = Vector3.zero;

        GroundCheck();
        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded)
        {
            if (Input.GetMouseButtonDown(1) && !isDashing && (Time.time >= lastDashTime + dashCooldown))
            {
                StartCoroutine(Dash());
            }
            Collider[] ground = new Collider[1];
            Physics.OverlapSphereNonAlloc(transform.TransformPoint(groundCheckOffset), groundCheckRadius, ground, groundLayer);

            //transform.SetParent(ground[0].transform);
            
            
            ySpeed = GroundedYSpeed;
            velocity = desiredMoveDir * moveSpeed;

            IsOnLedge = environmentScanner.ObstacleLedgeCheck(desiredMoveDir, out LedgeData ledgeData);
            if (IsOnLedge)
            {
                LedgeData = ledgeData;
                LedgeMovement();
            }
            animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, FixedTransitionDampDuration, Time.deltaTime);

        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;

            velocity = transform.forward * moveSpeed / FallTimeVelocityReducer; 
        }


        velocity.y = ySpeed;

        if(characterController.enabled) characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0 && moveDir.magnitude > MaxMoveValueRequiredForTragetRotation)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
            rotationSpeed * Time.deltaTime);


    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float originalSpeed = moveSpeed;
        moveSpeed *= dashSpeedMultiplier;

        animator.CrossFadeInFixedTime("Dash", FixedTransitionDampDuration);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        moveSpeed = originalSpeed;
        isDashing = false;
    }

    void LedgeMovement()
    {
        float signedAngle = Vector3.SignedAngle(LedgeData.surfaceHit.normal, desiredMoveDir, Vector3.up);
        float angle = Mathf.Abs(signedAngle);

        if (Vector3.Angle(desiredMoveDir, transform.forward) >= LedgeMovementAngleOffset)
        {
            // Don't move, but rotate
            velocity = Vector3.zero;
            return;
        }

        if (angle < 60)
        {
            velocity = Vector3.zero;
            moveDir = Vector3.zero;
        }
        else if (angle < 90)
        {
            // Angle is b/w 60 and 90, so limit the velocity to horizontal direction

            var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
            var dir = left * Mathf.Sign(signedAngle);

            velocity = velocity.magnitude * dir;
            moveDir = dir;
        }
    }

    public IEnumerator DoAction(string animName, MatchTargetParams matchParams = null,
        Quaternion targetRotation = new Quaternion(), bool rotate = false,
        float postDelay = 0f, bool mirror = false)
    {
        InAction = true;
        animator.SetBool("mirrorAction", mirror);
        animator.CrossFadeInFixedTime(animName, FixedTransitionDampDuration);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(animName))
            Debug.LogError("The parkour animation is wrong!");

        float rotateStartTime = (matchParams != null) ? matchParams.startTime : 0f;

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            if (rotate && normalizedTime > rotateStartTime)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (matchParams != null)
                MatchTarget(matchParams);

            if (animator.IsInTransition(0) && timer > TransitionTimerOffSet)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(postDelay);

        InAction = false;
    }

    void MatchTarget(MatchTargetParams mp)
    {
        if (animator.isMatchingTarget || animator.IsInTransition(0)) return;

        animator.MatchTarget(mp.pos, transform.rotation, mp.bodyPart, new MatchTargetWeightMask(mp.posWeight, 0),
            mp.startTime, mp.targetTime);
    }

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if (!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targetRotation = transform.rotation;
        }
    }

    public void EnableCharacterController(bool enabled)
    {
        characterController.enabled = enabled;
    }

    public void ResetTargetRotation()
    {
        targetRotation = transform.rotation;
    }


    public bool HasControl {
        get => hasControl;
        set => hasControl = value;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

   // public float RotationSpeed => rotationSpeed;
}

public class MatchTargetParams
{
    public Vector3 pos;
    public AvatarTarget bodyPart;
    public Vector3 posWeight;
    public float startTime;
    public float targetTime;
}
