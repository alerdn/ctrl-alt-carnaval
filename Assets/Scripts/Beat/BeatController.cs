using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Interval
{
    public float Step;
    public UnityEvent OnBeatHit;

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

public class BeatController : Singleton<BeatController>
{
    public UnityAction OnMusicStarted;

    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<Interval> _intervals;

    public List<BeatTime> AllBeatTimes = new List<BeatTime>(); // Lista global para a UI

    private void Start()
    {
        BeatReactive[] objs = FindObjectsByType<BeatReactive>(FindObjectsSortMode.None);
        foreach (BeatReactive obj in objs)
        {
            Interval interval = _intervals.Find(interval => interval.Step == obj.Step);
            if (interval != null)
            {
                interval.OnBeatHit.AddListener(obj.OnBeat);
            }
            else
            {
                Interval newInterval = new Interval
                {
                    Step = obj.Step,
                    OnBeatHit = new UnityEvent(),
                };

                newInterval.OnBeatHit.AddListener(obj.OnBeat);

                _intervals.Add(newInterval);
            }
        }
    }

    private void Update()
    {
        float audioTime = GetAudioTime();

        if (audioTime == 0 || audioTime == _audioSource.clip.length)
        {
            CalculateAllBeats();
            _audioSource.Play();
            OnMusicStarted?.Invoke();
        }

        foreach (Interval interval in _intervals)
        {
            float sampledTime = (float)_audioSource.timeSamples / (float)_audioSource.clip.frequency * interval.GetIntervalLength(_bpm);
            interval.CheckForNewInterval(sampledTime);
        }
    }

    private void CalculateAllBeats()
    {
        if (_audioSource.clip == null) return;

        AllBeatTimes.Clear();
        float songLength = _audioSource.clip.length;

        foreach (Interval interval in _intervals)
        {
            interval.CalculateBeatTimes(songLength, _bpm);
            AllBeatTimes.Add(new BeatTime() { Step = interval.Step, Times = interval.BeatTimes });
        }
    }

    public float GetAudioTime()
    {
        return _audioSource.time;
    }
}
