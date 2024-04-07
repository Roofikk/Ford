using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadFieldText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _infoText;

    [Space(10)]
    [SerializeField] private Image _effectedImage;
    [SerializeField] private float _halfPeriodDuration;
    [SerializeField] private Color _startColor = Color.gray;
    [SerializeField] private Color _endColor = Color.white;

    private Tween _tween;

    public void DisplayEffect(bool enable)
    {
        _effectedImage = _effectedImage != null ? _effectedImage : GetComponent<Image>();
        _effectedImage.gameObject.SetActive(enable);

        if (enable)
        {
            _effectedImage.color = _startColor;
            _tween = _effectedImage.DOColor(_endColor, _halfPeriodDuration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            _tween?.Kill();
        }
    }

    public void SetInfo(string info)
    {
        DisplayEffect(false);

        _infoText.gameObject.SetActive(true);
        _infoText.text = info;
    }
}
