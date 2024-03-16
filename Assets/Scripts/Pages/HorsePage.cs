using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Ford.SaveSystem.Ver2.Dto;
using Ford.SaveSystem;
using System.Linq;
using Ford.WebApi.Data;

public class HorsePage : Page
{
    [Header("Header")]
    [SerializeField] private TextMeshProUGUI _headerText;

    [Header("Input fields")]
    [SerializeField] private TMP_InputField _horseNameInputField;
    [SerializeField] private TMP_Dropdown _sexDropdown;
    [SerializeField] private TMP_InputField _birthdayInputFiled;
    [SerializeField] private TMP_InputField _descriptionInputField;
    [SerializeField] private TMP_InputField _countryInputFiled;
    [SerializeField] private TMP_InputField _cityInputFiled;
    [SerializeField] private TMP_InputField _regionInputFiled;
    [SerializeField] private OwnerPanel _ownerPanel;

    [Space(10)]
    [Header("Buttons")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _declineButton;

    [Space(10)]
    [SerializeField] private LoadScenePage _loadScenePage;

    [Space(10)]
    [Header("Toast message")]
    [SerializeField] private ToastMessage _toastMessagePrefab;

    private List<TMP_InputField> _inputFields;
    private List<FieldMaskValidate> _validators;
    private HorseBase _horseBase;

    public event Action<HorseBase> OnDeleted;
    public event Action<HorseBase> OnApply;

    private void Start()
    {
        _inputFields ??= transform.GetComponentsInChildren<TMP_InputField>().ToList();
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

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        if (_horseBase == null)
        {
            PageManager.Instance.OpenPage(_ownerPanel, popUpLevel);
            SetNormalMode();
        }
    }

    /// <summary>
    /// Open page with field inputs. This methods for update horse save, not create horse
    /// </summary>
    /// <typeparam name="HorseData"></typeparam>
    /// <param name="data"></param>
    public override void Open<T>(T horseData, int popUpLevel)
    {
        if (horseData is not HorsePageParam param)
        {
            Debug.LogError($"Need {typeof(HorsePageParam)} param");
            return;
        }

        _horseBase = param.Horse;
        switch (param.HorsePageMode)
        {
            case HorsePageMode.Read:
                SetReadMode();
                PageManager.Instance.OpenPage(_ownerPanel, 
                    new OwnerPanelParam(OwnerPanelMode.Read, param.Horse.Users.ToList()), popUpLevel + 1);
                break;
            case HorsePageMode.Write:
                SetWriteMode();
                PageManager.Instance.OpenPage(_ownerPanel, 
                    new OwnerPanelParam(OwnerPanelMode.Write, param.Horse.Users.ToList()), popUpLevel + 1);
                break;
        }

        _closeButton.onClick.AddListener(() =>
        {
            PageManager.Instance.ClosePage(this);
        });

        //Field inputs
        _headerText.text = _horseBase.Name;
        _horseNameInputField.text = param.Horse.Name;
        _sexDropdown.value = (int)Enum.Parse<HorseSex>(param.Horse.Sex);
        _birthdayInputFiled.text = param.Horse.BirthDate?.ToString("dd.MM.yyyy");
        _descriptionInputField.text = param.Horse.Description;
        _countryInputFiled.text = param.Horse.Country;
        _cityInputFiled.text = param.Horse.City;
        _regionInputFiled.text = param.Horse.Region;

        base.Open(horseData, popUpLevel);
    }

    public override void Close()
    {
        base.Close();
        _horseBase = null;
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
        _declineButton.onClick.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();
    }

    private void SetNormalMode()
    {
        _inputFields ??= GetComponentsInChildren<TMP_InputField>().ToList();
        foreach (var field in _inputFields)
        {
            field.SetInteractable(true);
        }

        _headerText.text = "Новый проект";
        _sexDropdown.interactable = true;

        _applyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Начать";
        _applyButton.onClick.AddListener(StartProject);

        _declineButton.GetComponentInChildren<TextMeshProUGUI>().text = "Назад";
        _declineButton.onClick.AddListener(Close);
        _declineButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(PageManager.Instance.StartPage); });
        _closeButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(PageManager.Instance.StartPage); });
    }

    private void SetReadMode()
    {
        _inputFields ??= GetComponentsInChildren<TMP_InputField>().ToList();
        foreach (var field in _inputFields)
        {
            field.SetInteractable(false);
        }

        _sexDropdown.interactable = false;

        _applyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Изменить";
        var copy = new HorseBase(_horseBase);

        _applyButton.onClick.AddListener(() =>
        {
            PageManager.Instance.ClosePage(this);
            PageManager.Instance.OpenPage(this, new HorsePageParam(HorsePageMode.Write, copy), 2);
        });

        _declineButton.GetComponentInChildren<TextMeshProUGUI>().text = "Удалить";
        _declineButton.onClick.AddListener(Close);
    }

    private void SetWriteMode()
    {
        _inputFields ??= GetComponentsInChildren<TMP_InputField>().ToList();
        foreach (var field in _inputFields)
        {
            field.SetInteractable(true);
        }

        _sexDropdown.interactable = true;

        _applyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Сохранить";
        _applyButton.onClick.AddListener(EditHorse);

        var copy = new HorseBase(_horseBase);

        _declineButton.GetComponentInChildren<TextMeshProUGUI>().text = "Отмена";
        _declineButton.onClick.AddListener(() =>
        {
            PageManager.Instance.ClosePage(this);
            PageManager.Instance.OpenPage(this, new HorsePageParam(HorsePageMode.Read, copy), 2);
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
            BirthDate = _birthdayInputFiled.GetComponent<InputFieldDateValidator>().Date,
            Sex = ((HorseSex)_sexDropdown.value).ToString(),
            Country = _countryInputFiled.text,
            City = _cityInputFiled.text,
            Region = _regionInputFiled.text,
            OwnerName = _ownerPanel.OwnerName,
            OwnerPhoneNumber = _ownerPanel.OwnerPhoneNumber,
        };

        //Storage storage = new(GameManager.Instance.Settings.PathSave);
        //var horseData = storage.UpdateHorse(horse);

        //OnApply?.Invoke(horseData);
        Close();

        ToastMessage toast = Instantiate(_toastMessagePrefab.gameObject, transform.parent).GetComponent<ToastMessage>();
        toast.Show("Изменения приняты");
    }

    private void StartProject()
    {
        if (!CheckValidData())
            return;

        CreateSaveDto save = new()
        {
            Header = "Новый проект",
            Description = "Стартовое сохранение без каких либо изменений",
            Date = DateTime.Now,
            Bones = null
        };

        CreationHorse horse = new()
        {
            Name = _horseNameInputField.text,
            Sex = ((HorseSex)_sexDropdown.value).ToString(),
            BirthDate = _birthdayInputFiled.GetComponent<InputFieldDateValidator>().Date,
            Description = _descriptionInputField.text,
            Country = _countryInputFiled.text,
            City = _cityInputFiled.text,
            Region = _regionInputFiled.text,
        };

        if (_ownerPanel.Owner == null)
        {
            horse.OwnerName = _ownerPanel.OwnerName;
            horse.OwnerPhoneNumber = _ownerPanel.OwnerPhoneNumber;

            foreach (var user in _ownerPanel.Users)
            {
                user.IsOwner = false;
            }
        }
        else if (_ownerPanel.Owner.Id == Player.UserData.UserId)
        {
            horse.Users.Add(new()
            {
                UserId = _ownerPanel.Owner.Id,
                IsOwner = true,
                RuleAccess = UserRoleAccess.Creator.ToString(),
            });
        }
        else
        {
            horse.Users.Add(new()
            {
                UserId = _ownerPanel.Owner.Id,
                IsOwner = true,
                RuleAccess = _ownerPanel.AccessRole,
            });
        }

        foreach (var user in _ownerPanel.Users)
        {
            horse.Users.Add(new()
            {
                UserId = user.Id,
                IsOwner = false,
                RuleAccess = user.AccessRole,
            });
        }

        horse.Saves.Add(new()
        {
            Header = "Начальное сохранение",
            Description = "Это начальное сохранение, оно всегда создается при создании нового проекта",
            Date = DateTime.Now,
        });

        PageManager.Instance.DisplayLoadingPage(true, 4);
        StorageSystem storage = new();
        storage.CreateHorse(horse).RunOnMainThread((result) =>
        {
            var horse = result;

            if (result == null)
            {
                throw new Exception("Все пошло по пизде");
            }

            SceneParameters.AddParam(horse);
            AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);
            loadingOperation.completed += (res) =>
            {
                PageManager.Instance.DisplayLoadingPage(false);
            };
        });
    }

    private void DeleteHorse()
    {
        PageManager.Instance.DisplayLoadingPage(true, 6);
        StorageSystem storage = new();

        storage.DeleteHorse(_horseBase.HorseId).RunOnMainThread((result) =>
        {
            if (result)
            {
                OnDeleted?.Invoke(_horseBase);
            }
            else
            {
                OnDeleted?.Invoke(null);
            }

            PageManager.Instance.DisplayLoadingPage(false);
        });
    }

    public bool CheckValidData()
    {
        foreach (var validator in _validators)
        {
            if (!validator.ValidateInput())
                return false;
        }

        return true;
    }
}
