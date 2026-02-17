using UnityEngine;

public class EnemyBazookaWeapon : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private ExplosiveWeaponDataSO weaponData;

    public void Fire(Vector2 direction)
    {
        if (weaponData == null)
        {
            Debug.LogWarning("[EnemyBazookaWeapon] weaponData is NULL", this);
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("[EnemyBazookaWeapon] firePoint is NULL", this);
            return;
        }

        GameObject proj = Instantiate(
            weaponData.projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        ExplosiveProjectile explosive = proj.GetComponent<ExplosiveProjectile>();
        if (explosive == null)
        {
            Debug.LogError("[EnemyBazookaWeapon] Projectile prefab has NO ExplosiveProjectile.", proj);
            Destroy(proj);
            return;
        }

        // Configura explosión
        explosive.ConfigureExplosion(
            weaponData.explosionRadius,
            weaponData.explosionDamage,
            weaponData.explosionForce,
            weaponData.explosionPrefab
        );

        // Inicializa movimiento + layer de objetivos (quién recibe daño)
        explosive.Initialize(
            direction,
            weaponData.projectileSpeed,
            weaponData.targetLayer
        );
    }
}
