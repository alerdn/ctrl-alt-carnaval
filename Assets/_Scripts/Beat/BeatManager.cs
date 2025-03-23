using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Interval
{
    public float Step;
    public UnityAction OnBeatHit;

    public List<float> BeatTimes = new List<float>(); // Armazena os tempos dos beats
    private int _lastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60f / bpm * Step;
    }

    public void CheckForNewInterval(float interval)
    {
        if (Mathf.FloorToInt(interval) != _lastInterval)
        {
            _lastInterval = Mathf.FloorToInt(interval);
            OnBeatHit.Invoke();
        }
    }

    public void CalculateBeatTimes(float songLength, float bpm)
    {
        BeatTimes.Clear();
        float intervalLength = GetIntervalLength(bpm);
        for (float t = 0; t <= songLength; t += intervalLength)
        {
            BeatTimes.Add(t);
        }
    }
}

[System.Serializable]
public record BeatTime
{
    public float Step;
    public List<float> Times;
}

public class BeatManager : Singleton<BeatManager>
{
    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<Interval> _intervals;

    private void Update()
    {
        float audioTime = _audioSource.time;

        if (audioTime == 0 || audioTime == _audioSource.clip.length)
        {
            _audioSource.Play();
        }

        foreach (Interval interval in _intervals)
        {
            float sampledTime = (float)_audioSource.timeSamples / (float)_audioSource.clip.frequency * interval.GetIntervalLength(_bpm);
            interval.CheckForNewInterval(sampledTime);
        }
    }

    public void AddBeatReactive(BeatReactive obj)
    {
        Interval interval = _intervals.Find(interval => interval.Step == obj.Step);
        if (interval != null)
        {
            interval.OnBeatHit += obj.OnBeat;
        }
        else
        {
            Interval newInterval = new()
            {
                Step = obj.Step
            };

            newInterval.OnBeatHit += obj.OnBeat;

            _intervals.Add(newInterval);
        }
    }
}
