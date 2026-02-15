using UnityEngine;

[RequireComponent(typeof(PlayerAimController))]
public class PlayerFlipController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private Transform weaponPivot;

    private PlayerAimController aimController;

    private void Awake()
    {
        aimController = GetComponent<PlayerAimController>();

        if (bodyRenderer == null || weaponPivot == null)
        {
            Debug.LogError("[PlayerFlipController] Missing references.");
            enabled = false;
            return;
        }

        Debug.Log("[PlayerFlipController] Initialized");
    }

    private void OnEnable()
    {
        aimController.OnAimDirectionChanged += HandleFlip;
    }

    private void OnDisable()
    {
        aimController.OnAimDirectionChanged -= HandleFlip;
    }

    private void HandleFlip(Vector2 aimDirection)
    {
        bool lookingRight = aimDirection.x >= 0f;

        // Flip body sprite
        bodyRenderer.flipX = !lookingRight;

        // Flip weapon vertically when aiming left
        Vector3 weaponScale = weaponPivot.localScale;
        weaponScale.y = lookingRight ? 1f : -1f;
        weaponPivot.localScale = weaponScale;
    }
}


