using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/Weapon Modifiers/Explosion", fileName = "WM_Explosion")]
public class ExplosionModifierSO : WeaponModifierSO
{
    [Header("Explosion")]
    public float radius = 2.5f;
    public int explosionDamage = 3;
    public float explosionForce = 0f;
    public GameObject explosionPrefab;

    public override void AfterSpawnedProjectile(ref WeaponFireContext ctx, GameObject projectileInstance)
    {
        var explosive = projectileInstance.GetComponent<ExplosiveProjectile>();
        if (explosive == null) return;

        explosive.ConfigureExplosion(radius, explosionDamage, explosionForce, explosionPrefab);
    }
}
