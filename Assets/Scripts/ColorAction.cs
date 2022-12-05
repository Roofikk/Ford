using UnityEngine;

[CreateAssetMenu(fileName = "New Color Action", menuName = "Color/Create new Color Action")]
public class ColorAction : ScriptableObject
{
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _dragColor;
    [SerializeField] private Color _highlightedColor;
    [SerializeField] private Color _deactivatedColor;

    public Color Default { get { return _defaultColor; } }
    public Color Drag { get { return _dragColor; } }
    public Color Highlighted { get { return _highlightedColor; } }
    public Color Deactivated { get { return _deactivatedColor; } }
}
