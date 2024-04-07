using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(RectTransform))]
public class LoadIcon : MonoBehaviour
{
    [SerializeField] private Image _loadImage;
    [SerializeField] private bool _check;
    [SerializeField] private float _fillSpeed = 1f;
    [SerializeField] private float _rotationSpeed = 1f;
    private bool _temp;

    private LayoutElement _layoutElement;
    private bool _freeRotation;
    private float _elapsed = 0f;
    private float _direction = 1f;

    public float MinHeight => _layoutElement.minHeight;
    public event Action OnIconFilled;
    public float FillAmount => _loadImage.fillAmount;

    private void Start()
    {
        _layoutElement = GetComponent<LayoutElement>();

        _layoutElement.enabled = false;
    }

    private void Update()
    {
        if (_temp != _check)
        {
            EnableFreeRotate(_check);
        }

        _temp = _check;

        // upload script
        if (_freeRotation)
        {
            if (_loadImage.fillAmount >= 1)
            {
                _direction = -1f;
                _loadImage.fillClockwise = false;
            }

            if (_loadImage.fillAmount <= 0)
            {
                _direction = 1f;
                _loadImage.fillClockwise = true;
            }

            _loadImage.rectTransform.rotation = Quaternion.Euler(new()
            {
                x = _loadImage.rectTransform.rotation.eulerAngles.x,
                y = _loadImage.rectTransform.rotation.eulerAngles.y,
                z = _loadImage.rectTransform.rotation.eulerAngles.z - (_rotationSpeed * Time.deltaTime),
            });

            _elapsed = _direction * Time.deltaTime * _fillSpeed;
            _loadImage.fillAmount += _elapsed;
        }
    }

    public void FillIcon(float fill)
    {
        _loadImage.fillAmount = fill;

        if (fill >= 1)
        {
            OnIconFilled?.Invoke();
        }
    }

    public void EnableLayout(bool enable)
    {
        _layoutElement.enabled = enable;
    }

    public void EnableFreeRotate(bool enable)
    {
        if (!enable)
        {
            _loadImage.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        _freeRotation = enable;
    }
}
