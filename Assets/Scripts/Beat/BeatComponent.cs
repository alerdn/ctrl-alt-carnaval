using System;
using UnityEngine;

public class BeatComponent : BeatReactive
{
    public event Action OnBeatEvent;

    public override void OnBeat()
    {
        OnBeatEvent?.Invoke();
    }
}