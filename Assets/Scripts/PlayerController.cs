using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Transform mouseTarget;
    [SerializeField] private Transform groundCheck;
    [Space(10), Header("MOVEMENT")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [Space(10), Header("SPRINT")]
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float maxSprintDuration = 5f; // in seconds
    [SerializeField] private float sprintRechargeRate = 1f;
    [SerializeField] private float sprintCooldown = 1f;
    [Space(10), Header("ANIMATION")]
    [SerializeField] private float animationLerpSpeed = 10f;
    [SerializeField] private float animationSnapThreshold = 0.1f;
    [Space(10), Header("FOOTSTEP")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float velocityThreshold = 0.2f;



    private CharacterController characterController;
    private Animator animator;
    private PlayerUI playerUI;

    private Vector3 moveDirection;
    private Vector3 lookDirection;

    private float currentSprintDuration;
    private bool isSprinting;
    private bool isSprintOnCooldown;
    private float nextStepTime;
    private bool isMoving;
    private bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerUI = GetComponent<PlayerUI>();

        currentSprintDuration = maxSprintDuration;

        Upgrade.OnUpgradeSelected += Upgrade_OnUpgradeSelected;
    }

    private void Upgrade_OnUpgradeSelected(UpgradeSO upgradeState)
    {
        switch (upgradeState.UpgradeType)
        {
            case UpgradeType.MaxHealth:
            case UpgradeType.PickupMagnetRadius:
            case UpgradeType.MinDamage:
            case UpgradeType.MaxDamage:
            case UpgradeType.CritChance:
            case UpgradeType.FireRate:
            case UpgradeType.MaxMagazineCount:
            case UpgradeType.ReloadTime:
            case UpgradeType.WalkSpeed:
                moveSpeed += moveSpeed * upgradeState.UpgradePercent;
                break;
            case UpgradeType.SprintMultiplier:
                sprintMultiplier += sprintMultiplier * upgradeState.UpgradePercent;
                break;
            case UpgradeType.MaxSprintDuration:
                maxSprintDuration += maxSprintDuration * upgradeState.UpgradePercent;
                break;
            case UpgradeType.SprintRechargeRate:
                sprintRechargeRate += sprintRechargeRate * upgradeState.UpgradePercent;
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        playerUI.StaminaBarGO.SetActive(false);
    }
    private void Update()
    {
        HandleMovement();
        HandleLook();
        HandleSprint();
        HandleFootstep();
    }

    private void HandleMovement()
    {
        bool canSprint = !isSprintOnCooldown && currentSprintDuration > 0f;
        if (InputManager.Instance.GetIsSprintPressed() && canSprint)
        {
            isSprinting = true;
            currentSprintDuration -= Time.deltaTime;

            if (!playerUI.StaminaBarGO.activeSelf)
            {
                playerUI.StaminaBarGO.SetActive(true);
            }

            playerUI.StaminaBarFG.fillAmount = currentSprintDuration / maxSprintDuration;
        }
        else
        {
            isSprinting = false;
        }

        float speed = moveSpeed;
        float animationSpeedMultiplier;

        if (isSprinting)
        {
            speed *= sprintMultiplier;
            animationSpeedMultiplier = 2f;
        }
        else
        {
            animationSpeedMultiplier = 1f;
        }

        Vector3 inputDirection = new Vector3(InputManager.Instance.GetMoveInputVector().x, 0f, InputManager.Instance.GetMoveInputVector().y);
        isMoving = inputDirection.magnitude > 0f;

        characterController.Move(inputDirection.normalized * speed * Time.deltaTime);

        Vector3 localMoveDirection = transform.InverseTransformDirection(inputDirection);
        localMoveDirection.Normalize();

        float targetMoveX = localMoveDirection.x * animationSpeedMultiplier;
        float targetMoveY = localMoveDirection.z * animationSpeedMultiplier;

        float currentMoveX = animator.GetFloat("Move X");
        float currentMoveY = animator.GetFloat("Move Y");

        float newMoveX, newMoveY;

        if (Mathf.Abs(currentMoveX - targetMoveX) < animationSnapThreshold)
        {
            newMoveX = targetMoveX;
        }
        else
        {
            newMoveX = Mathf.Lerp(currentMoveX, targetMoveX, animationLerpSpeed * Time.deltaTime);
        }

        if (Mathf.Abs(currentMoveY - targetMoveY) < animationSnapThreshold)
        {
            newMoveY = targetMoveY;
        }
        else
        {
            newMoveY = Mathf.Lerp(currentMoveY, targetMoveY, animationLerpSpeed * Time.deltaTime);
        }

        animator.SetFloat("Move X", newMoveX);
        animator.SetFloat("Move Y", newMoveY);
    }
    private void HandleLook()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetLookInputVector());

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            mouseTarget.position = hitInfo.point;
            Vector3 lookDirection = hitInfo.point - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
    private void HandleSprint()
    {
        if (isSprintOnCooldown)
        {
            currentSprintDuration += sprintRechargeRate * Time.deltaTime;
            currentSprintDuration = Mathf.Clamp(currentSprintDuration, 0f, maxSprintDuration);

            playerUI.StaminaBarFG.fillAmount = currentSprintDuration / maxSprintDuration;

            if (currentSprintDuration >= sprintCooldown)
            {
                isSprintOnCooldown = false;
            }
        }
        else
        {
            if(!isSprinting && currentSprintDuration < maxSprintDuration)
            {
                currentSprintDuration += sprintRechargeRate * Time.deltaTime;
                currentSprintDuration = Mathf.Clamp(currentSprintDuration, 0f, maxSprintDuration);

                playerUI.StaminaBarFG.fillAmount = currentSprintDuration / maxSprintDuration;
            }

            if(currentSprintDuration <= 0f)
            {
                isSprintOnCooldown = true;
            }

            if(currentSprintDuration >= maxSprintDuration)
            {
                playerUI.StaminaBarGO.SetActive(false);
            }
        }
    }
    private void HandleFootstep()
    {
        float currentStepInterval = InputManager.Instance.GetIsSprintPressed() ? sprintStepInterval : walkStepInterval;

        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.2f, LayerMask.GetMask("Ground"));
        isGrounded = colliders.Length > 0;
        
        if(isGrounded && isMoving && Time.time > nextStepTime && characterController.velocity.magnitude > velocityThreshold)
        {
            if(isSprinting) AudioManager.Instance.PlayFootstepSprintAudio();
            else AudioManager.Instance.PlayFootstepWalkAudio();

            nextStepTime = Time.time + currentStepInterval;
        }
    }
}
