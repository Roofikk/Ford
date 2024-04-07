using Ford.SaveSystem.Ver2;
using Ford.SaveSystem.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Ford.SaveSystem;

public class SaveElementUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _dateText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Color _selectedColor;

    [Space(10)]
    [Header("Buttons")]
    [SerializeField] private Button _moreInfoButton;

    private Toggle _toggle;
    private Image _image;

    public SaveInfo SaveData { get; private set; }

    public SaveElementUI Initiate(SaveInfo saveData, ToggleGroup toggleGroup, Action onClick, Action onMoreButtonClick)
    {
        SaveData = saveData;

        _nameText.text = saveData.Header;
        _dateText.text = saveData.Date.ToString("d");
        _descriptionText.text = saveData.Description;

        if (_toggle == null)
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(value =>
            {
                SelectSave(value);
                onClick?.Invoke();
            });
            _toggle.group = toggleGroup;
        }

        if (_image == null)
        {
            _image = GetComponent<Image>();
        }

        _moreInfoButton.onClick.AddListener(() =>
        {
            onMoreButtonClick?.Invoke();
        });

        return this;
    }

    private void OnDestroy()
    {
        _toggle.group = null;
        _moreInfoButton.onClick.RemoveAllListeners();
    }

    public void UpdateInfo(SaveInfo save)
    {
        SaveData = save;

        _nameText.text = SaveData.Header;
        _dateText.text = SaveData.Date.ToString("d");
        _descriptionText.text = SaveData.Description;
    }

    private void SelectSave(bool value)
    {
        if (_image == null) 
            _image = GetComponent<Image>();

        if (value)
            _image.color = _selectedColor;
        else
            _image.color = _toggle.colors.normalColor;
    }
}
