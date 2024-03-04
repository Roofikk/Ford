using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Ford.SaveSystem.Ver2;
using Ford.SaveSystem.Ver2.Dto;
using Ford.SaveSystem.Ver2.Data;

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

        if (param is not SaveData saveData)
            return;

        _cancelButton.onClick.AddListener(Close);
        _applyButton.onClick.AddListener(() => { EditSave(new()
        {
            Header = saveData.Header,
            Date = saveData.Date,
            Description = saveData.Description,
            Id = saveData.Id
        }); });
        _applyButtonText.text = "Изменить";

        _headerInputField.text = saveData.Header;
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
            inputMask.DisplayException(false);

            if (inputMask.TryGetComponent(out InputFieldValidateStroke stroke))
                stroke.DisplayStroke(false);
        }
    }

    private void EditSave(UpdateSaveDto saveData)
    {
        foreach (var inputMask in _inputMaskValidateList)
        {
            if (!inputMask.ValidateInput())
            {
                return;
            }
        }

        saveData.Header = _headerInputField.text;
        saveData.Description = _descriptionInputField.text;
        saveData.Date = DateTime.Parse(_dateInputField.text);

        Storage storage = new(GameManager.Instance.Settings.PathSave);
        storage.UpdateSave(saveData);

        ToastMessage toast =  Instantiate(_toastMessage.gameObject, transform.parent).GetComponent<ToastMessage>();
        toast.Show("Изменения применены");

        PageManager.Instance.ClosePage(this);
        HorseUpdated?.Invoke();
    }

    public void Save()
    {
        ToastMessage toast;

        //if (GameManager.Instance.DevMode)
        //{
        //    DevHorseSaveData devSave = new(
        //        _headerInputField.text,
        //        _descriptionInputField.text,
        //        DateTime.Parse(_dateInputField.text),
        //        null,
        //        _skeleton.Data.Id,
        //        null//_skeleton.GetDevBonesData()
        //    );

        //    Storage devStorage = new(GameManager.Instance.Settings.PathSave);
        //    //devStorage.DevSaveStateHorse(devSave);

        //    toast = Instantiate(_toastMessage.gameObject, transform.parent).GetComponent<ToastMessage>();
        //    toast.Show("Сохранение завершено");

        //    PageManager.Instance.ClosePage(this);

        //    return;
        //}

        foreach (var inputMask in _inputMaskValidateList)
        {
            if (!inputMask.ValidateInput())
            {
                return;
            }
        }

        CreateSaveDto save = new()
        {
            Header = _headerInputField.text,
            Description = _descriptionInputField.text,
            Date = _dateInputField.GetComponent<InputFieldDateValidator>().Date.Value,
            Bones = _skeleton.GetBonesForSave()
        };

        Storage storage = new(GameManager.Instance.Settings.PathSave);
        storage.CreateSave(_skeleton.Data.Id, save);

        toast = Instantiate(_toastMessage.gameObject, transform.parent).GetComponent<ToastMessage>();
        toast.Show("Сохранение завершено");

        PageManager.Instance.ClosePage(this);
    }

    //public void DevSave()
    //{
    //    DevHorseSaveData save = new(
    //        _headerInputField.text + " -- DevSave",
    //        _descriptionInputField.text,
    //        DateTime.Parse(_dateInputField.text),
    //        null,
    //        _skeleton.Data.Id,
    //        null//_skeleton.GetDevBonesData()
    //    );

    //    Storage storage = new(GameManager.Instance.Settings.PathSave);
    //    //storage.CreateSave(save);

    //    ToastMessage toast = Instantiate(_toastMessage.gameObject, transform.parent).GetComponent<ToastMessage>();
    //    toast.Show("Сохранение завершено");

    //    PageManager.Instance.ClosePage(this);
    //}
}