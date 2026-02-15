using UnityEngine;

public class EnemyIdleState : MonoBehaviour
{
    private EnemyController enemy;
    private float timer;

    private void Awake()
    {
        enemy = GetComponent<EnemyController>();
    }

    public void Enter()
    {
        timer = enemy.data.idleTime;
        enemy.StopMovement();
    }

    public void Tick()
    {
        if (enemy.PlayerInRange(enemy.data.chaseRange))
        {
            enemy.SetMode(EnemyController.EnemyMode.Chase);
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // más peso a WANDER
            float roll = Random.value;
            if (roll <= enemy.data.wanderChance)
                enemy.SetMode(EnemyController.EnemyMode.Wander);
            else
                Enter(); // seguir Idle un rato más
        }
    }
}
