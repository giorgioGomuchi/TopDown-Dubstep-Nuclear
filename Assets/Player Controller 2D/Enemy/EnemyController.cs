using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public enum EnemyMode { Idle, Wander, Chase }

    [Header("Data")]
    public EnemyDataSO data;

    [Header("References")]
    public Transform player;
    public Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;
    [SerializeField] private EnemyMode currentMode;

    [SerializeField] private float knockbackLockTime = 0.12f;
    private float knockbackTimer;



    [HideInInspector] public Rigidbody2D rb;

    private EnemyIdleState idle;
    private EnemyWanderState wander;
    private EnemyChaseState meleeChase;
    private EnemySniperShootState sniperChase;


    private static readonly int SPEED_HASH = Animator.StringToHash("Speed");
    private static readonly int HIT_HASH = Animator.StringToHash("Hit");
    private static readonly int DIE_HASH = Animator.StringToHash("Die");

    private float attackTimer;

    private float hitLockTimer;

    private float knockbackLockTimer;


    [Header("Sniper References")]
    public Transform weaponPivot;
    public Transform firePoint;
    public LineRenderer lineRenderer;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        idle = GetComponent<EnemyIdleState>();
        wander = GetComponent<EnemyWanderState>();
        meleeChase = GetComponent<EnemyChaseState>();
        sniperChase = GetComponent<EnemySniperShootState>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (spriteRenderer != null && data != null && data.sprite != null)
            spriteRenderer.sprite = data.sprite;
    }

    private void Start()
    {
        SetMode(EnemyMode.Idle);
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        //if (hitLockTimer > 0f)
        //{
        //hitLockTimer -= Time.deltaTime;
        //rb.velocity = Vector2.zero;
        //  return;
        //}
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;

            if (animator != null)
                animator.SetFloat(SPEED_HASH, rb.velocity.magnitude);

            return; // ⛔ NO tocar rb.velocity aquí
        }

        switch (currentMode)
        {
            case EnemyMode.Idle: idle.Tick(); break;
            case EnemyMode.Wander: wander.Tick(); break;
            case EnemyMode.Chase:
                if (data.attackType == EnemyAttackType.Melee)
                    meleeChase.Tick();
                else
                    sniperChase.Tick();
                break;
        }

        if (animator != null)
            animator.SetFloat(SPEED_HASH, rb.velocity.magnitude);
    }

    public void SetMode(EnemyMode mode)
    {
        if (currentMode == mode) return;

        currentMode = mode;

        if (debugLogs && data != null)
            //Debug.Log($"[{data.enemyName}] Mode -> {mode}", this);

            switch (currentMode)
            {
                case EnemyMode.Idle: idle.Enter(); break;
                case EnemyMode.Wander: wander.Enter(); break;
                case EnemyMode.Chase:
                    if (data.attackType == EnemyAttackType.Melee)
                        meleeChase.Enter();
                    else
                        sniperChase.Enter();
                    break;
            }
    }

    public void TryAttack()
    {
        if (attackTimer > 0f || player == null)
            return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null)
            return;

        playerHealth.TakeDamage(data.damage);
        Debug.Log($"[{data.enemyName}] Hit player for {data.damage}");

        attackTimer = data.attackCooldown;
    }



    // ===== Movement =====
    public void Move(Vector2 direction, float speed)
    {
        rb.velocity = direction * speed;
        UpdateFacing(direction);
    }

    public void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    public void MoveTowards(Vector2 target, float speed)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.velocity = dir * speed;
        UpdateFacing(dir);
    }

    private void UpdateFacing(Vector2 movement)
    {
        if (spriteRenderer == null) return;
        if (Mathf.Abs(movement.x) < 0.05f) return;
        spriteRenderer.flipX = movement.x < 0f;
    }

    // ===== Helpers =====
    public bool PlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= range;
    }

    public Vector2 DirectionToPlayer()
    {
        if (player == null) return Vector2.zero;
        return ((Vector2)player.position - (Vector2)transform.position).normalized;
    }

    // ===== Anim feedback =====
    public void PlayHit()
    {
        if (animator == null) return;
        animator.SetTrigger(HIT_HASH);
    }

    public void PlayDie()
    {
        if (animator == null) return;
        animator.SetTrigger(DIE_HASH);
    }

    public void DisableAIAndMovement()
    {
        enabled = false;
        StopMovement();
        if (rb != null) rb.simulated = false;
    }

    public void LockMovement(float time)
    {
        hitLockTimer = time;
    }


    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        Debug.Log($"[EnemyController] Knockback applied dir={direction} force={force} lock={duration}", this);

        knockbackTimer = duration;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }


    internal void OnKnockback()
    {
        knockbackTimer = knockbackLockTime;

    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        // Chase range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.chaseRange);

        // Stop distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.stopDistance);

        // Player line
        if (player != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, player.position);
        }

#if UNITY_EDITOR
        // Estado actual
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 1.2f,
            currentMode.ToString()
        );
#endif
    }




}
