using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private CameraController camController;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float groundCheckRadious = 0.2f;
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private LayerMask groundLayer;
    private Quaternion targetRotation;
    private bool isGrounded;
    private float ySpeed;
    private float moveAmount;
    private Vector3 moveDir;
    private Vector3 moveInput;
    private float horizontalInput;
    private float verticalInput;

    private bool hasControl = true;

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsMovingForward()
    {
        return moveAmount != 0;
    }

    private void FixedUpdate()
    {        
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        moveInput = (new Vector3(horizontalInput, 0, verticalInput)).normalized;
        moveDir = camController.PlannerRotation * moveInput;

        if (!hasControl)
        {
            return;
        }

        GroundChexk();

        if (isGrounded)
        {
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        Vector3 velocity = moveDir * Time.deltaTime;
        velocity.y = ySpeed;
        characterController.Move(velocity * moveSpeed);
    }
    private void Update()
    {
        if (!hasControl)
        {
            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
      

        if (moveAmount > 0)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        animator.SetFloat("MoveAmount" , moveAmount, 0.2f, Time.deltaTime);
    }

    private void GroundChexk()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadious, groundLayer);
    }

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        this.characterController.enabled = hasControl;
        if (!hasControl)
        {
            animator.SetFloat("MoveAmount", 0f);
            targetRotation = transform.rotation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadious);
    }
}
