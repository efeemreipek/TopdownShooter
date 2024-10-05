using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    XP,
    Health,
    Ammo
}

[CreateAssetMenu]
public class PickupSO : ScriptableObject
{
    public PickupType PickupType;
    public Material PickupMaterial;
    public int PickupAmount = 1;
}
