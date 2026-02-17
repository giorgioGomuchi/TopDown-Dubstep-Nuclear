using UnityEngine;

public class EnemyExplosiveProjectile : EnemyProjectile
{
    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private int explosionDamage = 3;
    [SerializeField] private GameObject explosionPrefab;

    protected override void OnHit(Collider2D other)
    {
        Explode();
    }

    private void Explode()
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius,
            LayerMask.GetMask("Player")
        );

        foreach (var hit in hits)
        {
            var player = hit.GetComponentInParent<PlayerHealth>();
            if (player != null)
                player.TakeDamage(explosionDamage);
        }

        Destroy(gameObject);
    }
}
