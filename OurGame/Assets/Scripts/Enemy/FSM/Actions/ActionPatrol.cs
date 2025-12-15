//Action Patrol

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPatrol : FSMAction
{
    [Header("Config")]
    [SerializeField] private float speed;
    private Vector3 moveDirection;

    private Waypoint waypoint;
    private int pointIndex;
    private Vector3 nextPosition;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        waypoint = GetComponent<Waypoint>();
    }
    public override void Act()
    {
        FollowPath();
    }

    private void FollowPath()
    {
        moveDirection = (GetCurrentPosition() - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, GetCurrentPosition(), speed * Time.deltaTime);
        animator.SetFloat("EnemyX", moveDirection.x);
        animator.SetFloat("EnemyY", moveDirection.y);
        if (Vector3.Distance(transform.position, GetCurrentPosition()) <= 0.1f)
        {
            UpdateNextPosition();
        }
    }

    private void UpdateNextPosition()
    {
        pointIndex++;
        if(pointIndex > waypoint.Points.Length - 1)
        {
            pointIndex = 0;
        }

        moveDirection = (GetCurrentPosition() - transform.position).normalized;
    }
    private Vector3 GetCurrentPosition()
    {
        return waypoint.GetPosition(pointIndex);
        //public Vector3 GetPosition(int pointIndex)
        //{
        //  return EntityPosition + points[pointIndex];
        //} add this in waypoint script
    }
}
//In waypoint script, in "private void Start()", add "gameStarted = true;"