using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image _centerImage;
    [SerializeField] private Gun _gun;

    private void Start()
    {
        _gun.OnFireEvent += VerifyFire;
    }

    private void VerifyFire(bool success)
    {
        if (success)
        {
            _centerImage.color = Color.green;
        }
        else
        {
            _centerImage.color = Color.red;
        }
    }
}
