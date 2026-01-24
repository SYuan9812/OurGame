using UnityEngine;
using System.Collections;

public class BossBase : MonoBehaviour
{
    [Header("Components Reference")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Rigidbody2D rb;

    [Header("Motion")]
    [SerializeField] protected float moveSpeed = 10f;

    [Header("Boundary")]
    [SerializeField] protected float minX;
    [SerializeField] protected float maxX;
    [SerializeField] protected float minY;
    [SerializeField] protected float maxY;

    [Header("Animation Parameters")]
    protected int BossMoveXHash;
    protected int BossMoveYHash;
    protected int BossIsAttackingMeleeHash;
    protected int BossIsAttackingRangedHash;
    protected int BossIsDeadHash;

    [Header("Boss Stats")]
    [SerializeField] public int maxHealth = 30;
    [SerializeField] protected int flashTimes = 3;
    [SerializeField] protected float FlashDuration = 0.1f;
    [SerializeField] protected Color hitFlashColor = Color.green;
    [SerializeField] protected float expReward = 100f;
    public int currentHealth;

    protected Vector2 moveDirection;
    protected bool isFlashing;
    private bool hasInitiatedDeath = false;

    protected virtual void Awake()
    {
        BossMoveXHash = Animator.StringToHash("BossMoveX");
        BossMoveYHash = Animator.StringToHash("BossMoveY");
        BossIsAttackingMeleeHash = Animator.StringToHash("BossIsAttackingMelee");
        BossIsAttackingRangedHash = Animator.StringToHash("BossIsAttackingRanged");
        BossIsDeadHash = Animator.StringToHash("BossIsDead");

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        if (IsDead()) return;

        animator.SetFloat(BossMoveXHash, moveDirection.x);
        animator.SetFloat(BossMoveYHash, moveDirection.y);

        Move();
        ClampToBoundary();
    }

    public virtual void SetMoveDirection(Vector2 dir)
    {
        if (dir.magnitude > 0.01f)
            moveDirection = dir.normalized;
        else
            moveDirection = Vector2.zero;
    }

    protected virtual void Move()
    {
        if (moveDirection == Vector2.zero) return;

        Vector2 newPos = rb.position + moveDirection * moveSpeed * Time.deltaTime;
        rb.MovePosition(newPos);
    }

    private void ClampToBoundary()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minX, maxX),
            Mathf.Clamp(transform.position.y, minY, maxY),
            transform.position.z
        );
    }

    public virtual void MeleeAttack()
    {
        if (IsDead()) return;
        animator.SetTrigger(BossIsAttackingMeleeHash);
    }

    public virtual void RangedAttack()
    {
        if (IsDead()) return;
        animator.SetTrigger(BossIsAttackingRangedHash);
    }

    public virtual void Die()
    {
        if (IsDead() || hasInitiatedDeath) return;

        hasInitiatedDeath = true;
        animator.SetBool(BossIsDeadHash, true);

        moveDirection = Vector2.zero; //Stop moving
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>(); //Disable colliders
        foreach (var col in childColliders)
        {
            col.enabled = false;
        }

        GivePlayerExp();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnBossKilled();
        }
    }

    private void GivePlayerExp()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            return;
        }

        PlayerExperience playerExp = playerObj.GetComponent<PlayerExperience>();
        if (playerExp == null)
        {
            return;
        }
        playerExp.AddExp(expReward);
    }

    public virtual bool IsDead()
    {
        return animator.GetBool(BossIsDeadHash);
    }

    public Vector2 GetMoveDirection() => moveDirection;


    public virtual void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }


    public void BossTakeDamage(int damage)
    {
        if (IsDead()) return;


        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (!isFlashing)
            StartCoroutine(FlashRedCoroutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRedCoroutine()
    {
        isFlashing = true;
        Color originalColor = spriteRenderer.color;
        int currentFlashCount = 0;

        while (currentFlashCount < flashTimes)
        {
            spriteRenderer.color = hitFlashColor;
            yield return new WaitForSeconds(FlashDuration);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(FlashDuration);

            currentFlashCount++;
        }

        isFlashing = false;
    }


    public bool IsAtBoundary()
    {
        return (transform.position.x <= minX ||
                transform.position.x >= maxX ||
                transform.position.y <= minY ||
                transform.position.y >= maxY);
    }
}