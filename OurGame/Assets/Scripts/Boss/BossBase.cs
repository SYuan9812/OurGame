using UnityEngine;

public class BossBase : MonoBehaviour
{
    [Header("Components Reference")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Rigidbody2D rb;

    [Header("Motion")]
    [SerializeField] protected float moveSpeed = 3f;

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

    protected Vector2 moveDirection;

    protected virtual void Awake()
    {
        BossMoveXHash = Animator.StringToHash("BossMoveX");
        BossMoveYHash = Animator.StringToHash("BossMoveY");
        BossIsAttackingMeleeHash = Animator.StringToHash("BossIsAttackingMelee");
        BossIsAttackingRangedHash = Animator.StringToHash("BossIsAttackingRanged");
        BossIsDeadHash = Animator.StringToHash("BossIsDead");

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
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
        animator.SetBool(BossIsDeadHash, true);
    }

    protected virtual bool IsDead()
    {
        return animator.GetBool(BossIsDeadHash);
    }

    public Vector2 GetMoveDirection() => moveDirection;
}