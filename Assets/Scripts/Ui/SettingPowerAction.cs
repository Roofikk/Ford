using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPowerAction : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] TextMeshProUGUI _powerValueText;

    private float _min;
    private float _max;

    public Action<float> OnPowerValueChanged;

    public void Start()
    {
        _slider.wholeNumbers = true;
        _slider.onValueChanged.AddListener(UpdateValue);
    }

    public void SetSetting(float min, float max, float current)
    {
        _min = min;
        _max = max;

        float stepLength = (max - min) / _slider.maxValue;
        float lengthToCurrent = current - min;

        _slider.value = lengthToCurrent / stepLength;

        _powerValueText.text = current.ToString();
    }

    public void UpdateValue(float sliderValue)
    {
        int countStep = Mathf.CeilToInt(_slider.maxValue);
        float stepLength = (_max - _min) / countStep;
        float value = _min + stepLength * sliderValue;

        _powerValueText.text = value.ToString();
        OnPowerValueChanged?.Invoke(value);
    }
}
