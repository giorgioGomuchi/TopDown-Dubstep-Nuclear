using UnityEngine;

public class EnemySniperShootState : MonoBehaviour
{
    private EnemyController enemy;

    private enum SniperState
    {
        Aim,
        Burst,
        Cooldown
    }

    private SniperState currentState;

    private float aimTimer;
    private float blinkTimer;
    private float burstTimer;
    private float cooldownTimer;

    private int shotsFired;
    private bool laserVisible;

    private void Awake()
    {
        enemy = GetComponent<EnemyController>();
    }

    public void Enter()
    {
        currentState = SniperState.Aim;

        aimTimer = 0f;
        blinkTimer = 0f;
        shotsFired = 0;

        EnableLaser(true);
    }

    public void Tick()
    {
        if (enemy.player == null || enemy.data == null)
        {
            enemy.SetMode(EnemyController.EnemyMode.Idle);
            return;
        }

        switch (currentState)
        {
            case SniperState.Aim:
                UpdateAim();
                break;

            case SniperState.Burst:
                UpdateBurst();
                break;

            case SniperState.Cooldown:
                UpdateCooldown();
                break;
        }
    }

    // =========================
    // AIM
    // =========================

    private void UpdateAim()
    {
        AimToPlayer();
        UpdateLaserPositions();

        // Blink
        blinkTimer -= Time.deltaTime;
        if (blinkTimer <= 0f)
        {
            blinkTimer = Mathf.Max(0.01f, enemy.data.sniperBlinkInterval);
            laserVisible = !laserVisible;
            EnableLaser(laserVisible);
        }

        aimTimer += Time.deltaTime;

        if (aimTimer >= enemy.data.sniperAimDuration)
        {
            currentState = SniperState.Burst;
            burstTimer = 0f;
            shotsFired = 0;
            EnableLaser(true);
        }
    }

    // =========================
    // BURST
    // =========================

    private void UpdateBurst()
    {
        burstTimer -= Time.deltaTime;

        if (burstTimer > 0f)
            return;

        FireOneProjectile(shotsFired, enemy.data.sniperProjectileCount);
        shotsFired++;

        if (shotsFired >= enemy.data.sniperProjectileCount)
        {
            DisableLaser();
            ResetWeaponIdlePose();

            currentState = SniperState.Cooldown;
            cooldownTimer = enemy.data.attackCooldown;
            return;
        }

        burstTimer = enemy.data.sniperTimeBetweenShots;
    }

    // =========================
    // COOLDOWN
    // =========================

    private void UpdateCooldown()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer > 0f)
            return;

        if (enemy.PlayerInRange(enemy.data.viewDistance))
            Enter(); // volver a Aim
        else
            enemy.SetMode(EnemyController.EnemyMode.Wander);
    }

    // =========================
    // SHOOT
    // =========================

    private void FireOneProjectile(int index, int total)
    {
        if (enemy.data.sniperProjectilePrefab == null || enemy.firePoint == null)
            return;

        Vector2 baseDir = enemy.DirectionToPlayer();
        if (baseDir == Vector2.zero) return;

        float spread = enemy.data.sniperSpreadAngle;
        float angleOffset = 0f;


        //TODO: INTERPOLA BIEN BOBO
        if (total > 1)
        {
            float t = (float)index / (total - 1);
            angleOffset = Mathf.Lerp(-spread * 0.5f, spread * 0.5f, t);
        }

        Vector2 dir = Rotate2D(baseDir, angleOffset);

        GameObject proj = Instantiate(
            enemy.data.sniperProjectilePrefab,
            enemy.firePoint.position,
            Quaternion.identity
        );

        var enemyProj = proj.GetComponent<EnemyProjectile>();
        if (enemyProj != null)
        {
            enemyProj.Initialize(
                dir,
                enemy.data.sniperProjectileSpeed,
                (int)enemy.data.damage,
                enemy.data.targetLayerProyectile
            );

            //Se instancia por algun motivo en dandole valor a la  posicion z ñapa para forzarla a cero
            Vector3 pos = proj.transform.position;
            pos.z = 0f;
            proj.transform.position = pos;
        }
    }

    // =========================
    // AIM ROTATION
    // =========================

    private void AimToPlayer()
    {
        if (enemy.weaponPivot == null) return;

        Vector2 dir = enemy.DirectionToPlayer();
        if (dir == Vector2.zero) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        enemy.weaponPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        if (dir.x < 0)
            enemy.weaponPivot.localScale = new Vector3(1f, -1f, 1f);
        else
            enemy.weaponPivot.localScale = new Vector3(1f, 1f, 1f);
    }

    private void ResetWeaponIdlePose()
    {
        if (enemy.weaponPivot == null) return;

        bool facingLeft = enemy.weaponPivot.localScale.y < 0f;

        enemy.weaponPivot.rotation = facingLeft
            ? Quaternion.Euler(0f, 0f, 180f)
            : Quaternion.Euler(0f, 0f, 0f);
    }

    // =========================
    // LASER
    // =========================

    private void EnableLaser(bool enabled)
    {
        if (enemy.lineRenderer != null)
            enemy.lineRenderer.enabled = enabled;
    }

    private void DisableLaser()
    {
        EnableLaser(false);
    }

    private void UpdateLaserPositions()
    {
        if (enemy.lineRenderer == null || enemy.firePoint == null || enemy.player == null)
            return;

        enemy.lineRenderer.SetPosition(0, enemy.firePoint.position);
        enemy.lineRenderer.SetPosition(1, enemy.player.position);
    }

    private static Vector2 Rotate2D(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);

        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }
}
