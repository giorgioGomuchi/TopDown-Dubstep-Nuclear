using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float speed = 6f;
    [SerializeField] private int damage = 1;

    private Vector2 direction;
    protected LayerMask targetLayer;
    private bool initialized;

    public void Initialize(Vector2 direction, float speed, LayerMask targetLayer)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        this.targetLayer = targetLayer;

        initialized = true;

        // Si tu sprite está "vertical hacia arriba" y quieres que apunte con la punta:
        // Usa +90/-90 según cómo esté dibujado.
        float angle = Mathf.Atan2(this.direction.y, this.direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Invoke(nameof(OnLifeTimeEnded), lifeTime);
    }

    private void Update()
    {
        if (!initialized) return;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized) return;

        // IMPORTANTE: impacta con cualquiera (pared/suelo/enemigo), decide OnHit qué hacer
        OnHit(other);
    }

    protected virtual void OnLifeTimeEnded()
    {
        Destroy(gameObject);
    }

    protected virtual void OnHit(Collider2D other)
    {
        // Solo hace daño si está en targetLayer
        if (((1 << other.gameObject.layer) & targetLayer) == 0)
        {
            Destroy(gameObject);
            return;
        }

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
