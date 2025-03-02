using System;
using DG.Tweening;
using UnityEngine;

public class Gun : BeatReactive
{
    public event Action<bool> OnFireEvent;

    [SerializeField] private PlayerStateMachine _player;
    [SerializeField] private float _range;
    [SerializeField] private float _followSpeed;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private float _beatThreshold;

    private Camera _mainCamera;
    private float _startYPostion;
    private float _lastBeatTime;

    private void Start()
    {
        _mainCamera = Camera.main;
        _startYPostion = transform.position.y;

        transform.DOMoveY(_startYPostion + .1f, 1f).From(_startYPostion - .1f).SetLoops(-1, LoopType.Yoyo);

        _player.InputReader.OnFireEvent.AddListener(Fire);
    }

    private void OnDestroy()
    {
        _player.InputReader.OnFireEvent.RemoveListener(Fire);
    }

    private void Update()
    {
        MoveToPlayer();
        Aim();
    }

    public override void OnBeat()
    {
        _lastBeatTime = Time.time;
    }

    private void MoveToPlayer()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > _range)
        {
            Vector3 targetPosition = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, distance * _followSpeed * Time.deltaTime);
        }
    }

    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            var direction = position - transform.position;
            direction.y = 0;

            transform.forward = direction;
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, GroundMask))
        {
            return (success: true, position: hitInfo.point);
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }

    private void Fire()
    {
        if (Time.time - _lastBeatTime < _beatThreshold)
        {
            Debug.Log("Fire!");
            OnFireEvent?.Invoke(true);
        }
        else
        {
            Debug.Log("Missed the beat!");
            OnFireEvent?.Invoke(false);
        }
    }
}