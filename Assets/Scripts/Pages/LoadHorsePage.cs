using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadHorsePage : Page
{
    [SerializeField] private ScrollRect _horsesScrollRect;
    [SerializeField] private Button _backButton;
    [SerializeField] private HorseLoadUI _horseUiPrefab;

    private void Start()
    {
        _backButton.onClick.AddListener(Close);
    }

    private void OnDestroy()
    {
        _backButton.onClick.RemoveListener(Close);
    }

    public override void Open()
    {
        base.Open();

        Storage storage = new Storage();
        List<HorseData> horses = storage.GetHorses();
        horses.Sort((x, y) => x.DateCreation.CompareTo(y.DateCreation));

        foreach(var horse in horses)
        {
            var horseUi = Instantiate(_horseUiPrefab.gameObject, _horsesScrollRect.content).GetComponent<HorseLoadUI>();
            horseUi.Initiate(horse);
        }
    }

    public override void Close()
    {
        base.Close();

        foreach (Transform t in _horsesScrollRect.content.transform)
        {
            Destroy(t.gameObject);
        }
    }
}
