using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMover2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerDataSO playerData;

    public event Action<Vector2> OnMoveInput;      // For UI/Audio/FX if needed.
    public event Action<string> OnPlayerStateChanged;

    private PlayerStateMachine fsm;
    private PlayerInputReader input;
    private PlayerMover2D mover;

    private void Awake()
    {
        if (playerData == null)
        {
            Debug.LogError("[Player] PlayerDataSO is missing!");
            enabled = false;
            return;
        }

        input = GetComponent<PlayerInputReader>();
        mover = GetComponent<PlayerMover2D>();

        fsm = new PlayerStateMachine(playerData, input, mover);
        fsm.OnStateChanged += HandleStateChanged;

        Debug.Log("[Player] Controller initialized");
    }

    private void Start()
    {


        fsm.Initialize();
    }

    private void Update()
    {
        // Broadcast input if external listeners want it (optional).
        OnMoveInput?.Invoke(input.MoveInput);

        fsm.Tick();
    }

    private void FixedUpdate()
    {
        fsm.FixedTick();
    }

    private void HandleStateChanged(string stateName)
    {
        OnPlayerStateChanged?.Invoke(stateName);
    }

    private void OnDestroy()
    {
        if (fsm != null)
            fsm.OnStateChanged -= HandleStateChanged;
    }
}
