using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Destructable : MonoBehaviour
{
    [SerializeField] private GameObject _drop;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        _health.OnDie += DropItem;
    }

    private void OnDestroy()
    {
        _health.OnDie -= DropItem;
    }

    private void DropItem()
    {
        Instantiate(_drop, transform.position + Vector3.up * .5f, Quaternion.identity);
        Destroy(gameObject);
    }
}