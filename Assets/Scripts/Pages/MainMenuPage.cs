using Ford.SaveSystem;
using Ford.SaveSystem.Ver2;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
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
    [Header("FOR TEST")]
    [SerializeField] private Button _historyTestButton;

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

        // Удалить после проверки!!!
        _historyTestButton.onClick.AddListener(() =>
        {
            Storage storage = new();
            HistoryPageParam param = new(storage.History);
            _pageManager.OpenPage(_pageManager.HistoryPage, param, 2);
        });

        _newProjectButton.onClick.AddListener(() => { _pageManager.ClosePage(this); });
        _loadProjectButton.onClick.AddListener(() => { _pageManager.ClosePage(this); });
        _settingsButton.onClick.AddListener(() => { _pageManager.ClosePage(this); });
        _guideButton.onClick.AddListener(() => { _pageManager.ClosePage(this); });

        Player.OnChangedAuthState += UpdatePage;
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

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        UpdatePage();
    }

    public void UpdatePage()
    {
        _authButton.onClick.RemoveAllListeners();
        StorageSystem storage = new();

        if (storage.CurrentState == SaveSystemStateEnum.Offline)
        {
            if (Player.IsLoggedIn)
            {
                _authorizeText.text = "Переподключиться";
                _authButton.onClick.AddListener(() =>
                {
                    TryReconnect();
                });
            }
            else
            {
                _authorizeText.text = "Авторизация";
                _authButton.onClick.AddListener(() => { _pageManager.OpenPage(_loginPage, 2); });
            }
        }
        else
        {
            _authorizeText.text = $"Приветствую, {Player.UserData.FirstName}";
            _authButton.onClick.AddListener(() => { _pageManager.OpenPage(_userInfoPage, 2); });
        }
    }

    private void TryReconnect()
    {
        PageManager.Instance.DisplayLoadingPage(true, 2);

        StorageSystem storage = new();
        storage.CanChangeState().RunOnMainThread(result =>
        {
            if (result)
            {
                storage.ChangeState(SaveSystemStateEnum.Authorized);
                Storage store = new();

                if (store.History.History.Count > 0)
                {
                    PageManager.Instance.OpenWarningPage(new WarningData(
                        "Предупреждение",
                        "У вас имеются некоторые изменения, пока вы находились вне сети.\n" +
                        "Желаете посмотреть и применить их к уже имеющимся?\n" +
                        "ОТМЕНА приведет к их уничтожению",
                        () => { ShowHistoryPage(store.History); },
                        onCancel: () => { RawApplyTransition(storage); }), 2);

                    PageManager.Instance.DisplayLoadingPage(false);
                }
                else
                {
                    ApplyTransition(storage);
                }
            }
            else
            {
                ToastMessage.Show("Не удалось подключиться. Попробуйте авторизоваться заново");
                PageManager.Instance.DisplayLoadingPage(false);
            }
        });
    }

    private void ApplyTransition(StorageSystem storage)
    {
        storage.ApplyTransition().RunOnMainThread(result =>
        {
            if (result)
            {
                ToastMessage.Show($"Подключение завершено.\nС возвращением, {Player.UserData.FirstName}");
            }
            else
            {
                ToastMessage.Show("Произошла ошибка");
            }

            PageManager.Instance.DisplayLoadingPage(false);
        });
    }

    private void RawApplyTransition(StorageSystem storage)
    {
        storage.RawApplyTransition().RunOnMainThread(result =>
        {
            if (result)
            {
                ToastMessage.Show($"Подключение завершено.\nС возвращением, {Player.UserData.FirstName}");
            }
            else
            {
                ToastMessage.Show("Произошла ошибка");
            }

            PageManager.Instance.DisplayLoadingPage(false);
        });
    }

    private void ShowHistoryPage(StorageHistory history)
    {
        var param = new HistoryPageParam(history);
        PageManager.Instance.OpenPage(_pageManager.HistoryPage, param, 2);
    }

    public override void Close()
    {
        base.Close();

        _authButton.onClick.RemoveAllListeners();
    }
}