using System.Linq;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _radius;

    private BeatComponent _beatComponent;

    private void Awake()
    {
        _beatComponent = GetComponent<BeatComponent>();
    }

    private void Start()
    {
        _beatComponent.OnBeatAction += Hit;
    }

    private void OnDestroy()
    {
        _beatComponent.OnBeatAction -= Hit;
    }

    private void Update()
    {
        _lifeTime -= Time.deltaTime;

        if (_lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Hit()
    {
        Physics.OverlapSphere(transform.position, _radius, LayerMask.GetMask("Enemy")).ToList()
            .ForEach(collider =>
            {
                if (collider.TryGetComponent<EnemyStateMachine>(out var enemy))
                {
                    DamageData damage = PlayerStateMachine.Instance.Gun.GetDamage();
                    damage.IsCritical = true;
                    enemy.Health.TakeDamage(damage);
                }
            });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
