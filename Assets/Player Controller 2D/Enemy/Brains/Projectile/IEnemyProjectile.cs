using UnityEngine;

public interface IEnemyProjectile
{
    void Initialize(
        Vector2 direction,
        float speed,
        int damage,
        LayerMask targetLayer
    );
}
