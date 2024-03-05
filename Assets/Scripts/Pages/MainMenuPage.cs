using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPage : Page
{
    [Space(10)]
    [Header("Buttons")]
    [SerializeField] private Button _newProjectButton;
    [SerializeField] private Button _loadProjectButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _guideButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _startDevProjectButton;
    [SerializeField] private Button _authButton;

    [Space(10)]
    [Header("Pages")]
    [SerializeField] private Page _newProjectPage;
    [SerializeField] private Page _loadProjectPage;
    [SerializeField] private Page _settingsPage;
    [SerializeField] private Page _guidePage;
    [SerializeField] private Page _userInfoPage;
    [SerializeField] private Page _loginPage;
    [SerializeField] private LoadScenePage _loadScenePage;

    [Space(10)]
    [Header("Other")]
    [SerializeField] private TextMeshProUGUI _authorizeText;

    private PageManager _pageManager => PageManager.Instance;

    private void Start()
    {
        _newProjectButton.onClick.AddListener(() => { _pageManager.OpenPage(_newProjectPage); });
        _loadProjectButton.onClick.AddListener(() => { _pageManager.OpenPage(_loadProjectPage); });
        _settingsButton.onClick.AddListener(() => { _pageManager.OpenPage(_settingsPage); });
        _guideButton.onClick.AddListener(() => { _pageManager.OpenPage(_guidePage); });

        _newProjectButton.onClick.AddListener(() => { _pageManager.ClosePage(this); });
        _loadProjectButton.onClick.AddListener(() => { _pageManager.ClosePage(this); });
        _settingsButton.onClick.AddListener(() => { _pageManager.ClosePage(this); });
        _guideButton.onClick.AddListener(() => { _pageManager.ClosePage(this); });

        Player.OnChangedAuthState += UpdatePage;
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        UpdatePage();
    }

    public void UpdatePage()
    {
        _authButton.onClick.RemoveAllListeners();

        if (Player.IsLoggedIn)
        {
            _authorizeText.text = $"Приветствую, {Player.UserData.FirstName}";
            _authButton.onClick.AddListener(() => { _pageManager.OpenPage(_userInfoPage, 1); });
        }
        else
        {
            _authorizeText.text = "Авторизация";
            _authButton.onClick.AddListener(() => { _pageManager.OpenPage(_loginPage, 1); });
        }
    }


    public override void Close()
    {
        base.Close();

        _authButton.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        _startDevProjectButton.onClick.RemoveAllListeners();
        _newProjectButton.onClick.RemoveAllListeners();
        _loadProjectButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.RemoveAllListeners();
        _guideButton.onClick.RemoveAllListeners();
        _authButton.onClick.RemoveAllListeners();

        Player.OnChangedAuthState -= UpdatePage;
    }



    //private void StartDevProject()
    //{
    //    DevHorseData devHorse = new(
    //        "Dev Horse",
    //        "Unsex",
    //        "01.01.1970",
    //        "This project start by developer",
    //        "Developer",
    //        "Dev City",
    //        "+7 (123) 456 78 90",
    //        new List<string>()
    //    );

    //    Storage storage = new(GameManager.Instance.Settings.PathSave);
    //    DevHorseSaveData devHorseSaveData = null;//storage.DevGetHorseState(devHorse.Id);

    //    if (devHorseSaveData != null)
    //    {
    //        SceneParameters.AddParam(devHorseSaveData);
    //    }

    //    SceneParameters.AddParam(devHorse);

    //    AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);
    //    _loadScenePage.Open(loadingOperation);
    //}
}