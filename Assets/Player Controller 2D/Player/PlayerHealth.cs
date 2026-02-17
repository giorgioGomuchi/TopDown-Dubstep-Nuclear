using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 10f;

    [SerializeField] private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        //Debug.Log($"[Player] Health initialized: {currentHealth}");
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"[Player] Took {amount} damage. HP = {currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }


    private void Die()
    {
        //Debug.Log("[Player] Died");
        // aquí luego:
        // - animación
        // - restart
        // - baksdjaklsdja`s
    }
}
