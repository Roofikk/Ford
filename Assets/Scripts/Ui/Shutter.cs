using System;
using UnityEngine;
using UnityEngine.UI;

public class Shutter : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _toggleImage;
    [SerializeField] private Sprite _showTexture;
    [SerializeField] private Sprite _hideTexture;

    public Shutter Initiate(Action<bool> onToggleValueChanged)
    {
        _toggle.onValueChanged.AddListener(value =>
        {
            _toggleImage.sprite = value ? _showTexture : _hideTexture;
            onToggleValueChanged?.Invoke(value);
        });
        return this;
    }
}
