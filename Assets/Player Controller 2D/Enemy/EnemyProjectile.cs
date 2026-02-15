using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector2 dir;
    private float speed;
    private int damage;
    private LayerMask targetLayer;

    private bool reflected;

    //ojo variable puclica cuidao
    public Vector2 CurrentDirection => dir;


    public void Initialize(Vector2 direction, float projectileSpeed, int dmg, LayerMask target)
    {
        dir = direction.normalized;
        transform.right = dir;
        speed = projectileSpeed;
        damage = dmg;
        targetLayer = target;
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }



    public void Reflect(Vector2 newDirection)
    {
        reflected = true;
        dir = newDirection.normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

       
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (reflected)
        {
            // Ahora daña enemigos
            var enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Destroy(gameObject);
            }
            return;
        }

        // comportamiento normal
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(damage);

        Destroy(gameObject);
    }

}
