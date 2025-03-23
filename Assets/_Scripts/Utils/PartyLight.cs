using DG.Tweening;
using UnityEngine;

public class PartyLight : MonoBehaviour
{
    [Range(-1f, 1f)]
    public float hue;
    public float hueSpeed = 0.2f;
    private float hueValue = 0f;

    private Light _light;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void Start()
    {
        _light.transform.DOMove(transform.position + Vector3.one * 2, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        hueValue += hueSpeed * Time.deltaTime;
        if (hueValue > 1f) hueValue -= 1f;

        Color.RGBToHSV(_light.color, out _, out float saturation, out float value);

        Color newColor = Color.HSVToRGB(hueValue, saturation, value);
        _light.color = newColor;
    }
}