using UnityEngine;

public class PlayerView : MonoBehaviour, ISubmergable
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private EnvironmentScanner environmentScanner;

    public PlayerController PlayerController { private get; set; }
    public CalamityScript Calamity { get; private set; }

    public void PlayerTriggerCalamity(CalamityScript calamityScript)
    {
        Calamity = calamityScript;
        PlayerController.EventService.OnPlayerTriggerCalamity.InvokeEvent();
    }

    private void Update()
    {
        PlayerController.Update();
    }

    public void SetGrounded(bool isGrounded)
    {
        animator.SetBool("isGrounded", isGrounded);
    }
    public void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
    public void SetCharacterControllerEnabled(bool enabled)
    {
        characterController.enabled = enabled;
    }

    public void SetMoveAmount(float moveAmount, float dampDuration)
    {
        animator.SetFloat("moveAmount", moveAmount, dampDuration, Time.deltaTime);
    }

    public void CrossFadeAnimation(string animationName, float duration)
    {
        animator.CrossFadeInFixedTime(animationName, duration);
    }

    public bool IsAnimating(string animationName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    public void SetMirrorAction(bool mirror)
    {
        animator.SetBool("mirrorAction", mirror);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ICollectable>(out var collectable))
        {
            collectable.Interact();
        }
    }

    public void PlayerFinishedTheGameTrigger()
    {
        Debug.Log("Player Game Win trigger.");
        PlayerController.EventService.OnPlayerCompleteLevel.InvokeEvent(ResultType.Winner);
    }

    public void OnFullySubmerged()
    {
        if(PlayerController.PlayerModel.IsDead == false) 
        {
            PlayerController.PlayerModel.IsDead = true;
            PlayerController.EventService.OnPlayerDied.InvokeEvent(ResultType.Did_Not_Win);
            Debug.Log("Player is fully submerged. Player dies.");
            return; 
        }
        
    }

    public void OnPartiallySubmerged()
    {
        Debug.Log("Player is partially submerged.");
    }

    public Animator Animator => animator; // Exposing Animator safely for internal control

    public CharacterController CharacterController => characterController;
    public EnvironmentScanner EnvironmentScanner => environmentScanner;
}

