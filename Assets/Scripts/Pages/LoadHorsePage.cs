using Ford.SaveSystem;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadHorsePage : Page
{
    [SerializeField] private ScrollRect _horsesScrollRect;
    [SerializeField] private TextMeshProUGUI _emptyHorseListText;
    [SerializeField] private HorseLoadElement _horseUiPrefab;
    [SerializeField] private ToggleGroup _horseToggleGroup;

    [Space(10)]
    [SerializeField] private ScrollRect _savesScroll;
    [SerializeField] private TextMeshProUGUI _emptySaveListText;
    [SerializeField] private SaveElementUI _savePrefab;
    [SerializeField] private ToggleGroup _saveToggleGroup;

    [Space(10)]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _deleteButton;

    [Space(10)]
    [SerializeField] private HorsePage _horseInfoPage;
    [SerializeField] private Page _saveInfoPage;

    private Dictionary<long, HorseLoadElement> _horseInfoDict = new();
    private Dictionary<long, SaveElementUI> _saveInfoDict = new();

    private void Start()
    {
        _closeButton.onClick.AddListener(Close);
        _closeButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(PageManager.Instance.StartPage); });
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveListener(Close);
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        PageManager.Instance.DisplayLoadingPage(true, 2);
        ActivateButtons(false);

        StorageSystem storage = new();
        storage.GetHorses().RunOnMainThread((result) =>
        {
            List<HorseBase> horses = result.ToList();
            FillHorseList(horses);

            PageManager.Instance.DisplayLoadingPage(false);
        });
    }

    public override void Close()
    {
        base.Close();
        ClearHorses();
        ClearSaves();
        ActivateButtons(false);
    }

    private void ClearHorses()
    {
        _horseInfoDict.Clear();
        foreach (Transform t in _horsesScrollRect.content.transform)
        {
            Destroy(t.gameObject);
        }
    }

    private void ClearSaves()
    {
        _saveInfoDict.Clear();
        foreach (Transform t in _savesScroll.content.transform)
        {
            Destroy(t.gameObject);
        }
    }

    private void FillHorseList(List<HorseBase> horses)
    {
        ClearHorses();
        horses.Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));

        foreach (var horse in horses)
        {
            var horseElement = Instantiate(_horseUiPrefab, _horsesScrollRect.content)
                .Initiate(horse,
                    () => { FillSaveList(horse); },
                    () => { OpenHorseInfoPage(horse.HorseId); },
                    _horseToggleGroup);
            _horseInfoDict.Add(horse.HorseId, horseElement);
        }

        _emptyHorseListText.gameObject.SetActive(horses.Count == 0);
    }

    private void FillSaveList(HorseBase horse)
    {
        ClearSaves();
        var saves = horse.Saves.ToList();
        saves.Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));

        foreach (var save in saves)
        {
            var saveElement = Instantiate(_savePrefab, _savesScroll.content)
                .Initiate(save, _saveToggleGroup,
                () => { ActivateButtons(true); },
                () => { OpenSaveInfoPage(save.SaveId); });

            _saveInfoDict.Add(save.SaveId, saveElement);
        }

        _emptySaveListText.gameObject.SetActive(saves.Count == 0);
    }

    private void ActivateButtons(bool enable)
    {
        _loadButton.interactable = enable;
        _editButton.interactable = enable;
        _deleteButton.interactable = enable;
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

    }

    private void OpenHorseInfoPage(long horseId)
    {
        PageManager.Instance.OpenPage(_horseInfoPage, new HorsePageParam(PageMode.Read, _horseInfoDict[horseId].HorseData), 2);
        _horseInfoPage.OnHorseInfoUpdated += UpdateHorseLoadElenets;
    }

    private void OpenSaveInfoPage(long saveId)
    {
        PageManager.Instance.OpenPage(_saveInfoPage, new SavePanelParam(PageMode.Read, _saveInfoDict[saveId].SaveData), 2);
        ((SavePanel)_saveInfoPage).OnSaveUpdated += UpdateSaveLoadElement;
    }
}
