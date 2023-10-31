using UnityEngine;

[CreateAssetMenu(fileName = "New ColorOutline", menuName = "Color/Create new color Outline")]
public class ColorSelectionBone : ScriptableObject
{
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectionColor;
    [SerializeField] private Color _highlightedColor;
    [SerializeField] private Color _highlightedAfterSelectColor;

    public Color DefaultColor { get { return _defaultColor; } }
    public Color SelectionColor { get { return _selectionColor; } }
    public Color HighlightColor { get { return _highlightedColor; } }
    public Color HighlightedAfterSelectColor { get { return _highlightedAfterSelectColor; } }
}
