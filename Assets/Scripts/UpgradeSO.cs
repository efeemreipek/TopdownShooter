using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UpgradeSO : ScriptableObject
{
    public UpgradeType UpgradeType;
    [Range(-1f, 1f)] public float UpgradePercent;
}
