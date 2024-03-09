using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Ford.SaveSystem.Ver2;
using Ford.SaveSystem.Ver2.Dto;
using Ford.SaveSystem;
using System.Linq;

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
    [SerializeField] private OwnerPanel _ownerPanel;

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
    private List<FieldMaskValidate> _validators;
    public event Action<HorseBase> OnApply;

    private void Start()
    {
        _inputFields = transform.GetComponentsInChildren<TMP_InputField>().ToList();
        _validators = new();

        foreach (var field in _inputFields)
        {
            if (field.TryGetComponent<FieldMaskValidate>(out var validator))
            {
                _validators.Add(validator);
            }
        }
    }

    private void OnDestroy()
    {
        _applyButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Open page with field inputs. This methods for update horse save, not create horse
    /// </summary>
    /// <typeparam name="HorseData"></typeparam>
    /// <param name="data"></param>
    public override void Open<T>(T horseData, int popUpLevel)
    {
        base.Open(horseData, popUpLevel);
        PageManager.Instance.OpenPage(_ownerPanel);

        if (horseData is not HorseBase data)
        {
            Debug.LogError($"Need {nameof(HorseBase)} param");
            return;
        }

        //Field inputs
        _headerText.text = $"Изменение данных о {data.Name}";
        _horseNameInputField.text = data.Name;
        _sexText.text = data.Sex;
        _birthdayInputFiled.text = data.BirthDate?.ToString("dd.MM.yyyy");
        _descriptionInputField.text = data.Description;
        _countryInputFiled.text = data.Country;
        _cityInputFiled.text = data.City;
        _regionInputFiled.text = data.Region;

        var owner = data.Users.SingleOrDefault(o => o.IsOwner);

        if (owner != null)
        {
            _ownerPanel.SetRealOwner(owner);
        }

        _applyButtonText.text = "Применить";
        _applyButton.onClick.AddListener(EditHorse);
        _cancelButton.onClick.AddListener(Close);
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);
        PageManager.Instance.OpenPage(_ownerPanel);

        _headerText.text = "Новый проект";

        _applyButtonText.text = "Начать";
        _applyButton.onClick.AddListener(StartProject);
        _cancelButton.onClick.AddListener(Close);
        _cancelButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(PageManager.Instance.StartPage); });
    }

    public override void Close()
    {
        base.Close();
        PageManager.Instance.ClosePage(_ownerPanel);

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

    private void EditHorse()
    {
        if (!CheckValidData())
            return;

        CreationHorseData horse = new()
        {
            Name = _horseNameInputField.text,
            Description = _descriptionInputField.text,
            BirthDate = _birthdayInputFiled.GetComponent<InputFieldDateValidator>().Date,
            Sex = _sexText.text,
            Country = _countryInputFiled.text,
            City = _cityInputFiled.text,
            Region = _regionInputFiled.text,
            OwnerName = _ownerPanel.OwnerName,
            PhoneNumber = _ownerPanel.OwnerNumber,
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
            BirthDate = _birthdayInputFiled.GetComponent<InputFieldDateValidator>().Date,
            Description = _descriptionInputField.text,
            Country = _countryInputFiled.text,
            City = _cityInputFiled.text,
            Region = _regionInputFiled.text,
            OwnerName = _ownerPanel.OwnerName,
            PhoneNumber = _ownerPanel.OwnerNumber,
        };

        var creatingHorse = storage.CreateHorse(horse);
        storage.CreateSave(creatingHorse.HorseId, save);

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
