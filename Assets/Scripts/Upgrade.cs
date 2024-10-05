using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public enum UpgradeType
{
    MaxHealth,
    PickupMagnetRadius,
    MinDamage,
    MaxDamage,
    CritChance,
    FireRate,
    MaxMagazineCount,
    ReloadTime,
    WalkSpeed,
    SprintMultiplier,
    MaxSprintDuration,
    SprintRechargeRate
}

public class Upgrade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<UpgradeSO> OnUpgradeSelected;

    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeListText;
    [SerializeField] private Image upgradeIcon;

    private RectTransform rectTransform;
    private UpgradeController upgradeController;

    private UpgradeSO upgradeSO;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        upgradeController = GetComponentInParent<UpgradeController>();

        SetupUpgrade();

        XPController.OnLevelUpped += XPController_OnLevelUpped;
    }

    private void XPController_OnLevelUpped()
    {
        SetupUpgrade();
    }

    private void SetupUpgrade()
    {
        rectTransform.localScale = Vector3.one;

        upgradeSO = upgradeController.PickRandomUpgrade();

        upgradeNameText.text = upgradeSO.UpgradeType.ToString();
        upgradeListText.text = $"{upgradeSO.UpgradeType.ToString()} {upgradeSO.UpgradePercent * 100f}%";
    }

    public void SelectUpgrade()
    {
        Debug.Log("Selected " + upgradeSO.UpgradeType.ToString() + " upgrade.");
        OnUpgradeSelected?.Invoke(upgradeSO);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale = Vector3.one * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = Vector3.one;
    }
}
