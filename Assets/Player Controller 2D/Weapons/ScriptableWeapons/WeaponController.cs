using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private WeaponDataSO weaponData;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private Animator weaponAnimator;


    private PlayerAimController aim;
    private SpriteRenderer spriteRenderer;
    private bool canAttack = true;

    private static readonly int ATTACK_HASH = Animator.StringToHash("Attack");

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        aim = GetComponentInParent<PlayerAimController>();

        if (weaponData == null || aim == null)
        {
            Debug.LogError("[WeaponController] Missing references.");
            enabled = false;
            return;
        }

        SetupWeapon();
        Debug.Log("[WeaponController] Initialized with " + weaponData.weaponName);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;
        switch (weaponData)
        {
            case RangedWeaponDataSO ranged:
                FireRanged(ranged);
                break;

            case MeleeTrailWeaponDataSO meleeTrail:
                yield return MeleeTrailAttack(meleeTrail);
                break;

            case MeleeAnimatedWeaponDataSO meleeAnim:
                yield return MeleeAnimatedAttack(meleeAnim);
                break;
        }

        yield return new WaitForSeconds(weaponData.cooldown);
        canAttack = true;
    }

    // ---------------- RANGED ----------------
    private void FireRanged(RangedWeaponDataSO data)
    {
        //TODO OBJECT PULL CREAR MANAGER EN OTRA CLASE
        GameObject proj = Instantiate(
            data.projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Initialize(aim.AimDirection, data.projectileSpeed, data.targetLayer);

        if (data is ExplosiveWeaponDataSO explosiveData)
        {
            ExplosiveProjectile explosive = proj.GetComponent<ExplosiveProjectile>();

            if (explosive != null)
            {
                explosive.ConfigureExplosion(
                    explosiveData.explosionRadius,
                    explosiveData.explosionDamage,
                    explosiveData.explosionForce,
                    explosiveData.explosionPrefab
                );
            }
            else
            {
                Debug.LogWarning("ExplosiveWeaponData pero el prefab no tiene ExplosiveProjectile");
            }
        }

        TriggerCameraShake();
    }


    // ---------------- MELEE TRAIL ----------------
    private IEnumerator MeleeTrailAttack(MeleeTrailWeaponDataSO data)
    {
        if (trail == null)
            yield break;

        trail.Clear();
        trail.emitting = true;

        float elapsed = 0f;
        float halfAngle = data.swingAngle * 0.5f;

        while (elapsed < data.swingDuration)
        {
            float t = elapsed / data.swingDuration;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);

            transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        trail.emitting = false;
        transform.localRotation = Quaternion.identity;

        Debug.Log("[WeaponController] Melee trail attack");
    }

    // ---------------- MELEE ANIMATED ----------------
    private IEnumerator MeleeAnimatedAttack(MeleeAnimatedWeaponDataSO data)
    {
        if (data == null || data.hitPrefab == null)
        {
            Debug.LogWarning("[WeaponController] MeleeAnimatedAttack missing data or hitPrefab");
            yield break;
        }

        // Weapon visual feedback (optional)
        if (weaponAnimator != null && data.attackAnimation != null)
        {
            weaponAnimator.Play(data.attackAnimation.name, 0, 0f);
        }

        // Spawn hit as child of WeaponPivot (parent of firePoint)
        Transform weaponPivot = firePoint != null ? firePoint.parent : transform.parent;

        if (weaponPivot == null)
        {
            Debug.LogError("[WeaponController] weaponPivot not found (firePoint.parent is null)");
            yield break;
        }

        GameObject hit = Instantiate(data.hitPrefab, weaponPivot);

        // WeaponPivot already aims; push forward on +X
        hit.transform.localPosition = Vector3.right * data.range;
        hit.transform.localRotation = Quaternion.Euler(0f, 0f, data.spriteAngleOffset);

        if (data.feedbackRotationDuration > 0f)
        {
            StartCoroutine(RotateWeaponFeedback(data.feedbackRotationAngle, data.feedbackRotationDuration));
        }

        var hitController = hit.GetComponent<MeleeHitController>();
        if (hitController != null)
        {
            hitController.Initialize(data.damage, data.targetLayer, data.hitLifetime, data.knockbackForce);
        }


        else
        {
            Debug.LogError("[WeaponController] MeleeHit prefab has NO MeleeHitController");
        }

        TriggerCameraShake();

        // IMPORTANT: No cooldown wait here!
        yield return null;
    }




    private IEnumerator RotateWeaponFeedback(float angle,float duration)
    {
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation =
            startRotation * Quaternion.Euler(0f, 0f, angle);

        float t = 0f;

        // Rotate towards attack
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        t = 0f;

        // Return to original rotation
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localRotation = Quaternion.Lerp(targetRotation, startRotation, t);
            yield return null;
        }
    }

    private void TriggerCameraShake()
    {
        if (weaponData == null) return;

        if (weaponData.cameraShakeDuration <= 0f ||
            weaponData.cameraShakeStrength <= 0f)
            return;

        CameraShakeProvider.Instance?.Shake(
            weaponData.cameraShakeDuration,
            weaponData.cameraShakeStrength
        );
    }


    // ---------------- SETUP ----------------
    private void SetupWeapon()
    {
        spriteRenderer.sprite = weaponData.weaponIcon;

        if (weaponData is MeleeTrailWeaponDataSO trailWeapon && trail != null)
        {
            trail.startWidth = trailWeapon.trailWidth;
            trail.emitting = false;
        }

        if (weaponData is MeleeAnimatedWeaponDataSO animated && weaponAnimator != null)
        {
            var overrideController =
                new AnimatorOverrideController(weaponAnimator.runtimeAnimatorController);

            overrideController["Attack"] = animated.attackAnimation;
            weaponAnimator.runtimeAnimatorController = overrideController;
        }
    }
}

