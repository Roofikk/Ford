using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class SavePanel : Page
{
    [SerializeField] private ToastMessage _toastMessage;
    [SerializeField] private Skeleton _skeleton;

    [Space(10)]
    [Header("Fields")]
    [SerializeField] private TMP_InputField _headerInputField;
    [SerializeField] private TMP_InputField _dateInputField;
    [SerializeField] private TMP_InputField _descriptionInputField;
    [SerializeField] private TextMeshProUGUI _exceptionText;

    [Space(10)]
    [Header("Buttons")]
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _applyButton;
    [SerializeField] private TextMeshProUGUI _applyButtonText;

    private List<FieldMaskValidate> _inputMaskValidateList;

    public event Action HorseUpdated;
    
    private void Start()
    {
        _inputMaskValidateList = GetComponentsInChildren<FieldMaskValidate>().ToList();
    }

    public override void Open<T>(T param, int popUpLevel = 0)
    {
        base.Open(param, popUpLevel);

        if (param is not HorseSaveData saveData)
            return;

        _cancelButton.onClick.AddListener(Close);
        _applyButton.onClick.AddListener(() => { EditSave(saveData); });
        _applyButtonText.text = "Изменить";

        _headerInputField.text = saveData.Name;
        _descriptionInputField.text = saveData.Description;
        _dateInputField.text = saveData.Date.ToString("dd.MM.yyyy");
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        _cancelButton.onClick.AddListener(Close);
        _applyButton.onClick.AddListener(Save);
        _applyButtonText.text = "Сохранить";

        _dateInputField.text = DateTime.Now.ToString("dd.MM.yyyy");
    }

    public override void Close()
    {
        base.Close();

        _applyButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        _headerInputField.text = "";
        _descriptionInputField.text = "";

        foreach (var inputMask in _inputMaskValidateList)
        {
            inputMask.DisplayDescription(false);

            if (inputMask.TryGetComponent(out InputFieldValidateStroke stroke))
                stroke.DisplayStroke(false);
        }
    }

    private void EditSave(HorseSaveData saveData)
    {
        foreach (var inputMask in _inputMaskValidateList)
        {
            if (!inputMask.ValidateInput())
            {
                return;
            }
        }

        saveData.Name = _headerInputField.text;
        saveData.Description = _descriptionInputField.text;
        saveData.Date = DateTime.Parse(_dateInputField.text);

        Storage storage = new(GameManager.Instance.Settings.PathSave);
        storage.UpdateHorseSave(saveData);

        ToastMessage toast =  Instantiate(_toastMessage.gameObject, transform.parent).GetComponent<ToastMessage>();
        toast.Show("Изменения применены");

        PageManager.Instance.ClosePage(this);
        HorseUpdated?.Invoke();
    }

    public void Save()
    {
        ToastMessage toast;

        if (GameManager.Instance.DevMode)
        {
            DevHorseSaveData devSave = new(
                _headerInputField.text,
                _descriptionInputField.text,
                DateTime.Parse(_dateInputField.text),
                null,
                _skeleton.Data.Id,
                _skeleton.GetDevBonesData()
            );

            Storage devStorage = new(GameManager.Instance.Settings.PathSave);
            devStorage.DevSaveStateHorse(devSave);

            toast = Instantiate(_toastMessage.gameObject, transform.parent).GetComponent<ToastMessage>();
            toast.Show("Сохранение завершено");

            PageManager.Instance.ClosePage(this);

            return;
        }

        foreach (var inputMask in _inputMaskValidateList)
        {
            if (!inputMask.ValidateInput())
            {
                return;
            }
        }

        HorseSaveData save = new(
            _headerInputField.text,
            _descriptionInputField.text,
            DateTime.Parse(_dateInputField.text),
            _skeleton.GetBonesForSave(),
            _skeleton.Data.Id
        );

        Storage storage = new(GameManager.Instance.Settings.PathSave);
        storage.AddHorseSave(save);

        toast = Instantiate(_toastMessage.gameObject, transform.parent).GetComponent<ToastMessage>();
        toast.Show("Сохранение завершено");

        PageManager.Instance.ClosePage(this);
    }

    public void DevSave()
    {
        DevHorseSaveData save = new(
            _headerInputField.text + " -- DevSave",
            _descriptionInputField.text,
            DateTime.Parse(_dateInputField.text),
            null,
            _skeleton.Data.Id,
            _skeleton.GetDevBonesData()
        );

        Storage storage = new(GameManager.Instance.Settings.PathSave);
        storage.AddHorseSave(save);

        ToastMessage toast = Instantiate(_toastMessage.gameObject, transform.parent).GetComponent<ToastMessage>();
        toast.Show("Сохранение завершено");

        PageManager.Instance.ClosePage(this);
    }
}