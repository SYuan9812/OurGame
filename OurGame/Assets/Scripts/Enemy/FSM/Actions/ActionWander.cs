//Wander State
using UnityEngine;

public class ActionWander : FSMAction
{
    [Header("Config")]
    [SerializeField] private float speed;
    [SerializeField] private float wanderTime; //the time to move to another place
    [SerializeField] private Vector2 moveRange; //range limit of enemy

    private Vector3 movePosition; //Store next move position
    private float timer; //control wander time, =0 when game starts
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GetNewDestination();
    }

    public override void Act()
    {
        timer -= Time.deltaTime; //time <= 0, get new destination
        Vector3 moveDirection = (movePosition - transform.position).normalized; //max length of 1
        Vector3 movement = moveDirection * (speed * Time.deltaTime);
        animator.SetFloat("EnemyX", moveDirection.x);
        animator.SetFloat("EnemyY", moveDirection.y);
        if (Vector3.Distance(transform.position, movePosition) >= 0.5f)
        {
            transform.Translate(movement);
            //leave a gap to avoid collision
        }

        if (timer <= 0f)
        {
            GetNewDestination();
            timer = wanderTime;
        }

    }

    private void GetNewDestination() //get a random position inside the move range
    {
        float randomX = Random.Range(-moveRange.x, moveRange.x);
        //get random position in X axis
        float randomY = Random.Range(-moveRange.y, moveRange.y);
        //get random position in Y axis
        movePosition = transform.position + new Vector3(randomX, randomY);
    }

    private void OnDrawGizmosSelected()
    {
        if (moveRange != Vector2.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, moveRange * 2f);
            Gizmos.DrawLine(transform.position, movePosition);
        }
    }
}