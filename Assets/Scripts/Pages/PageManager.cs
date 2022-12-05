using System;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    [SerializeField] private Page _startPage;
    [SerializeField] private Page _mainMenu;
    [SerializeField] private Page _newProjectHorsePage;
    [SerializeField] private Page _loadHorsePage;

    public Page CurrentPage { get; private set; }

    private void Start()
    {
        (CurrentPage = _startPage).Open();

        //Test();
    }

    private void Test()
    {
        Storage storage = new Storage();
        for (int i = 0; i < 50; i++)
        {
            HorseData horse = new HorseData($"Ford - {i}", "Жеребец", new DateTime(1970 + i, 1, 1), "Same data", $"Owner Ford - {i}", $"City - {i}", new List<string>());
            storage.AddHorse(horse);

            for (int j = 0; j < 50; j++)
            {
                HorseSaveData save = new HorseSaveData($"Name - {j}", "Same data", new DateTime(2022, 11, 22, 11, 00 + j, 0), null, horse.Id);
                storage.AddHorseSave(save);
            }
        }
    }

    public void OpenPage(Page page)
    {
        CurrentPage.Close();

        page.Open();
        CurrentPage = page;
    }

    public void OpenPage(Page page, bool isPopUp)
    {
        if (!isPopUp)
            ClosePage(CurrentPage);

        page.Open();
        CurrentPage = page;
    }

    public void ClosePage(Page page)
    {
        page.Close();
    }
}
