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
    [SerializeField] private Button _removeButton;
    [SerializeField] private Button _editButton;

    private Toggle _toggle;
    private Image _image;

    private HorseSaveData _horseSaveData;

    public Button RemoveButton { get { return _removeButton; } }
    public Button EditButton { get { return _editButton; } }

    public event Action OnClicked;
    public event Action OnRemoved;

    public void Initiate(HorseSaveData saveData, ToggleGroup toggleGroup)
    {
        _horseSaveData = saveData;

        _nameText.text = saveData.Name;
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
        _removeButton.onClick.RemoveAllListeners();
        _editButton.onClick.RemoveAllListeners();
    }

    public void UpdateInfo()
    {
        _nameText.text = _horseSaveData.Name;
        _dateText.text = _horseSaveData.Date.ToString("d");
        _descriptionText.text = _horseSaveData.Description;
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
        storage.DeleteHorseSave(_horseSaveData);

        Destroy(gameObject);

        PageManager.Instance.CloseWarningPage();

        OnRemoved?.Invoke();
    }

    public void EditSave(HorseSaveData saveData)
    {
        _horseSaveData = saveData;

        _nameText.text = saveData.Name;
        _descriptionText.text = saveData.Description;
        _dateText.text = saveData.Date.ToString("d");
    }
}
