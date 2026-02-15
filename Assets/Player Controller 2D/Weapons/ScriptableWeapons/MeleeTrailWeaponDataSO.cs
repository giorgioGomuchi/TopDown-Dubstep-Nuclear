using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Weapons/Melee Trail")]
public class MeleeTrailWeaponDataSO : MeleeWeaponDataSO
{
    [Header("Trail")]
    public float trailWidth = 0.25f;
    public Gradient trailColor;
}
