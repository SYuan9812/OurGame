using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;
    [SerializeField] private float runSpeedMultiplier = 1.5f;
    [SerializeField] private bool enableRunning = true;

    private PlayerAnimations playerAnimations;
    private PlayerActions actions;
    private Player player;
    private Rigidbody2D rb2D;
    private Vector2 moveDirection;
    private float currentSpeed;

    // Stamina related variables
    private bool isRunning;
    private Coroutine staminaCoroutine; // Coroutine for stamina consume/recover


    private void Awake()
    {
        player = GetComponent<Player>();
        actions = new PlayerActions();
        rb2D = GetComponent<Rigidbody2D>();
        playerAnimations = GetComponent<PlayerAnimations>();
        currentSpeed = speed;

        // Initialize stamina on start: to max and can run
        if (player != null && player.Stats != null)
        {
            player.Stats.CurrentStamina = player.Stats.MaxStamina;
            player.Stats.isStaminaLocked = false;
        }
    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        ReadMovement();
        HandleRunning();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (player.Stats.Health <= 0) return; //if player is stead stop movement
        rb2D.MovePosition(rb2D.position + moveDirection * (currentSpeed * Time.fixedDeltaTime));
    }


    private void ReadMovement()
    {
        moveDirection = actions.Movement.Move.ReadValue<Vector2>().normalized;
        if (moveDirection == Vector2.zero)
        {
            playerAnimations.SetMovingAnimation(false);
            if (isRunning)
            {
                StopRunning();
            }
            return;//not moving = no updates
        }
        // Update parameters
        playerAnimations.SetMovingAnimation(true);
        playerAnimations.SetMoveAnimation(moveDirection);
    }

    private void HandleRunning()
    {
        if (!enableRunning || player.Stats.Health <= 0)
        {
            if (isRunning) StopRunning();
            return;
        }

        // Check run input
        bool runInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        // Can run only if moving and have enough stamina
        bool canRun = runInput && moveDirection != Vector2.zero &&
                      player.Stats.CurrentStamina > 0 && !player.Stats.isStaminaLocked;

        if (canRun && !isRunning)
        {
            StartRunning();
        }
        else if ((!canRun || !runInput) && isRunning)
        {
            StopRunning();
        }
    }


    private void StartRunning()
    {
        isRunning = true;
        currentSpeed = speed * runSpeedMultiplier;

        // Start stamina consume coroutine
        if (staminaCoroutine != null)
        {
            StopCoroutine(staminaCoroutine);
        }
        staminaCoroutine = StartCoroutine(StaminaConsumeCoroutine());
    }

    // Stop running and start stamina recovery
    private void StopRunning()
    {
        isRunning = false;
        currentSpeed = speed;

        // Start stamina recover coroutine
        if (staminaCoroutine != null)
        {
            StopCoroutine(staminaCoroutine);
        }
        staminaCoroutine = StartCoroutine(StaminaRecoverCoroutine());
    }

    // Coroutine for stamina consumption while running (locks when stamina clears)
    private IEnumerator StaminaConsumeCoroutine()
    {
        while (isRunning && player.Stats.CurrentStamina > 0)
        {
            // Consume stamina over time
            player.Stats.CurrentStamina -= player.Stats.staminaConsumeRate * Time.deltaTime;
            // Prevent stamina from going below 0
            player.Stats.CurrentStamina = Mathf.Max(player.Stats.CurrentStamina, 0f);

            // Stop running when stamina is 0
            if (player.Stats.CurrentStamina <= 0)
            {
                player.Stats.isStaminaLocked = true;
                StopRunning();
            }

            yield return null;
        }
    }

    // Coroutine for stamina recovery when not running (unlocks when stamina returns max)
    private IEnumerator StaminaRecoverCoroutine()
    {
        while (!isRunning && player.Stats.CurrentStamina < player.Stats.MaxStamina)
        {
            // Recover stamina over time (faster recovery when not moving)
            float recoverRate = moveDirection == Vector2.zero ? player.Stats.staminaRecoverRate * 2 : player.Stats.staminaRecoverRate;
            player.Stats.CurrentStamina += recoverRate * Time.deltaTime;

            // Prevent stamina from exceeding max
            player.Stats.CurrentStamina = Mathf.Min(player.Stats.CurrentStamina, player.Stats.MaxStamina);

            if (player.Stats.CurrentStamina >= player.Stats.MaxStamina) //unlocking running
            {
                player.Stats.isStaminaLocked = false;
            }

            yield return null;
        }

        // Stop coroutine when stamina is full
        staminaCoroutine = null;
    }

    private void OnEnable()
    {
        if (staminaCoroutine != null)
        {
            StopCoroutine(staminaCoroutine);
        }
        actions.Enable();
    }


    private void OnDisable()
    {
        actions.Disable();
    }

    // Public method to get running state (for UI)
    public bool IsRunning => isRunning;
    public bool IsStaminaLocked => player?.Stats?.isStaminaLocked ?? false;
}
