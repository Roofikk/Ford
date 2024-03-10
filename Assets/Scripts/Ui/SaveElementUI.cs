using Ford.SaveSystem.Ver2;
using Ford.SaveSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private SaveData _saveData;

    public event Action OnClicked;
    public event Action OnRemoved;

    public void Initiate(SaveData saveData, ToggleGroup toggleGroup)
    {
        _saveData = saveData;

        _nameText.text = saveData.Header;
        _dateText.text = saveData.Date.ToString("d");
        _descriptionText.text = saveData.Description;

        if (_toggle == null)
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(SelectSave);
            _toggle.group = toggleGroup;
        }

        if (_image == null)
        {
            _image = GetComponent<Image>();
        }
    }

    private void OnDestroy()
    {
        _toggle.group = null;
        _moreInfoButton.onClick.RemoveAllListeners();
    }

    public void UpdateInfo()
    {
        _nameText.text = _saveData.Header;
        _dateText.text = _saveData.Date.ToString("d");
        _descriptionText.text = _saveData.Description;
    }

    private void SelectSave(bool value)
    {
        if (_image == null) 
            _image = GetComponent<Image>();

        if (value)
            _image.color = _selectedColor;
        else
            _image.color = _toggle.colors.normalColor;

        OnClicked?.Invoke();
    }

    public void RemoveSave()
    {
        Storage storage = new(GameManager.Instance.Settings.PathSave);
        storage.DeleteSave(_saveData.Id);

        Destroy(gameObject);
        PageManager.Instance.CloseWarningPage();
        OnRemoved?.Invoke();
    }

    public void EditSave(SaveData saveData)
    {
        _saveData = saveData;

        _nameText.text = saveData.Header;
        _descriptionText.text = saveData.Description;
        _dateText.text = saveData.Date.ToString("d");
    }
}
