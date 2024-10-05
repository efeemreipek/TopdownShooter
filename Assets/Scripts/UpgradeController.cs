using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    public List<UpgradeSO> Upgrades = new List<UpgradeSO>();

    private static List<UpgradeSO> lastPickedUpgradesList = new List<UpgradeSO>();

    public UpgradeSO PickRandomUpgrade()
    {
        if(lastPickedUpgradesList.Count == 3)
        {
            lastPickedUpgradesList.Clear();
        }

        int index = Random.Range(0, Upgrades.Count);

        UpgradeSO upgradeSO = Upgrades[index];
        if (!lastPickedUpgradesList.Contains(upgradeSO))
        {
            lastPickedUpgradesList.Add(upgradeSO);
            return upgradeSO;
        }
        else
        {
            return PickRandomUpgrade();
        }
        
    }
}
