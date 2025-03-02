using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Interval
{
    public float Steps;
    public UnityEvent OnTrigger;
    private int _lastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60 / bpm * Steps;
    }

    public void CheckForNewInterval(float interval)
    {
        if (Mathf.FloorToInt(interval) != _lastInterval)
        {
            _lastInterval = Mathf.FloorToInt(interval);
            OnTrigger.Invoke();
        }
    }
}

public class BeatController : MonoBehaviour
{
    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<Interval> _intervals;

    private void Start()
    {
        BeatReactive[] objs = FindObjectsByType<BeatReactive>(FindObjectsSortMode.None);
        foreach (BeatReactive obj in objs)
        {
            Interval interval = _intervals.Find(interval => interval.Steps == obj.Step);
            if (interval != null)
            {
                interval.OnTrigger.AddListener(obj.OnBeat);
            }
            else
            {
                Interval newInterval = new Interval { Steps = obj.Step, OnTrigger = new UnityEvent() };
                newInterval.OnTrigger.AddListener(obj.OnBeat);

                _intervals.Add(newInterval);
            }
        }
    }

    private void Update()
    {
        foreach (Interval interval in _intervals)
        {
            float sampledTime = _audioSource.timeSamples / _audioSource.clip.frequency * interval.GetIntervalLength(_bpm);
            interval.CheckForNewInterval(sampledTime);
        }
    }
}