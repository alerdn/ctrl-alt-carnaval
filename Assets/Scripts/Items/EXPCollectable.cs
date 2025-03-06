using UnityEngine;
using UnityEngine.Pool;

public class EXPCollectable : Collectable
{
    private int _expValue;
    private IObjectPool<EXPCollectable> _expPool;

    public void SetPool(IObjectPool<EXPCollectable> expPool)
    {
        _expPool = expPool;
    }

    public void Init(int expValue, Vector3 position)
    {
        _expValue = expValue;
        Vector3 newPosition = new(position.x, transform.position.y, position.z);
        transform.SetPositionAndRotation(newPosition, Quaternion.identity);
    }

    public override void Collect()
    {
        ExperienceManager.Instance.AddExperience(_expValue);
        _expPool.Release(this);
    }
}