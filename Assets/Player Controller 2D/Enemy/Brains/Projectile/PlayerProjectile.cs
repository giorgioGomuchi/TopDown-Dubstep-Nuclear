using UnityEngine;

public class PlayerProjectile : BaseProjectile
{
    public void Initialize(Vector2 dir, float projectileSpeed, int dmg, LayerMask target)
    {
        direction = dir.normalized;
        speed = projectileSpeed;
        damage = dmg;
        targetLayer = target;

        RotateToDirection();
    }

    protected override void OnHit(Collider2D other)
    {
        var enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
