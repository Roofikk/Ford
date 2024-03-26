using UnityEngine;

public class PageManager : MonoBehaviour
{
    [SerializeField] private Page _startPage;
    [SerializeField] private Page _loadingPage;
    [SerializeField] private WarningPage _warningPage;
    [SerializeField] private Page _historyPage;

    public Page StartPage { get { return _startPage; } }
    public static PageManager Instance { get; private set; }
    public Page CurrentPage { get; private set; }
    public Page HistoryPage => _historyPage;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        StartProjectObject.OnProjectStarted += OpenStartPage;

        if (StartProjectObject.ProjectStarted && StartPage != null)
        {
            OpenStartPage();
        }
    }

    private void OnDestroy()
    {
        StartProjectObject.OnProjectStarted -= OpenStartPage;
    }

    public void OpenStartPage()
    {
        OpenPage(StartPage);
    }

    public void OpenPage(Page page, int popUpLevel = 0)
    { 
        if (popUpLevel == 0)
            CurrentPage?.Close();

        page.Open(popUpLevel);
        CurrentPage = page;
    }

    public void ClosePage(Page page)
    {
        page.Close();
    }

    public void OpenPage<T>(Page page, T param, int popUpLevel = 0)
    {
        if (popUpLevel == 0)
            CurrentPage?.Close();

        page.Open(param, popUpLevel);
        CurrentPage = page;
    }

    public void OpenWarningPage(WarningData data, int popUpLevel = 1)
    {
        _warningPage.Open(data, popUpLevel);
    }

    public void CloseWarningPage()
    {
        _warningPage.Close();
    }

    public void DisplayLoadingPage(bool enable, int popUpLevel = 0)
    {
        if (enable)
        {
            OpenPage(_loadingPage, popUpLevel);
        }
        else
        {
            ClosePage(_loadingPage);
        }
    }
}
