using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Ford.SaveSystem.Ver2.Data;
using Ford.SaveSystem.Ver2;
using Ford.SaveSystem.Ver2.Dto;
using System.Globalization;

public class HorsePage : Page
{
    [Header("Header")]
    [SerializeField] private TextMeshProUGUI _headerText;

    [Header("Input fields")]
    [SerializeField] private TMP_InputField _horseNameInputField;
    [SerializeField] private TextMeshProUGUI _sexText;
    [SerializeField] private TMP_InputField _birthdayInputFiled;
    [SerializeField] private TMP_InputField _descriptionInputField;
    [SerializeField] private TMP_InputField _countryInputFiled;
    [SerializeField] private TMP_InputField _cityInputFiled;
    [SerializeField] private TMP_InputField _regionInputFiled;
    [SerializeField] private TMP_InputField _ownerNameInputFiled;
    [SerializeField] private TMP_InputField _phoneNumberInputField;

    [Space(10)]
    [Header("Buttons")]
    [SerializeField] private Button _applyButton;
    [SerializeField] private TextMeshProUGUI _applyButtonText;
    [SerializeField] private Button _cancelButton;

    [Space(10)]
    [SerializeField] private LoadScenePage _loadScenePage;

    [Space(10)]
    [Header("Toast message")]
    [SerializeField] private ToastMessage _toastMessagePrefab;

    private List<TMP_InputField> _inputFields;
    public event Action<HorseData> OnApply;

    /// <summary>
    /// Open page with field inputs. This methods for update horse save, not create horse
    /// </summary>
    /// <typeparam name="HorseData"></typeparam>
    /// <param name="data"></param>
    public override void Open<T>(T horseData, int popUpLevel)
    {
        base.Open(horseData, popUpLevel);

        if (horseData is not HorseData data)
        {
            Debug.LogError("Параметр не соответствует запрашиваемому");
            return;
        }

        //Field inputs
        _headerText.text = $"Изменение данных о {data.Name}";
        _horseNameInputField.text = data.Name;
        _sexText.text = data.Sex;
        _birthdayInputFiled.text = data.BirthDate?.ToString("dd.MM.yyyy");
        _descriptionInputField.text = data.Description;
        _ownerNameInputFiled.text = data.Owner.Name;
        _phoneNumberInputField.text = data.Owner.PhoneNumber;
        _countryInputFiled.text = data.Country;
        _cityInputFiled.text = data.City;
        _regionInputFiled.text = data.Region;

        _applyButtonText.text = "Применить";
        _applyButton.onClick.AddListener(EditHorse);
        _cancelButton.onClick.AddListener(Close);

        InitiatePhoneInput();
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        _headerText.text = "Новый проект";

        _applyButtonText.text = "Начать";
        _applyButton.onClick.AddListener(StartProject);
        _cancelButton.onClick.AddListener(Close);
        _cancelButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(PageManager.Instance.StartPage); });

        InitiatePhoneInput();
    }

    public override void Close()
    {
        base.Close();

        foreach (var input in _inputFields)
        {
            input.text = string.Empty;
            
            if (input.TryGetComponent<FieldMaskValidate>(out var validator))
                validator.DisplayException(false);

            if (input.TryGetComponent<InputFieldValidateStroke>(out var stroke))
                stroke.DisplayStroke(false);
        }

        _applyButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        _inputFields = new List<TMP_InputField>()
        {
            _horseNameInputField,
            _birthdayInputFiled,
            _descriptionInputField,
            _ownerNameInputFiled,
            _phoneNumberInputField,
            _countryInputFiled,
            _cityInputFiled,
            _regionInputFiled
        };
    }

    private void OnDestroy()
    {
        _phoneNumberInputField.onEndEdit.RemoveAllListeners();
        _applyButton.onClick.RemoveAllListeners();
    }

    private void InitiatePhoneInput()
    {
        _ownerNameInputFiled.GetComponent<FieldMaskValidate>().enabled = false;

        _phoneNumberInputField.onEndEdit.AddListener((text) =>
        {
            var validator = _ownerNameInputFiled.GetComponent<FieldMaskValidate>();

            if (string.IsNullOrEmpty(text))
            {
                validator.enabled = false;
            }
            else
            {
                validator.enabled = true;
            }

            validator.DisplayException(false);
        });
    }

    private void EditHorse()
    {
        if (!CheckValidData())
            return;

        CreationHorseData horse = new()
        {
            Name = _horseNameInputField.text,
            Description = _descriptionInputField.text,
            BirthDate = _birthdayInputFiled.GetComponent<InputFieldDateValidate>().Date,
            Sex = _sexText.text,
            Country = _countryInputFiled.text,
            City = _cityInputFiled.text,
            Region = _regionInputFiled.text,
            OwnerName = _ownerNameInputFiled.text,
            PhoneNumber = _phoneNumberInputField.text,
        };

        Storage storage = new(GameManager.Instance.Settings.PathSave);
        var horseData = storage.UpdateHorse(horse);

        OnApply?.Invoke(horseData);
        Close();

        ToastMessage toast = Instantiate(_toastMessagePrefab.gameObject, transform.parent).GetComponent<ToastMessage>();
        toast.Show("Изменения приняты");
    }

    private void StartProject()
    {
        if (!CheckValidData())
            return;

        Storage storage = new(GameManager.Instance.Settings.PathSave);

        CreateSaveDto save = new()
        {
            Header = "Новый проект",
            Description = "Стартовое сохранение без каких либо изменений",
            Date = DateTime.Now,
            Bones = null
        };

        CreationHorseData horse = new()
        {
            Name = _horseNameInputField.text,
            Sex = _sexText.text,
            BirthDate = _birthdayInputFiled.GetComponent<InputFieldDateValidate>().Date,
            Description = _descriptionInputField.text,
            Country = _countryInputFiled.text,
            City = _cityInputFiled.text,
            Region = _regionInputFiled.text,
            OwnerName = _ownerNameInputFiled.text,
            PhoneNumber = _phoneNumberInputField.text
        };

        var creatingHorse = storage.CreateHorse(horse);
        storage.CreateSave(creatingHorse.Id, save);

        SceneParameters.AddParam(creatingHorse);
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);
        _loadScenePage.Open(loadingOperation);
    }

    public bool CheckValidData()
    {
        int countInvalid = 0;
        foreach (var input in _inputFields)
        {
            var validator = input.GetComponent<FieldMaskValidate>();
            if (validator == null)
            {
                continue;
            }

            if (!validator.enabled)
            {
                continue;
            }

            if (!validator.ValidateInput(input.text))
                countInvalid++;
        }

        if (countInvalid > 0)
            return false;
        else
            return true;
    }
}
