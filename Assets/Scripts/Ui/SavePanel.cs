using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Ford.SaveSystem.Data;
using Ford.SaveSystem;
using Ford.SaveSystem.Data.Dtos;

public class SavePanel : Page
{
    [Space(10)]
    [Header("Fields")]
    [SerializeField] private TMP_InputField _headerInputField;
    [SerializeField] private TMP_InputField _dateInputField;
    [SerializeField] private TMP_InputField _descriptionInputField;

    [Space]
    [Header("Additional info")]
    [SerializeField] private GameObject _additionalPanel;
    [SerializeField] private TMP_InputField _creationDateField;
    [SerializeField] private TMP_InputField _lastUpdateField;

    [Space(10)]
    [Header("Buttons")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _declineButton;
    [SerializeField] private Button _applyButton;

    private List<TMP_InputField> _inputFields;
    public ISaveInfo SaveInfo { get; private set; }

    public event Action<SaveInfo> OnSaveUpdated;
    public event Action<long> OnSaveDeleted;
    
    private void Start()
    {
        _inputFields ??= GetComponentsInChildren<TMP_InputField>(true).ToList();
        _closeButton.onClick.AddListener(() =>
        {
            PageManager.Instance.ClosePage(this);
            OnSaveDeleted = null;
            OnSaveUpdated = null;
        });
    }

    public override void Open<T>(T param, int popUpLevel = 0)
    {
        base.Open(param, popUpLevel);

        if (param is not SavePanelParam saveParam)
        {
            return;
        }

        SaveInfo = saveParam.SaveInfo;
        OpenMode(saveParam, popUpLevel);
    }

    public override void Close()
    {
        base.Close();

        _declineButton.onClick.RemoveAllListeners();
        _applyButton.onClick.RemoveAllListeners();
    }

    private void OpenMode(SavePanelParam param, int popUpLevel)
    {
        _inputFields ??= GetComponentsInChildren<TMP_InputField>(true).ToList();
        FillData(param.SaveInfo);
        _declineButton.interactable = true;

        switch (param.Mode)
        {
            case PageMode.Read:
                foreach (var input in _inputFields)
                {
                    input.SetInteractable(false);
                }

                _additionalPanel.gameObject.SetActive(true);
                _creationDateField.text = param.SaveInfo.CreatedBy.Date.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
                _lastUpdateField.text = param.SaveInfo.LastModifiedBy.Date.ToLocalTime().ToString("dd.MM.yyyy HH:mm");

                _applyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Изменить";
                _applyButton.onClick.AddListener(() =>
                {
                    PageManager.Instance.ClosePage(this);
                    PageManager.Instance.OpenPage(this, new SavePanelParam(PageMode.Write, param.SaveInfo, param.CanDelete, param.Self), popUpLevel);
                });

                _declineButton.GetComponentInChildren<TextMeshProUGUI>().text = "Удалить";
                _declineButton.onClick.AddListener(() =>
                {
                    PageManager.Instance.OpenWarningPage(new(
                        "Удалить сохранение?",
                        "Вы уверены, что хотите удалить сохранение?\nОбратно вернуть его не будет возможности",
                        () =>
                        {
                            DeleteSave();
                            PageManager.Instance.ClosePage(this);
                            PageManager.Instance.CloseWarningPage();
                        }), 4);
                });

                if (Enum.Parse<UserAccessRole>(param.Self.AccessRole) > UserAccessRole.Viewer)
                {
                    _applyButton.interactable = true;
                    _declineButton.interactable = true;

                    _declineButton.interactable = param.CanDelete;
                }
                else
                {
                    _applyButton.interactable = false;
                    _declineButton.interactable = false;
                }
                break;
            case PageMode.Write:
                foreach (var input in _inputFields)
                {
                    input.SetInteractable(true);
                }

                _additionalPanel.gameObject.SetActive(false);

                _applyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Сохранить";
                _declineButton.GetComponentInChildren<TextMeshProUGUI>().text = "Отмена";

                if (param.SaveInfo is FullSaveInfo fullSaveInfo)
                {
                    _applyButton.onClick.AddListener(() =>
                    {
                        Save();
                    });

                    _declineButton.onClick.AddListener(() =>
                    {
                        PageManager.Instance.ClosePage(this);
                    });
                }
                else
                {
                    _applyButton.onClick.AddListener(() =>
                    {
                        EditSave();
                    });

                    _declineButton.onClick.AddListener(() =>
                    {
                        PageManager.Instance.ClosePage(this);
                        PageManager.Instance.OpenPage(this, new SavePanelParam(PageMode.Read, param.SaveInfo, param.CanDelete, param.Self), popUpLevel);
                    });
                }

                if (Enum.Parse<UserAccessRole>(param.Self.AccessRole) > UserAccessRole.Viewer)
                {
                    _applyButton.interactable = true;
                }
                else
                {
                    _applyButton.interactable = false;
                }
                break;
        }
    }

    private void FillData(ISaveInfo saveData)
    {
        if (saveData == null)
        {
            return;
        }

        _headerInputField.text = saveData.Header;
        _descriptionInputField.text = saveData.Description;
        _dateInputField.text = saveData.Date.ToString("dd.MM.yyyy");
    }

    private void EditSave()
    {
        if (!ValidateInput())
        {
            return;
        }

        StorageSystem storage = new();
        storage.UpdateSave(new()
        {
            SaveId = SaveInfo.SaveId,
            Header = _headerInputField.text,
            Date = _dateInputField.GetComponent<InputFieldDateValidator>().Date.Value,
            Description = _descriptionInputField.text,
        }).RunOnMainThread(result =>
        {
            SaveInfo = result;
            if (result != null)
            {
                ToastMessage.Show("Изменения применены");
            }
            else
            {
                ToastMessage.Show("Произошла ошибка при сохранении");
            }

            PageManager.Instance.ClosePage(this);
            OnSaveUpdated?.Invoke(result);

            OnSaveUpdated = null;
            OnSaveDeleted = null;
        });
    }

    private void Save()
    {
        if (!ValidateInput())
        {
            return;
        }

        CreatingSaveDto save = new()
        {
            CreationDate = DateTime.Now,
            Header = _headerInputField.text,
            Description = _descriptionInputField.text,
            Date = _dateInputField.GetComponent<InputFieldDateValidator>().Date.Value,
            HorseId = SaveInfo.HorseId,
        };

        foreach (var bone in ((FullSaveInfo)SaveInfo).Bones)
        {
            save.Bones.Add(bone);
        }

        StorageSystem storage = new();
        storage.CreateSave(save).RunOnMainThread(result =>
        {
            if (result == null)
            {
                ToastMessage.Show("Произошла ошибка при сохранении");
            }
            else
            {
                ToastMessage.Show("Сохранение завершено");
            }

            PageManager.Instance.ClosePage(this);
        });
    }

    private void DeleteSave()
    {
        PageManager.Instance.DisplayLoadingPage(true, 8);
        StorageSystem storage = new();
        var saveId = SaveInfo.SaveId;

        storage.DeleteSave(SaveInfo.SaveId).RunOnMainThread(result =>
        {
            if (result)
            {
                ToastMessage.Show("Сохранение успешно удалено");
                OnSaveDeleted?.Invoke(saveId);
            }
            else
            {
                ToastMessage.Show("Произошла ошибка при удалении");
            }

            PageManager.Instance.DisplayLoadingPage(false);

            OnSaveDeleted = null;
            OnSaveUpdated = null;
        });
    }

    private bool ValidateInput()
    {
        foreach (var input in _inputFields)
        {
            if (input.TryGetComponent<FieldMaskValidate>(out var validator))
            {
                if (!validator.ValidateInput())
                {
                    return false;
                }
            }
        }

        return true;
    }
}

public class SavePanelParam
{
    public PageMode Mode { get; }
    public ISaveInfo SaveInfo { get; }
    public bool CanDelete { get; }
    public HorseUserDto Self { get; }

    public SavePanelParam(PageMode mode, ISaveInfo saveInfo, bool canDelete, HorseUserDto self)
    {
        Mode = mode;
        SaveInfo = saveInfo;
        CanDelete = canDelete;
        Self = self;
    }
}