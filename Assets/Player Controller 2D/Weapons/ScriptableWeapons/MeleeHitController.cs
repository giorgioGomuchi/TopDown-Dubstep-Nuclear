using System.Text;
using UnityEngine;

public class MeleeHitController : MonoBehaviour
{
    private int damage;
    private LayerMask targetLayer;
    private bool initialized;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce;

    private Animator animator;
    private Collider2D hitCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        hitCollider = GetComponent<Collider2D>();
    }

    public void Initialize(int damage, LayerMask targetLayer, float fallbackLifetime, float knockbackForce)
    {
        this.damage = damage;
        this.targetLayer = targetLayer;
        initialized = true;
        this.knockbackForce = knockbackForce;

        float lifetime = fallbackLifetime;

        if (animator != null && animator.runtimeAnimatorController != null &&
            animator.runtimeAnimatorController.animationClips.Length > 0)
        {
            lifetime = animator.runtimeAnimatorController.animationClips[0].length;
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized) return;
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

        var projectile = other.GetComponent<EnemyProjectile>();
        if (projectile != null)
        {

            /*
            Vector2 incoming = projectile.CurrentDirection;
            Vector2 normal = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;

            Vector2 reflectDir = Vector2.Reflect(incoming, normal);
           
            
            Vector2 incoming = projectile.CurrentDirection;

           
            Vector2 center = (Vector2)transform.position; // centro del melee
            Vector2 hitPoint = other.ClosestPoint(center);

            Vector2 normal = ((Vector2)hitPoint - center).normalized;

            Vector2 reflectDir = Vector2.Reflect(incoming, normal);

            // Forward del arma (eje +X local del hit)
            Vector2 forward = transform.right;

            float dot = Vector2.Dot(forward, incoming);



            //Vector2 reflectDir = -projectile.CurrentDirection;

            //Vector2 reflectDir = -incoming;
            if (dot < 0f) // solo reflejar si viene de frente
            {
                projectile.Reflect(reflectDir);
            }
             */

            Vector2 incoming = projectile.CurrentDirection;

            // Dirección hacia delante del arma
            Vector2 forward = transform.right;

            // Solo permitir reflect si la bala viene de frente
            float dot = Vector2.Dot(incoming, -forward);

            if (dot > 0.5f) // viene suficientemente de frente
            {
                projectile.Reflect(forward);
            }

            //projectile.Reflect(-projectile.CurrentDirection);
            //projectile.Reflect(reflectDir);

            //Debug.Break();

            return;
        }

        // daño
        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // ✅ knockback: lo buscamos en el EnemyHealth (o un componente que lo implemente)
        var enemyHealth = other.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            Vector2 hitDir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
            enemyHealth.ApplyKnockback(hitDir, knockbackForce);

            Debug.Log($"[Knockback] Called. hitDir={hitDir} knockbackForce={knockbackForce}", this);

        }

        // evitar multi-hit en el mismo swing
        if (hitCollider != null) hitCollider.enabled = false;
    }
}
