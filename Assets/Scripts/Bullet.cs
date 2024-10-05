using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //public static event Action<Bullet> OnBulletHit; 
    public static event Action<GameObject, float> OnBulletHit; 

    [SerializeField] private float speed = 5f;
    [SerializeField] private float despawnTime = 2f;

    private TrailRenderer trailRenderer;

    private int damage;
    private bool isCriticalHit;
    private bool isHit = false;
    private bool isInstantiated = false;
    private WaitForSeconds wait;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        wait = new WaitForSeconds(despawnTime);
    }
    private void OnEnable()
    {
        isHit = false;
        GetComponent<MeshRenderer>().enabled = true;
    }
    private IEnumerator DespawnAfterTime()
    {
        yield return wait;
        if (!isHit)
        {
            trailRenderer.Clear();
            isInstantiated = false;
            OnBulletHit?.Invoke(gameObject, 0f);
        }
    }
    private void Update()
    {
        if (!isInstantiated) return;

        Ray ray = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, speed * Time.deltaTime))
        {
            if (hit.collider.TryGetComponent(out IDamagable damagable))
            {
                isHit = true;
                transform.position = hit.point;
                if (damagable.GetCurrentHealth() > 0)
                {
                    damagable.TakeDamage(damage);
                }

                GetComponent<MeshRenderer>().enabled = false;
                trailRenderer.Clear();
                isInstantiated = false;
                OnBulletHit?.Invoke(gameObject, 0f);
            }
        }
        else
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
    public void FireBullet(Vector3 direction)
    {
        transform.forward = direction;
        isInstantiated = true;
        StartCoroutine(DespawnAfterTime());
    }
    public void SetDamage(int damage) => this.damage = damage;
    public void SetIsCriticalHit(bool isCriticalHit) => this.isCriticalHit = isCriticalHit;
}
