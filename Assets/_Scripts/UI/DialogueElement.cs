using UnityEngine;
using UnityEngine.Pool;

public abstract class DialogueElement : MonoBehaviour
{
    protected IObjectPool<DialogueElement> elementPool;

    public void SetPool(IObjectPool<DialogueElement> elementPool)
    {
        this.elementPool = elementPool;
    }
}