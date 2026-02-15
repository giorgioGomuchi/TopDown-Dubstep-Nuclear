using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Player Data", fileName = "PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Movement")]
    [Min(0f)] public float moveSpeed = 8f;
}
