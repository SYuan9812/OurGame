using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;
    [SerializeField] private float runSpeedMultiplier = 1.5f;
    [SerializeField] private bool enableRunning = true;

    private readonly int moveX = Animator.StringToHash("MoveX");
    private readonly int moveY = Animator.StringToHash("MoveY");
    private readonly int moving = Animator.StringToHash("Moving");
    private readonly int running = Animator.StringToHash("Running");

    private PlayerActions actions;
    private Rigidbody2D rb2D;
    private Animator animator;
    private Vector2 moveDirection;
    private bool isRunning = false;
    private float currentSpeed;

    private void Awake()
    {
        actions = new PlayerActions();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
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
        rb2D.MovePosition(rb2D.position + moveDirection * (currentSpeed * Time.fixedDeltaTime));
    }


    private void ReadMovement()
    {
        moveDirection = actions.Movement.Move.ReadValue<Vector2>().normalized;
        if (moveDirection == Vector2.zero)
        {
            animator.SetBool(moving, false);
            animator.SetBool(running, false);
            return;//not moving = no updates
        }
        // Update parameters
        animator.SetBool(moving, true);
        animator.SetFloat(moveX, moveDirection.x);
        animator.SetFloat(moveY, moveDirection.y);

        animator.SetBool(running, isRunning && moveDirection != Vector2.zero);
    }

    private void HandleRunning()
    {
        if (!enableRunning) return;

        // 🆕 直接使用Input.GetKey检测Shift键
        bool runInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // 只有在移动时才能奔跑
        if (runInput && moveDirection != Vector2.zero)
        {
            isRunning = true;
            currentSpeed = speed * runSpeedMultiplier;
        }
        else
        {
            isRunning = false;
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
