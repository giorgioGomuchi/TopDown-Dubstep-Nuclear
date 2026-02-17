using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    private float explosionRadius;
    private int explosionDamage;
    private float explosionForce;
    private GameObject explosionPrefab;

    private bool exploded;

    public void ConfigureExplosion(float radius, int damage, float force, GameObject prefab)
    {
        explosionRadius = radius;
        explosionDamage = damage;
        explosionForce = force;
        explosionPrefab = prefab;
    }

    protected override void OnHit(Collider2D other)
    {
        Explode();
    }

    protected override void OnLifeTimeEnded()
    {
        Explode();
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Daño solo a targetLayer
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayer);

        foreach (Collider2D hit in hits)
        {
            var damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(explosionDamage);

            // Knockback opcional (solo si hay Rigidbody2D)
            if (hit.attachedRigidbody != null && explosionForce > 0f)
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                hit.attachedRigidbody.AddForce(dir * explosionForce, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
