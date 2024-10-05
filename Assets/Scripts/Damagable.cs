using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour, IDamagable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int pickupAmount;

    private void Awake()
    {
        currentHealth = maxHealth;        
    }

    public int GetCurrentHealth() => currentHealth;
    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        if(currentHealth <= 0)
        {
            //Spawn pickups
            for (int i = 0; i < pickupAmount; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
                spawnPos.y = 0.25f;
                var pickup = Instantiate(GameAssets.Instance.PickupPrefab, spawnPos, Quaternion.identity);
                pickup.GetComponent<Pickup>().Setup();
            }

            Destroy(gameObject);
        }
    }
}
