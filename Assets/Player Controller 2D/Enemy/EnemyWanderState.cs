using UnityEngine;

public class EnemyWanderState : MonoBehaviour
{
    private EnemyController enemy;

    private Vector2 target;
    private float timer;

    private void Awake()
    {
        enemy = GetComponent<EnemyController>();
    }

    public void Enter()
    {
        timer = enemy.data.wanderTime;

        target = (Vector2)enemy.transform.position +
                 Random.insideUnitCircle.normalized * Random.Range(0.5f, enemy.data.wanderRadius);
    }

    public void Tick()
    {
        if (enemy.PlayerInRange(enemy.data.chaseRange))
        {
            enemy.SetMode(EnemyController.EnemyMode.Chase);
            return;
        }

        timer -= Time.deltaTime;

        enemy.MoveTowards(target, enemy.data.moveSpeed);

        if (Vector2.Distance(enemy.transform.position, target) < 0.2f || timer <= 0f)
        {
            enemy.SetMode(EnemyController.EnemyMode.Idle);
        }
    }
}
