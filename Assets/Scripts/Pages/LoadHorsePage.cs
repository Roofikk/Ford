using Ford.SaveSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadHorsePage : Page
{
    [SerializeField] private ScrollRect _horsesScrollRect;
    [SerializeField] private ScrollRect _savesScroll;
    [SerializeField] private ToggleGroup _toggleGroup;

    [Space(10)]
    [SerializeField] private HorseLoadElement _horseUiPrefab;
    [SerializeField] private SaveElementUI _savePrefab;

    [SerializeField] private Button _backButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _removeButton;


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

        PageManager.Instance.DisplayLoadingPage(true);

        StorageSystem storage = new();
        storage.GetHorses().RunOnMainThread((result) =>
        {
            List<HorseBase> horses = result.ToList();
            horses.Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));

            foreach (var horse in horses)
            {
                var horseElement = Instantiate(_horseUiPrefab, _horsesScrollRect.content)
                    .Initiate(horse, null, _toggleGroup);

                horseElement.OnDestroyed += OnHorseRemoved;
            }

            PageManager.Instance.DisplayLoadingPage(false);
        });
    }

    private void ClearHorses()
    {

    }

    private void ClearSaves()
    {

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
