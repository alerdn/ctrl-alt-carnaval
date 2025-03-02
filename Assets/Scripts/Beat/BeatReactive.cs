using UnityEngine;

public abstract class BeatReactive : MonoBehaviour
{
    [field: SerializeField] public float Step { get; private set; }

    public abstract void OnBeat();
}