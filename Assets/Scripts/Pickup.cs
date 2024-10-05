using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pickup : MonoBehaviour
{
    public PickupSO PickupSO;

    [SerializeField] private float pickupSpeed;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Ease ease;

    private void Awake()
    {
        meshRenderer.material = PickupSO.PickupMaterial;
    }

    public void AttractPickup(Vector3 to)
    {
        transform.DOMove(to, pickupSpeed).SetEase(ease).OnComplete(() => { Destroy(gameObject); });
    }
    public void Setup()
    {
        PickupSO = GameAssets.Instance.PickupTypes[Random.Range(0, GameAssets.Instance.PickupTypes.Length)];
        meshRenderer.material = PickupSO.PickupMaterial;
    }
}
