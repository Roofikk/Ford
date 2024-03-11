using Ford.SaveSystem;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] private Button _removeButton;

    [Space(10)]
    [SerializeField] private HorsePage _horseInfoPage;
    [SerializeField] private Page _saveInfoPage;

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

        PageManager.Instance.DisplayLoadingPage(true);

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

        foreach (Transform t in _horsesScrollRect.content.transform)
        {
            Destroy(t.gameObject);
        }
    }

    private void ClearHorses()
    {

    }

    private void ClearSaves()
    {

    }

    private void FillHorseList(List<HorseBase> horses)
    {
        horses.Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));

        foreach (var horse in horses)
        {
            var horseElement = Instantiate(_horseUiPrefab, _horsesScrollRect.content)
                .Initiate(horse,
                    () => { FillSaveList(horse); },
                    () => { OpenHorseInfoPage(horse); },
                    _saveToggleGroup);
        }
    }

    private void FillSaveList(HorseBase horse)
    {

    }

    public void OpenHorseInfoPage(HorseBase horse)
    {
        PageManager.Instance.OpenPage(_horseInfoPage, new HorsePageParam(HorsePageMode.Read, horse), 2);
    }

    public void OpenSaveInfoPage(SaveData save)
    {

    }
}
