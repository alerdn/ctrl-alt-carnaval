using UnityEngine;

public class MapObject : MonoBehaviour
{
    public Color Color => _color;

    [ColorHtmlProperty]
    [SerializeField] private Color _color;
}