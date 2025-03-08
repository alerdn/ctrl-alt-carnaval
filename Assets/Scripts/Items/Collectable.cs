using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Collectable : MonoBehaviour
{
    public abstract void Collect();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Collect();
        }
    }
}