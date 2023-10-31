using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadHorsePage : Page
{
    [SerializeField] private ScrollRect _horsesScrollRect;
    [SerializeField] private Button _backButton;
    [SerializeField] private LoadSavesPage _savesPage;

    [SerializeField] private HorseLoadUI _horseUiPrefab;
    [SerializeField] private ToggleGroup _toggleGroup;

    private void Start()
    {
        _backButton.onClick.AddListener(Close);
        _backButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(PageManager.Instance.StartPage); });
    }

    private void OnDestroy()
    {
        _backButton.onClick.RemoveListener(Close);
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        Storage storage = new(GameManager.Instance.Settings.PathSave);
        List<HorseData> horses = storage.GetHorses();
        horses.Sort((x, y) => x.DateCreation.CompareTo(y.DateCreation));

        foreach(var horse in horses)
        {
            var horseUi = Instantiate(_horseUiPrefab.gameObject, _horsesScrollRect.content).GetComponent<HorseLoadUI>();

            UnityAction<HorseData> onHorseClicked = new(_savesPage.Open);
            horseUi.Initiate(horse, onHorseClicked, _toggleGroup);
            horseUi.OnDestroyed += OnHorseRemoved;
            _savesPage.HorseInfoPanel.HorseUpdated += horseUi.UpdateHorseInfo;
        }
    }

    public override void Close()
    {
        base.Close();

        foreach (Transform t in _horsesScrollRect.content.transform)
        {
            Destroy(t.gameObject);
        }

        _savesPage.Close();
    }

    private void OnHorseRemoved()
    {
        _savesPage.Close();
    }
}
