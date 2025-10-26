using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;

    private readonly int moveX = Animator.StringToHash("MoveX");
    private readonly int moveY = Animator.StringToHash("MoveY");
    private readonly int moving = Animator.StringToHash("Moving");

    private PlayerActions actions;
    private Rigidbody2D rb2D;
    private Animator animator;
    private Vector2 moveDirection;


    private void Awake()
    {
        actions = new PlayerActions();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        ReadMovement();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb2D.MovePosition(rb2D.position + moveDirection * (speed * Time.fixedDeltaTime));
    }


    private void ReadMovement()
    {
        moveDirection = actions.Movement.Move.ReadValue<Vector2>().normalized;
        if (moveDirection == Vector2.zero)
        {
            animator.SetBool(moving, false);
            return;//not moving = no updates
        }
        // Update parameters
        animator.SetBool(moving, true);
        animator.SetFloat(moveX, moveDirection.x);
        animator.SetFloat(moveY, moveDirection.y);
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
