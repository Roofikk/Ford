using Ford.SaveSystem;
using Ford.SaveSystem.Data;
using Ford.SaveSystem.Ver2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadHorsePage : Page
{
    [SerializeField] private CustomScrollRect _horsesScrollRect;
    [SerializeField] private TextMeshProUGUI _emptyHorseListText;
    [SerializeField] private HorseLoadElement _horseUiPrefab;
    [SerializeField] private ToggleGroup _horseToggleGroup;

    [Space(10)]
    [SerializeField] private CustomScrollRect _savesScroll;
    [SerializeField] private TextMeshProUGUI _emptySaveListText;
    [SerializeField] private SaveElementUI _savePrefab;
    [SerializeField] private ToggleGroup _saveToggleGroup;

    [Space(10)]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _refreshButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _deleteButton;

    [Space(10)]
    [SerializeField] private HorsePage _horseInfoPage;
    [SerializeField] private Page _saveInfoPage;

    private Dictionary<long, HorseLoadElement> _horseInfoDict = new();
    private Dictionary<long, SaveElementUI> _saveInfoDict = new();

    private int _below = 0;
    private int _amount = 20;

    private void Start()
    {
        _closeButton.onClick.AddListener(Close);
        _closeButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(PageManager.Instance.StartPage); });
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveListener(Close);
        _horsesScrollRect.OnUploadFocused -= () => { GetNextHorses(false); };
        _horsesScrollRect.OnRefreshFocused -= RefreshHorsePage;
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        _below = 0;
        ActivateButtons(false);
        ClearHorses();
        GetNextHorses();
        _horsesScrollRect.OnUploadFocused += () => { GetNextHorses(false); } ;
        _horsesScrollRect.OnRefreshFocused += RefreshHorsePage;
    }

    public override void Close()
    {
        base.Close();

        ActivateButtons(false);
        ClearHorses();
        ClearSaves();
        _horsesScrollRect.OnUploadFocused -= () => { GetNextHorses(false); };
        _horsesScrollRect.OnRefreshFocused -= RefreshHorsePage;
    }

    private void ShowEmptyHorseScroll(bool enable)
    {
        _emptyHorseListText.gameObject.SetActive(enable);
        _horsesScrollRect.vertical = !enable;
    }

    private void ShowEmptySaveScroll(bool enable)
    {
        _emptySaveListText.gameObject.SetActive(enable);
        _savesScroll.vertical = !enable;
    }

    private void RefreshHorsePage()
    {
        GetNextHorses(true);
    }

    private void ClearHorses()
    {
        foreach (var pair in _horseInfoDict)
        {
            Destroy(pair.Value.gameObject);
        }

        _horseInfoDict.Clear();
    }

    private void ClearSaves()
    {
        foreach (var pair in _saveInfoDict)
        {
            Destroy(pair.Value.gameObject);
        }

        ActivateButtons(false);
        _saveInfoDict.Clear();
    }

    private void FillHorseList(List<HorseBase> horses)
    {
        foreach (var horse in horses)
        {
            var horseElement = Instantiate(_horseUiPrefab, _horsesScrollRect.content)
                .Initiate(horse,
                    () => { GetNextSaves(horse); },
                    () => { OpenHorseInfoPage(horse.HorseId); },
                    _horseToggleGroup);
            _horseInfoDict.Add(horse.HorseId, horseElement);
        }
    }

    private void GetNextHorses(bool isRefresh = false)
    {
        _refreshButton.onClick.RemoveAllListeners();

        if (isRefresh)
        {
            _below = 0;
            _horsesScrollRect.OnRefreshFocused -= RefreshHorsePage;
        }

        if (_below == 0)
        {
            _horsesScrollRect.SetLoadIcon();
        }

        StorageSystem storage = new();
        _horsesScrollRect.ActivatePagination(false);
        _horsesScrollRect.LockScroll(true);
        storage.GetHorses(_below, _amount).RunOnMainThread((result) =>
        {
            _refreshButton.onClick.AddListener(RefreshHorsePage);
            if (isRefresh)
            {
                ClearHorses();
                _horsesScrollRect.OnRefreshFocused += RefreshHorsePage;
            }

            _horsesScrollRect.LockScroll(false);
            _horsesScrollRect.DisableRefreshElement();

            if (result.Count > 0)
            {
                FillHorseList(result.ToList());

                if (isRefresh)
                {
                    StartCoroutine(SetAllToggleOff(_horseToggleGroup));
                    ClearSaves();
                }
            }

            ShowEmptyHorseScroll(result.Count == 0);

            if (result.Count != _amount)
            {
                _horsesScrollRect.OnUploadFocused -= () => { GetNextHorses(false); } ;
                _horsesScrollRect.ActivatePagination(false);
            }
            else
            {
                _horsesScrollRect.ActivatePagination(true);
            }
        });

        _below += _amount;
    }

    private System.Collections.IEnumerator SetAllToggleOff(ToggleGroup toggleGroup)
    {
        toggleGroup.allowSwitchOff = true;
        yield return new WaitForEndOfFrame();
        toggleGroup.SetAllTogglesOff();
        toggleGroup.allowSwitchOff = false;
    }

    private void GetNextSaves(HorseBase horse)
    {
        ClearSaves();
        var saves = horse.Saves.ToList();

        foreach (var save in saves)
        {
            var self = _horseInfoDict[save.HorseId].HorseData.Self;
            var saveElement = Instantiate(_savePrefab, _savesScroll.content)
                .Initiate(save, _saveToggleGroup,
                () => { ActivateButtons(true, Enum.Parse<UserAccessRole>(self.AccessRole)); },
                () => { OpenSaveInfoPage(save.SaveId); });

            _saveInfoDict.Add(save.SaveId, saveElement);
        }

        _loadButton.onClick.RemoveAllListeners();
        _editButton.onClick.RemoveAllListeners();

        _loadButton.onClick.AddListener(() =>
        {
            var saveInfo = _saveToggleGroup.GetFirstActiveToggle().GetComponent<SaveElementUI>().SaveData;
            var horseInfo = _horseToggleGroup.GetFirstActiveToggle().GetComponent<HorseLoadElement>().HorseData;

            LoadSave(horseInfo, saveInfo);
        });
        _editButton.onClick.AddListener(() =>
        {
            OpenSaveInfoPage(_saveToggleGroup.GetFirstActiveToggle().GetComponent<SaveElementUI>().SaveData.SaveId);
        });

        ShowEmptySaveScroll(saves.Count == 0);
    }

    private void ActivateButtons(bool enable, UserAccessRole userAccessRole = UserAccessRole.Viewer)
    {
        _loadButton.interactable = enable;
        _editButton.interactable = enable;

        if (_saveInfoDict.Count > 1)
        {
            _deleteButton.interactable = enable;
        }
        else
        {
            _deleteButton.interactable = false;
        }

        if (userAccessRole == UserAccessRole.Viewer)
        {
            _editButton.interactable = false;
            _deleteButton.interactable = false;
        }
    }
    
    public void UpdateHorseLoadElenets(HorseBase horse)
    {
        _horseInfoDict[horse.HorseId].UpdateHorseInfo(horse);
    }

    public void UpdateSaveLoadElement(SaveInfo save)
    {
        var foundSave = _horseInfoDict[save.HorseId].HorseData.Saves.SingleOrDefault(s => s.SaveId == save.SaveId);

        if (_saveInfoDict.TryGetValue(save.SaveId, out var saveElement))
        {
            saveElement.UpdateInfo(save);
        }
    }

    public void DeleteHorseLoadElement(long horseId)
    {
        Destroy(_horseInfoDict[horseId].gameObject);
        _horseInfoDict.Remove(horseId);
    }

    public void DeleteSaveLoadElement(long saveId)
    {
        Destroy(_saveInfoDict[saveId].gameObject);
        _saveInfoDict.Remove(saveId);
    }

    private void OpenHorseInfoPage(long horseId)
    {
        PageManager.Instance.OpenPage(_horseInfoPage, new HorsePageParam(PageMode.Read, _horseInfoDict[horseId].HorseData), 2);
        _horseInfoPage.OnHorseInfoUpdated += UpdateHorseLoadElenets;
        _horseInfoPage.OnDeleted += DeleteHorseLoadElement;
    }

    private void OpenSaveInfoPage(long saveId)
    {
        var saveInfo = _saveInfoDict[saveId].SaveData;
        PageManager.Instance.OpenPage(_saveInfoPage, new SavePanelParam(PageMode.Read, 
            saveInfo, _saveInfoDict.Count > 1, _horseInfoDict[saveInfo.HorseId].HorseData.Self), 2);
        ((SavePanel)_saveInfoPage).OnSaveUpdated += UpdateSaveLoadElement;
        ((SavePanel)_saveInfoPage).OnSaveDeleted += DeleteSaveLoadElement;
    }

    private void LoadSave(HorseBase horseInfo, ISaveInfo saveInfo)
    {
        PageManager.Instance.DisplayLoadingPage(true, 4);
        StorageSystem storage = new();
        storage.GetSave(saveInfo.HorseId, saveInfo.SaveId)
            .RunOnMainThread((result) =>
            {
                if (result == null)
                {
                    ToastMessage.Show("Не удалость загрузить сохранение");
                    PageManager.Instance.DisplayLoadingPage(false);
                    return;
                }

                SceneParameters.AddParam(horseInfo);

                var saveBonesData = new SaveBonesData()
                {
                    SaveId = saveInfo.SaveId,
                };

                foreach (var bone in result.Bones)
                {
                    saveBonesData.Bones.Add(bone);
                }

                SceneParameters.AddParam(saveBonesData);

                AsyncOperation loadingScene = SceneManager.LoadSceneAsync(1);
                loadingScene.completed += (res) =>
                {
                    PageManager.Instance.DisplayLoadingPage(false);
                };
            });
    }
}
