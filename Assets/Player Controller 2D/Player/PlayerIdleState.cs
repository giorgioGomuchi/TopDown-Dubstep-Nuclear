using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine fsm) : base(fsm) { }

    public override void Enter()
    {
        //Debug.Log("[Player] Enter Idle");
        // Ensure we stop motion while idle (optional but clean).
        fsm.Mover.SetVelocity(Vector2.zero);
    }

    public override void Tick()
    {
        if (fsm.Input.MoveInput.sqrMagnitude > 0.0001f)
        {
            fsm.ChangeState(fsm.MoveState);
        }
    }
}
