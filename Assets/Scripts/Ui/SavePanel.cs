using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Ford.SaveSystem;

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
        OpenMode(saveParam.Mode, saveParam.SaveInfo, popUpLevel);
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);
    }

    public override void Close()
    {
        base.Close();

        _declineButton.onClick.RemoveAllListeners();
        _applyButton.onClick.RemoveAllListeners();
    }

    private void OpenMode(PageMode mode, ISaveInfo saveInfo, int popUpLevel)
    {
        _inputFields ??= GetComponentsInChildren<TMP_InputField>(true).ToList();
        FillData(saveInfo);

        switch (mode)
        {
            case PageMode.Read:
                foreach (var input in _inputFields)
                {
                    input.SetInteractable(false);
                }

                _additionalPanel.gameObject.SetActive(true);
                _creationDateField.text = saveInfo.CreationDate.ToString("dd.MM.yyyy HH:mm");
                _lastUpdateField.text = saveInfo.LastUpdate.ToString("dd.MM.yyyy HH:mm");

                _applyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Изменить";
                _applyButton.onClick.AddListener(() =>
                {
                    PageManager.Instance.ClosePage(this);
                    PageManager.Instance.OpenPage(this, new SavePanelParam(PageMode.Write, saveInfo), popUpLevel);
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
                            PageManager.Instance.CloseWarningPage();
                        }), 4);
                });
                break;
            case PageMode.Write:
                foreach (var input in _inputFields)
                {
                    input.SetInteractable(true);
                }

                _additionalPanel.gameObject.SetActive(false);

                _applyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Сохранить";
                _declineButton.GetComponentInChildren<TextMeshProUGUI>().text = "Отмена";

                if (saveInfo is FullSaveInfo fullSaveInfo)
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
                        PageManager.Instance.OpenPage(this, new SavePanelParam(PageMode.Read, saveInfo), popUpLevel);
                    });
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
            HorseId = SaveInfo.HorseId,
            SaveId = SaveInfo.SaveId,
            Header = _headerInputField.text,
            Date = _dateInputField.GetComponent<InputFieldDateValidator>().Date.Value,
            Description = _descriptionInputField.text,
            LastUpdate = DateTime.Now,
            CreationDate = SaveInfo.CreationDate,
            SaveFileName = ((SaveInfo)SaveInfo).SaveFileName,
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

        FullSaveInfo save = new()
        {
            Header = _headerInputField.text,
            Description = _descriptionInputField.text,
            Date = _dateInputField.GetComponent<InputFieldDateValidator>().Date.Value,
            CreationDate = DateTime.UtcNow,
            LastUpdate = DateTime.UtcNow,
            HorseId = SaveInfo.HorseId,
            Bones = ((FullSaveInfo)SaveInfo).Bones,
        };

        StorageSystem storage = new();
        storage.CreateSave(save).RunOnMainThread(result =>
        {
            ToastMessage.Show("Сохранение завершено");
            PageManager.Instance.ClosePage(this);
        });
    }

    private void DeleteSave()
    {
        PageManager.Instance.DisplayLoadingPage(true, 6);
        StorageSystem storage = new();
        storage.DeleteSave(SaveInfo.SaveId).RunOnMainThread(result =>
        {
            if (result)
            {
                ToastMessage.Show("Сохранение успешно удалено");
                OnSaveDeleted?.Invoke(SaveInfo.SaveId);
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
    public ISaveInfo SaveInfo { get; set; }
    public event Action OnSaveUpdated;
    public event Action OnSaveDeleted;

    public SavePanelParam(PageMode mode, ISaveInfo saveInfo)
    {
        Mode = mode;
        SaveInfo = saveInfo;
    }

    public SavePanelParam(PageMode mode, SaveInfo saveData)
    {
        Mode = mode;
        SaveInfo = saveData;
    }

    public SavePanelParam(PageMode mode, FullSaveInfo saveInfo)
    {
        Mode = mode;
        SaveInfo = saveInfo;
    }
}