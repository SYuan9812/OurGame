using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private readonly int moveX = Animator.StringToHash("MoveX");
    private readonly int moveY = Animator.StringToHash("MoveY");
    private readonly int moving = Animator.StringToHash("Moving");
    private readonly int dead = Animator.StringToHash("Dead");
    private readonly int theEnd = Animator.StringToHash("TheEnd");

    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerWeaponManager playerWeaponManager;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerWeaponManager = GetComponent<PlayerWeaponManager>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void SetDeadAnimation()
    {
        animator.SetTrigger(dead);
    }

    public void SetMovingAnimation(bool value)
    {
        animator.SetBool(moving, value);
    }

    public void SetMoveAnimation(Vector2 dir)
    {
        animator.SetFloat(moveX, dir.x);
        animator.SetFloat(moveY, dir.y);
    }

    public void SetTheEndAnimation()
    {
        animator.SetTrigger(theEnd);
        DisablePlayerActions();

        if (rb2d != null)
        {
            rb2d.velocity = Vector2.zero;
            rb2d.isKinematic = true;
        }
    }

    private void DisablePlayerActions()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        if (playerWeaponManager != null)
        {
            playerWeaponManager.enabled = false;
        }
    }
}
