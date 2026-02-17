using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    protected Vector2 direction;
    protected float speed;
    protected int damage;
    protected LayerMask targetLayer;

    [SerializeField] protected float collisionRadius = 0.15f;
    [SerializeField] protected float lifetime = 5f;

    protected virtual void Start()
    {
        Destroy(gameObject, lifetime);
    }

    protected virtual void Update()
    {
        Move();
        CheckCollision();
    }

    protected void Move()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    protected void CheckCollision()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            collisionRadius,
            targetLayer
        );

        if (hit != null)
        {
            OnHit(hit);
        }
    }

    protected abstract void OnHit(Collider2D other);

    protected void RotateToDirection()
    {
        transform.right = direction;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collisionRadius);
    }
}
