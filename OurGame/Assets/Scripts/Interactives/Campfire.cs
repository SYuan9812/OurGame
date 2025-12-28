using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    private Animator animator;
    private List<EnemyBrain> affectedEnemies = new List<EnemyBrain>(); // Track enemies affected by campfire
    private bool hasHealedPlayer = false; // Only heal once per campfire
    private bool isPlayerInRange = false;

    [Header("Healing Settings")]
    [SerializeField] private float healAmount = 5f;


    void Start()
    {
        animator = GetComponent<Animator>();
        PlayCampfireAnimation();
    }

    void Update()
    {
        PlayCampfireAnimation();


        // Check if player presses H while in range and hasn't healed yet
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.H) && !hasHealedPlayer)
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                hasHealedPlayer = true;
            }
        }
    }

    void PlayCampfireAnimation()
    {
        animator.Play("Campfire");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Enemy detection
        if (other.CompareTag("Enemy"))
        {
            EnemyBrain enemy = other.GetComponentInParent<EnemyBrain>();

            if (enemy != null)
            {
                Vector2 pushDir = (enemy.transform.position - transform.position).normalized;
                Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    if (rb.bodyType == RigidbodyType2D.Kinematic)
                    {
                        enemy.transform.position += (Vector3)pushDir * 0.05f; // Push back enemy
                    }
                    else
                    {
                        rb.AddForce(pushDir * 3f, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    enemy.transform.position += (Vector3)pushDir * 0.05f;
                }

                if (enemy.CurrentState != null) // Returning state
                {
                    string currentStateId = enemy.CurrentState.ID;
                    if (currentStateId == "Chase" || currentStateId == "Attack")
                    {
                        enemy.forceStateLock = true;
                        enemy.ChangeState(enemy.initState, force: true);
                        affectedEnemies.Add(enemy);
                    }
                }
            }
            else
            {
                Debug.LogWarning("EnemyBrain not found on object or parent of: " + other.gameObject.name);
            }
        }

        // Player detection
        else if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;


            foreach (var enemy in affectedEnemies)
            {
                if (enemy != null)
                {
                    enemy.forceStateLock = false;
                }
            }
            affectedEnemies.Clear();
        }
    }
}