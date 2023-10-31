using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewHorseProjectPage : Page
{
    [Header("Header")]
    [SerializeField] private TextMeshProUGUI _headerText;

    [Header("Input fields")]
    [SerializeField] private TMP_InputField _horseNameInputField;
    [SerializeField] private TextMeshProUGUI _sexText;
    [SerializeField] private TMP_InputField _birthdayInputFiled;
    [SerializeField] private TMP_InputField _descriptionInputField;
    [SerializeField] private TMP_InputField _ownerNameInputFiled;
    [SerializeField] private TMP_InputField _phoneNumberInputField;
    [SerializeField] private TMP_InputField _localityInputFiled;

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
    private HorseData _horseData;

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

        _horseData = data;

        //Field inputs
        _headerText.text = $"Изменение данных о {_horseData.Name}";
        _horseNameInputField.text = _horseData.Name;
        _sexText.text = _horseData.Sex;
        _birthdayInputFiled.text = _horseData.Birthday;
        _descriptionInputField.text = _horseData.Description;
        _ownerNameInputFiled.text = _horseData.OwnerName;
        _phoneNumberInputField.text = _horseData.PhoneNumber;
        _localityInputFiled.text = _horseData.Locality;

        _applyButtonText.text = "Применить";
        _applyButton.onClick.AddListener(EditHorse);
        _cancelButton.onClick.AddListener(Close);
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        _headerText.text = "Новый проект";

        _applyButtonText.text = "Начать";
        _applyButton.onClick.AddListener(StartProject);
        _cancelButton.onClick.AddListener(Close);
        _cancelButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(PageManager.Instance.StartPage); });
    }

    public override void Close()
    {
        base.Close();

        foreach (var input in _inputFields)
        {
            input.text = string.Empty;
            
            if (input.TryGetComponent<FieldMaskValidate>(out var validator))
                validator.DisplayDescription(false);

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
            _localityInputFiled
        };
    }

    private void OnDestroy()
    {
        _applyButton.onClick.RemoveAllListeners();
    }

    private void EditHorse()
    {
        if (!CheckValidData())
            return;

        _horseData.Name = _horseNameInputField.text;
        _horseData.Sex = _sexText.text;
        _horseData.Birthday = _birthdayInputFiled.text;
        _horseData.Description = _descriptionInputField.text;
        _horseData.OwnerName = _ownerNameInputFiled.text;
        _horseData.PhoneNumber = _phoneNumberInputField.text;
        _horseData.Locality = _localityInputFiled.text;

        Storage storage = new(GameManager.Instance.Settings.PathSave);
        storage.UpdateHorse(_horseData);

        OnApply?.Invoke(_horseData);

        Close();

        ToastMessage toast = Instantiate(_toastMessagePrefab.gameObject, transform.parent).GetComponent<ToastMessage>();
        toast.Show("Изменения приняты");
    }

    private void StartProject()
    {
        if (!CheckValidData())
            return;

        string dateStr = string.Empty;
        if (!_birthdayInputFiled.GetComponent<InputFieldDateValidate>().IsTextEmpty())
        {
            dateStr = _birthdayInputFiled.GetComponent<InputFieldDateValidate>().Date.ToString();
        }

        HorseData horse = new HorseData(
            _horseNameInputField.text,
            _sexText.text,
            dateStr,
            _descriptionInputField.text,
            _ownerNameInputFiled.text,
            _localityInputFiled.text,
            _phoneNumberInputField.text,
            new List<string>()
        );
        
        HorseSaveData save = new("Новый проект", "Стартовое сохранение без каких либо изменений", DateTime.Now, null, horse.Id);

        Storage storage = new Storage(GameManager.Instance.Settings.PathSave);
        storage.AddHorse(horse);
        storage.AddHorseSave(save);

        SceneParameters.AddParam(horse);
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
                continue;

            if (!validator.ValidateInput(input.text))
                countInvalid++;
        }

        if (countInvalid > 0)
            return false;
        else
            return true;
    }
}
