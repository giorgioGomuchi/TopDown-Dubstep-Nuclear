using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    private float explosionRadius;
    private int explosionDamage;
    private float explosionForce;
    private GameObject explosionPrefab;

    public void ConfigureExplosion(
    float radius,
    int damage,
    float force,
    GameObject explosionPrefab)
    {
        this.explosionRadius = radius;
        this.explosionDamage = damage;
        this.explosionForce = force;
        this.explosionPrefab = explosionPrefab;
    }


    

    protected override void OnHit(Collider2D other)
    {
        Debug.Log("OnHit triggered with: " + other.name);
        Explode();
    }

    private void Explode()
    {
        Debug.Log("EXPLODE called at: " + transform.position);

        if (explosionPrefab == null)
            Debug.LogWarning("Explosion prefab is NULL");

        if (explosionRadius <= 0)
            Debug.LogWarning("Explosion radius is ZERO");

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Debug.LogWarning("Explosion radius is ZERO");


        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius,
            targetLayer
        );

        Debug.Log("Objects hit in explosion: " + hits.Length);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Explosion affecting: " + hit.name);

            var damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage);
                Debug.Log("Damage applied: " + explosionDamage);
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
