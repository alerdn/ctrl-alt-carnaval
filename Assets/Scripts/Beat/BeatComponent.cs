using UnityEngine.Events;

public class BeatComponent : BeatReactive
{
    public UnityEvent OnBeatEvent;
    public UnityAction OnBeatAction;

    public override void OnBeat()
    {
        OnBeatAction?.Invoke();
        OnBeatEvent?.Invoke();
    }
}