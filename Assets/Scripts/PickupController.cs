using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    public static event Action<PickupSO> OnPickupGathered;

    [SerializeField] private float pickupRadius = 1f;

    private int pickupLayerMask;
    private Collider[] pickupColliders;

    private void Awake()
    {
        pickupLayerMask = LayerMask.GetMask("Pickup");

        Upgrade.OnUpgradeSelected += Upgrade_OnUpgradeSelected;
    }

    private void Upgrade_OnUpgradeSelected(UpgradeSO upgradeType)
    {
        switch (upgradeType.UpgradeType)
        {
            case UpgradeType.MaxHealth:
            case UpgradeType.PickupMagnetRadius:
                pickupRadius += pickupRadius * upgradeType.UpgradePercent;
                break;
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
        pickupColliders = Physics.OverlapSphere(transform.position, pickupRadius, pickupLayerMask);

        if(pickupColliders.Length > 0)
        {
            foreach(Collider collider in pickupColliders)
            {
                collider.GetComponent<Pickup>().AttractPickup(transform.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Pickup pickup))
        {
            Debug.Log("Pickup Gathered: " + pickup.PickupSO.PickupType.ToString() + " " + pickup.PickupSO.PickupAmount);
            OnPickupGathered?.Invoke(pickup.PickupSO);
            AudioManager.Instance.PlayPickupGatherAudio();
            //Destroy(pickup.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
