using Ford.SaveSystem.Ver2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuPage : Page
{
    [Space(10)]
    [Header("Buttons")]
    [SerializeField] private Button _newProjectButton;
    [SerializeField] private Button _newProjectByNewSaveSystem;
    [SerializeField] private Button _loadProjectButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _guideButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _startDevProjectButton;

    [Space(10)]
    [Header("Yandex disk button")]
    [SerializeField] private Button _yandexDiskButton;

    [Space(10)]
    [Header("Pages")]
    [SerializeField] private Page _newProjectPage;
    [SerializeField] private Page _loadProjectPage;
    [SerializeField] private Page _settingsPage;
    [SerializeField] private Page _guidePage;
    [SerializeField] private Page _yandexAuthorizePage;
    [SerializeField] private LoadScenePage _loadScenePage;

    private void Start()
    {
        _startDevProjectButton.onClick.AddListener(StartDevProject);

        _newProjectButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(_newProjectPage); });
        _newProjectByNewSaveSystem.onClick.AddListener(() => { PageManager.Instance.OpenPage(_newProjectPage); });
        _loadProjectButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(_loadProjectPage); });
        _settingsButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(_settingsPage); });
        _guideButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(_guidePage); });

        _newProjectButton.onClick.AddListener(() => { PageManager.Instance.ClosePage(this); });
        _newProjectByNewSaveSystem.onClick.AddListener(() => { PageManager.Instance.ClosePage(this); });
        _loadProjectButton.onClick.AddListener(() => { PageManager.Instance.ClosePage(this); });
        _settingsButton.onClick.AddListener(() => { PageManager.Instance.ClosePage(this); });
        _guideButton.onClick.AddListener(() => { PageManager.Instance.ClosePage(this); });

        //Yandex disk settings
        _yandexDiskButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(_yandexAuthorizePage, 1); });

        YandexDiskToken yandexDiskToken = new();
    }

    private void OnDestroy()
    {
        _startDevProjectButton.onClick.RemoveAllListeners();
        _newProjectButton.onClick.RemoveAllListeners();
        _loadProjectButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.RemoveAllListeners();
        _guideButton.onClick.RemoveAllListeners();
    }

    private void StartDevProject()
    {
        DevHorseData devHorse = new(
            "Dev Horse",
            "Unsex",
            "01.01.1970",
            "This project start by developer",
            "Developer",
            "Dev City",
            "+7 (123) 456 78 90",
            new List<string>()
        );

        Storage storage = new(GameManager.Instance.Settings.PathSave);
        DevHorseSaveData devHorseSaveData = null;//storage.DevGetHorseState(devHorse.Id);

        if (devHorseSaveData != null)
        {
            SceneParameters.AddParam(devHorseSaveData);
        }

        SceneParameters.AddParam(devHorse);

        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);
        _loadScenePage.Open(loadingOperation);
    }
}