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


    private void Awake()
    {
        player = GetComponent<Player>();
        actions = new PlayerActions();
        rb2D = GetComponent<Rigidbody2D>();
        playerAnimations = GetComponent<PlayerAnimations>();
        currentSpeed = speed;
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
            return;//not moving = no updates
        }
        // Update parameters
        playerAnimations.SetMovingAnimation(true);
        playerAnimations.SetMoveAnimation(moveDirection);
    }

    private void HandleRunning()
    {
        if (!enableRunning) return;

        // 🆕 直接使用Input.GetKey检测Shift键
        bool runInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // 只有在移动时才能奔跑
        if (runInput && moveDirection != Vector2.zero)
        {
            currentSpeed = speed * runSpeedMultiplier;
        }
        else
        {
            currentSpeed = speed;
        }
    }




    private void OnEnable()
    {
        actions.Enable();
    }


    private void OnDisable()
    {
        actions.Disable();
    }
}
