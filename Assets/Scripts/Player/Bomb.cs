using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5f;

    private void Update()
    {
        _lifeTime -= Time.deltaTime;

        if (_lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isTarget = other.CompareTag("Enemy");

        if (isTarget)
        {
            if (other.TryGetComponent(out Health health))
            {
                DamageData damage = PlayerStateMachine.Instance.Gun.GetDamage();
                damage.IsCritical = true;
                health.TakeDamage(damage);
            }
        }
    }
}
