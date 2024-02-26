using Ford.SaveSystem.Ver2.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HorseInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _horseNameText;
    [SerializeField] private TextMeshProUGUI _sexText;
    [SerializeField] private TextMeshProUGUI _birthdateText;
    [SerializeField] private TextMeshProUGUI _ageText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _ownerNameText;
    [SerializeField] private TextMeshProUGUI _localityText;
    [SerializeField] private TextMeshProUGUI _phoneNumberText;

    [SerializeField] private Button _editButton;

    [Space(15)]
    [SerializeField] private HorsePage _horsePage;

    private HorseData _horseData;

    public event Action HorseUpdated;

    private void Start()
    {
        _editButton.onClick.AddListener(OpenEditPage);
        _horsePage.OnApply += UpdateHorseInfo;
    }

    private void OnDestroy()
    {
        _editButton.onClick.RemoveAllListeners();
        _horsePage.OnApply -= UpdateHorseInfo;
    }

    public void FillData(HorseData horseData)
    {
        _horseData = horseData;

        _horseNameText.text = horseData.Name;
        _sexText.text = horseData.Sex;
        _birthdateText.text = horseData.BirthDate?.ToString("dd.MM.yyyy");

        TimeSpan? timeSpan = DateTime.Now - horseData.BirthDate;
        if (timeSpan == null)
        {
            _ageText.text = "-";
        }
        else
        {
            _ageText.text = new DateTime(timeSpan.Value.Ticks).Year.ToString();
        }

        _descriptionText.text = horseData.Description;
        _localityText.text = horseData.City;

        if (horseData.Owner != null)
        {
            _ownerNameText.text = horseData.Owner.Name;
            _phoneNumberText.text = horseData.Owner.PhoneNumber;
        }
        else
        {
            _ownerNameText.text = "Безхозный";
            _phoneNumberText.text = "Отсутствует";
        }
    }

    private void UpdateHorseInfo(HorseData horseData)
    {
        FillData(horseData);
        HorseUpdated?.Invoke();
    }

    public void OpenEditPage()
    {
        PageManager.Instance.OpenPage(_horsePage, _horseData, 1);
    }
}
