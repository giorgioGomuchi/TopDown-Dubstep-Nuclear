using System;
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

        // Rotate projectile to face movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Invoke(nameof(OnLifeTimeEnded), lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("Trigger with: " + other.name);

        if (!initialized)
            return;

        if (((1 << other.gameObject.layer) & targetLayer) == 0)
            return;

        OnHit(other);
    }

    protected virtual void OnLifeTimeEnded()
    {
        Destroy(gameObject);
    }


    protected virtual void OnHit(Collider2D other)
    {
        Vector2 hitDirection =
            (other.transform.position - transform.position).normalized;

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }




}


