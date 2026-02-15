using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimRotation : MonoBehaviour
{
    [SerializeField] private Transform weaponPivot;

    private PlayerAimController aim;

    private void Awake()
    {
        aim = GetComponent<PlayerAimController>();

        if (weaponPivot == null)
        {
            Debug.LogError("[PlayerAimRotation] WeaponPivot is missing.");
            enabled = false;
            return;
        }

        Debug.Log("[PlayerAimRotation] Initialized");
    }

    private void OnEnable()
    {
        aim.OnAimDirectionChanged += OnAimChanged;
    }

    private void OnDisable()
    {
        aim.OnAimDirectionChanged -= OnAimChanged;
    }

    private void OnAimChanged(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        weaponPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
