using UnityEngine;

public class PulseObject : MonoBehaviour
{
    [field: SerializeField] public float Step { get; private set; }

    [SerializeField] private float _pulseSize = 1.15f;
    [SerializeField] private float _returnSpeed = 5f;

    private Vector3 _startSize;

    private void Start()
    {
        _startSize = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _startSize, Time.deltaTime * _returnSpeed);
    }

    public void Pulse()
    {
        transform.localScale = _startSize * _pulseSize;
    }
}
