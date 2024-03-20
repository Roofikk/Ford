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

    public event Action<long> OnDeleted;
    public event Action<HorseBase> OnHorseInfoUpdated;

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
            case PageMode.Read:
                SetReadMode();
                PageManager.Instance.OpenPage(_ownerPanel,
                    new OwnerPanelParam(PageMode.Read, param.Horse.Users.ToList(), 
                        param.Horse.OwnerName, param.Horse.OwnerPhoneNumber), popUpLevel + 1);
                break;
            case PageMode.Write:
                SetWriteMode();
                PageManager.Instance.OpenPage(_ownerPanel, 
                    new OwnerPanelParam(PageMode.Write, param.Horse.Users.ToList(), 
                        param.Horse.OwnerName, param.Horse.OwnerPhoneNumber), popUpLevel + 1);
                break;
        }

        _closeButton.onClick.AddListener(() =>
        {
            PageManager.Instance.ClosePage(this);

            OnHorseInfoUpdated = null;
            OnDeleted = null;
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
            PageManager.Instance.OpenPage(this, new HorsePageParam(PageMode.Write, copy), 2);
        });

        _declineButton.GetComponentInChildren<TextMeshProUGUI>().text = "Удалить";
        _declineButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenWarningPage(new(
                "Удалить лошадь?",
                "Вы уверены, что хотите удалить лошадь?\nВернуть ее не будет возможности!",
                () =>
                {
                    DeleteHorse();
                    PageManager.Instance.ClosePage(this);
                    PageManager.Instance.CloseWarningPage();
                }), 4);
        });
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
            PageManager.Instance.OpenPage(this, new HorsePageParam(PageMode.Read, copy), 2);
        });
    }

    private void EditHorse()
    {
        if (!CheckValidData())
            return;

        UpdatingHorse horse = new()
        {
            HorseId = _horseBase.HorseId,
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

        PageManager.Instance.DisplayLoadingPage(true, 4);
        StorageSystem storage = new();
        storage.UpdateHorse(horse).RunOnMainThread(result =>
        {
            if (result == null)
            {
                ToastMessage.Show("Произошла ошибка при сохранении");
            }
            else
            {
                ToastMessage.Show("Изменения приняты");
            }

            PageManager.Instance.DisplayLoadingPage(false);
            PageManager.Instance.ClosePage(this);

            OnHorseInfoUpdated?.Invoke(result);

            OnHorseInfoUpdated = null;
            OnDeleted = null;
        });
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
        else if (_ownerPanel.Owner.UserId == Player.UserData.UserId)
        {
            horse.Users.Add(new()
            {
                UserId = _ownerPanel.Owner.UserId,
                IsOwner = true,
                RuleAccess = UserRoleAccess.Creator.ToString(),
            });
        }
        else
        {
            horse.Users.Add(new()
            {
                UserId = _ownerPanel.Owner.UserId,
                IsOwner = true,
                RuleAccess = _ownerPanel.AccessRole,
            });
        }

        foreach (var user in _ownerPanel.Users)
        {
            horse.Users.Add(new()
            {
                UserId = user.UserId,
                IsOwner = false,
                RuleAccess = user.AccessRole,
            });
        }

        horse.Saves.Add(new()
        {
            Header = "Начальное сохранение",
            Description = "Это начальное сохранение, оно всегда создается при создании нового проекта",
            Date = DateTime.Now,
            CreationDate = DateTime.Now,
            LastUpdate = DateTime.Now,
        });

        PageManager.Instance.DisplayLoadingPage(true, 4);
        StorageSystem storage = new();
        storage.CreateHorse(horse).RunOnMainThread((result) =>
        {
            var horse = result;

            if (horse == null)
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
        PageManager.Instance.DisplayLoadingPage(true, 8);
        StorageSystem storage = new();
        var horseId = _horseBase.HorseId;

        storage.DeleteHorse(_horseBase.HorseId).RunOnMainThread((result) =>
        {
            if (result)
            {
                ToastMessage.Show("Удаление завершено");
                OnDeleted?.Invoke(horseId);
            }
            else
            {
                ToastMessage.Show("Произошла ошибка при удалении");
            }

            PageManager.Instance.DisplayLoadingPage(false);

            OnHorseInfoUpdated = null;
            OnDeleted = null;
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
