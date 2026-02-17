using UnityEngine;

public class EnemyProjectile : BaseProjectile
{
    private bool reflected;

    public Vector2 CurrentDirection => direction;

    public void Initialize(Vector2 dir, float projectileSpeed, int dmg, LayerMask target)
    {
        direction = dir.normalized;
        speed = projectileSpeed;
        damage = dmg;
        targetLayer = target;

        RotateToDirection();
    }

    public void Reflect(Vector2 newDirection)
    {
        reflected = true;

        direction = newDirection.normalized;
        RotateToDirection();

        // Ahora debe golpear enemigos
        targetLayer = LayerMask.GetMask("Enemy");
    }

    protected override void OnHit(Collider2D other)
    {
        if (reflected)
        {
            var enemy = other.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }
        else
        {
            var player = other.GetComponentInParent<PlayerHealth>();
            if (player != null)
                player.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
