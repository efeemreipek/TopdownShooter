using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Transform firePointTransform;
    [SerializeField] private LeanGameObjectPool bulletObjectPool;
    [Space(10), Header("DAMAGE")]
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private bool isCritical;
    [Space(10), Header("AMMO")]
    [SerializeField] private int maxMagazineCount;
    [SerializeField] private int currentMagazineCount;
    [SerializeField] private int currentAmmo = 70;
    [Space(10), Header("RELOAD")]
    [SerializeField] private bool isReloading;
    [SerializeField] private float reloadTime;
    [SerializeField] private float fireRate;
    [Space(10), Header("CAMERA SHAKE")]
    [SerializeField] private float cameraShakeDuration;
    [SerializeField] private float cameraShakeAmount;
    [SerializeField] private Vector3 aimCameraOffset = new Vector3(0f, 12f, -1f);
    [SerializeField] private Vector3 normalCameraOffset = new Vector3(0f, 10f, 0f);

    private Timer timer;
    private CameraFollow cameraFollow;
    private Animator animator;
    private PlayerUI playerUI;

    private float timeSinceLastShot = 0f;
    private bool isPerfectReload;
    private int reloadedAmmo;

    private void Awake()
    {
        timer = GetComponent<Timer>();
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        animator = GetComponentInChildren<Animator>();
        playerUI = GetComponent<PlayerUI>();

        currentMagazineCount = maxMagazineCount;

        Bullet.OnBulletHit += Bullet_OnBulletHit;
        Timer.OnTimerEnd += Timer_OnTimerEnd;
        PickupController.OnPickupGathered += PickupController_OnPickupGathered;
        Upgrade.OnUpgradeSelected += Upgrade_OnUpgradeSelected;
    }

    private void Upgrade_OnUpgradeSelected(UpgradeSO upgradeState)
    {
        switch (upgradeState.UpgradeType)
        {
            case UpgradeType.MaxHealth:
            case UpgradeType.PickupMagnetRadius:
            case UpgradeType.MinDamage:
                minDamage += (int)(minDamage * upgradeState.UpgradePercent);
                break;
            case UpgradeType.MaxDamage:
                maxDamage += (int)(maxDamage * upgradeState.UpgradePercent);
                break;
            case UpgradeType.CritChance:

                break;
            case UpgradeType.FireRate:
                fireRate += fireRate * upgradeState.UpgradePercent;
                break;
            case UpgradeType.MaxMagazineCount:
                maxMagazineCount += (int)(maxMagazineCount * upgradeState.UpgradePercent);
                break;
            case UpgradeType.ReloadTime:
                reloadTime += reloadTime * upgradeState.UpgradePercent; 
                break;
            case UpgradeType.WalkSpeed:
            case UpgradeType.SprintMultiplier:
            case UpgradeType.MaxSprintDuration:
            case UpgradeType.SprintRechargeRate:
            default:
                break;
        }
    }

    private void PickupController_OnPickupGathered(PickupSO pickup)
    {
        if (pickup.PickupType != PickupType.Ammo) return;

        currentAmmo += pickup.PickupAmount;
        playerUI.CurrentAmmoText.text = currentAmmo.ToString("D4");
    }

    private void Start()
    {
        playerUI.CurrentMagazineText.text = currentMagazineCount.ToString("D2");
        playerUI.CurrentAmmoText.text = currentAmmo.ToString("D4");
    }
    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        animator.SetBool("isAiming", InputManager.Instance.GetIsAimPressed());

        if (InputManager.Instance.GetIsAimPressed())
        {
            cameraFollow.ChangeOffset(aimCameraOffset);
        }
        else
        {
            cameraFollow.ChangeOffset(normalCameraOffset);
        }

        if(InputManager.Instance.GetIsAimPressed() && (InputManager.Instance.GetIsFirePressed() || InputManager.Instance.GetIsFirePressedThisFrame()))
        {
            Shoot();
        }
        if (InputManager.Instance.GetIsReloadPressedThisFrame())
        {
            if (!isReloading)
            {
                isPerfectReload = false;
                StartReload();
            }
            else
            {
                if(timer.GetRemainingTimeNormalizedInverse() >= 0.5f && timer.GetRemainingTimeNormalizedInverse() <= 0.7f)
                {
                    isPerfectReload = true;
                    timer.StopTimer();
                }
            }
        }

        if (isReloading)
        {
            playerUI.ReloadTimerFG.fillAmount = timer.GetRemainingTimeNormalizedInverse();
        }
    }

    private void StartReload()
    {
        if(!isReloading && currentMagazineCount < maxMagazineCount && currentAmmo > 0)
        {
            reloadedAmmo = Mathf.Clamp(maxMagazineCount - currentMagazineCount, 0, currentAmmo);

            playerUI.ReloadTimerGO.SetActive(true);
            timer.StartTimer(reloadTime);
            animator.SetTrigger("Start Reload");
            AudioManager.Instance.PlayReloadStartAudio(0.7f);
            isReloading = true;
        }
    }
    private void Shoot()
    {
        if(currentMagazineCount > 0 && CanShoot())
        {
            GameObject bulletGO = bulletObjectPool.Spawn(firePointTransform.position, Quaternion.identity, bulletObjectPool.transform);
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            if(bullet == null) bullet = bulletGO.AddComponent<Bullet>();

            CameraShake.Instance.Shake(cameraShakeDuration, cameraShakeAmount);
            AudioManager.Instance.PlayRandomWeaponAudio();

            int damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);
            bullet.SetDamage(damage);

            isCritical = damage == maxDamage;
            bullet.SetIsCriticalHit(isCritical);

            bullet.FireBullet(firePointTransform.forward);

            currentMagazineCount--;
            playerUI.CurrentMagazineText.text = currentMagazineCount.ToString("D2");

            timeSinceLastShot = 0f;
        }
    }
    private void Timer_OnTimerEnd()
    {
        currentMagazineCount += reloadedAmmo;
        playerUI.CurrentMagazineText.text = currentMagazineCount.ToString("D2");

        currentAmmo -= reloadedAmmo;
        playerUI.CurrentAmmoText.text = currentAmmo.ToString("D4");

        isReloading = false;

        if(isPerfectReload) AudioManager.Instance.PlayReloadPerfectAudio(0.7f);
        else AudioManager.Instance.PlayReloadEndAudio(0.7f);

        animator.SetTrigger("End Reload");

        playerUI.ReloadTimerGO.SetActive(false);
    }
    private void Bullet_OnBulletHit(GameObject gameObject, float time)
    {
        bulletObjectPool.Despawn(gameObject, time);
    }
    private bool CanShoot() => !isReloading && timeSinceLastShot > 1f / (fireRate / 60f);
}
