using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPController : MonoBehaviour
{
    public static event Action OnLevelUpped;

    [SerializeField] private int firstLevelXPAmount = 5;

    private PlayerUI playerUI;

    private int xpLevel;
    private int xpAmount;
    private int currentLevelXPAmount;

    private void Awake()
    {
        playerUI = GetComponent<PlayerUI>();

        currentLevelXPAmount = firstLevelXPAmount;
        playerUI.XPBarFG.fillAmount = 0f;
        playerUI.XPLevelText.text = xpLevel.ToString();

        PickupController.OnPickupGathered += PickupController_OnPickupGathered;
        Upgrade.OnUpgradeSelected += Upgrade_OnUpgradeSelected;
    }

    private void Upgrade_OnUpgradeSelected(UpgradeSO upgradeState)
    {
        Time.timeScale = 1f;
        playerUI.LevelUpgradePanel.SetActive(false);
    }

    private void PickupController_OnPickupGathered(PickupSO pickup)
    {
        if (pickup.PickupType != PickupType.XP) return;

        xpAmount += pickup.PickupAmount;

        playerUI.XPBarFG.fillAmount = (float)xpAmount / currentLevelXPAmount;

        if(xpAmount == currentLevelXPAmount)
        {
            xpAmount = 0;
            currentLevelXPAmount *= 2;
            xpLevel++;
            OnLevelUpped?.Invoke();
            Time.timeScale = Mathf.Epsilon;
            playerUI.LevelUpgradePanel.SetActive(true);

            playerUI.XPBarFG.fillAmount = (float)xpAmount / currentLevelXPAmount;
            playerUI.XPLevelText.text = xpLevel.ToString();
        }

    }
}
