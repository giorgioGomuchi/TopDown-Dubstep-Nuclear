using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    protected readonly PlayerStateMachine fsm;

    protected PlayerState(PlayerStateMachine fsm)
    {
        this.fsm = fsm;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }        // Per-frame logic
    public virtual void FixedTick() { }   // Physics logic
}