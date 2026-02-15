using UnityEngine;

public class EnemyChaseState : MonoBehaviour
{
    private EnemyController enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyController>();
    }

    public void Enter()
    {
        // Nada especial
    }

   public void Tick()
{
    if (!enemy.PlayerInRange(enemy.data.chaseRange))
    {
        enemy.SetMode(EnemyController.EnemyMode.Idle);
        return;
    }

        

        float dist = Vector2.Distance(
        enemy.transform.position,
        enemy.player.position
    );

    if (dist <= enemy.data.stopDistance)
    {
        enemy.StopMovement();
        enemy.TryAttack();
    }
    else
    {
        Vector2 dir = enemy.DirectionToPlayer();
        enemy.Move(dir, enemy.data.chaseSpeed);

            Debug.DrawRay(
               enemy.transform.position,
               dir * 1.5f,
               Color.cyan
           );
        }
}

}
