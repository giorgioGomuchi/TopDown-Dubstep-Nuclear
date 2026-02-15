using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    private void Update()
    {
        // Simple legacy input (KISS).
        MoveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        // Normalize to avoid faster diagonal movement.
        if (MoveInput.sqrMagnitude > 1f)
            MoveInput = MoveInput.normalized;
    }
}
