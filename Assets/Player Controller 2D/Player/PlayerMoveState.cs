using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerStateMachine fsm) : base(fsm) { }

    public override void Enter()
    {
        //Debug.Log("[Player] Enter Move");
    }

    public override void Tick()
    {
        if (fsm.Input.MoveInput.sqrMagnitude <= 0.0001f)
        {   
            fsm.ChangeState(fsm.IdleState);
        }
    }

    public override void FixedTick()
    {
        Vector2 desiredVelocity = fsm.Input.MoveInput * fsm.Data.moveSpeed;
        fsm.Mover.SetVelocity(desiredVelocity);
    }

    public override void Exit()
    {
        // Stop when leaving move to avoid drift.
        fsm.Mover.SetVelocity(Vector2.zero);
    }
}
