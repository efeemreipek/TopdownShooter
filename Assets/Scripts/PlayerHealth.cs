using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private float healthBackgroundChangeSpeed;

    private PlayerUI playerUI;

    private bool canChangeBackground;

    private void Awake()
    {
        playerUI = GetComponent<PlayerUI>();

        PlayerStats.MaxHealth = maxHealth;
        print(PlayerStats.MaxHealth);

        currentHealth = maxHealth;
        playerUI.HealthBarFG.fillAmount = 1f;

        PickupController.OnPickupGathered += PickupController_OnPickupGathered;
        Upgrade.OnUpgradeSelected += Upgrade_OnUpgradeSelected;
    }

    private void Upgrade_OnUpgradeSelected(UpgradeSO upgradeState)
    {
        switch (upgradeState.UpgradeType)
        {
            case UpgradeType.MaxHealth:
                maxHealth += (int)(maxHealth * upgradeState.UpgradePercent);
                ChangeHealth(0);
                PlayerStats.MaxHealth = maxHealth;
                print(PlayerStats.MaxHealth);
                break;
            case UpgradeType.PickupMagnetRadius:
            case UpgradeType.MinDamage:
            case UpgradeType.MaxDamage:
            case UpgradeType.CritChance:
            case UpgradeType.FireRate:
            case UpgradeType.MaxMagazineCount:
            case UpgradeType.ReloadTime:
            case UpgradeType.WalkSpeed:
            case UpgradeType.SprintMultiplier:
            case UpgradeType.MaxSprintDuration:
            case UpgradeType.SprintRechargeRate:
            default:
                break;
        }
    }

    private void Update()
    {
        if (canChangeBackground)
        {
            playerUI.HealthBarChangeFG.fillAmount = Mathf.Lerp(
                                                   playerUI.HealthBarChangeFG.fillAmount,
                                                   playerUI.HealthBarFG.fillAmount,
                                                   healthBackgroundChangeSpeed * Time.deltaTime);

            if (Mathf.Abs(playerUI.HealthBarChangeFG.fillAmount - playerUI.HealthBarFG.fillAmount) < 0.01f)
            {
                playerUI.HealthBarChangeFG.fillAmount = playerUI.HealthBarFG.fillAmount;
                canChangeBackground = false;
            }
        }
    }

    private void PickupController_OnPickupGathered(PickupSO pickup)
    {
        if (pickup.PickupType != PickupType.Health) return;

        ChangeHealth(pickup.PickupAmount);
    }

    private void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        playerUI.HealthBarFG.fillAmount = (float)currentHealth / maxHealth;

        canChangeBackground = true;

        if (currentHealth <= 0)
        {
            //Player dead
        }
    }
}
