using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public event Action<string> OnStateChanged;

    public PlayerDataSO Data { get; private set; }
    public PlayerInputReader Input { get; private set; }
    public PlayerMover2D Mover { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }

    private PlayerState currentState;

    public PlayerStateMachine(PlayerDataSO data, PlayerInputReader input, PlayerMover2D mover)
    {
        Data = data;
        Input = input;
        Mover = mover;

        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
    }

    public void Initialize()
    {
        ChangeState(IdleState);
    }

    public void ChangeState(PlayerState newState)
    {
        if (newState == null || newState == currentState)
            return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();

        OnStateChanged?.Invoke(currentState.GetType().Name);
        //Debug.Log($"[Player] State changed to {currentState.GetType().Name}");
    }

    public void Tick()
    {
        currentState?.Tick();
    }

    public void FixedTick()
    {
        currentState?.FixedTick();
    }
}
