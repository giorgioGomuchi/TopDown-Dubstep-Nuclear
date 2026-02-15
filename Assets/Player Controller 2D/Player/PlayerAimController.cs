using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    public event Action<Vector2> OnAimDirectionChanged;

    [Header("Debug")]
    [SerializeField] private bool drawDebug = true;

    [Header("References")]
    [SerializeField] private Transform firePoint;

    private Camera mainCamera;
    private Vector2 aimDirection;

    public Vector2 AimDirection => aimDirection;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (firePoint == null)
        {
            Debug.LogError("[PlayerAimController] FirePoint reference is missing.");
            enabled = false;
            return;
        }

        Debug.Log("[PlayerAimController] Initialized");
    }

    private void Update()
    {
        UpdateAimDirection();
    }

    private void UpdateAimDirection()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;

        if (direction != aimDirection)
        {
            aimDirection = direction;
            OnAimDirectionChanged?.Invoke(aimDirection);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawDebug) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, firePoint.position + (Vector3)aimDirection * 3);
    }
}
