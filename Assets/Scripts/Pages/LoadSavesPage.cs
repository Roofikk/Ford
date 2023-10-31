using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSavesPage : Page
{
    [SerializeField] private HorseInfoPanel _horseInfoPanel;
    [SerializeField] private SavesPanel _savesPanel;
    [SerializeField] private Button _loadButton;
    
    [Space(10)]
    [SerializeField] private LoadScenePage _loadScenePage;

    private HorseData _horseData;
    public HorseInfoPanel HorseInfoPanel { get { return _horseInfoPanel; } }

    private void Start()
    {
        _loadButton.onClick.AddListener(Load);
    }

    public void Open(HorseData horseData)
    {
        _horseData = horseData;

        if (!IsOpen)
            Open();

        _horseInfoPanel.FillData(_horseData);

        Storage storage = new(GameManager.Instance.Settings.PathSave);
        var saves = storage.GetHorseSaves(_horseData);

        _savesPanel.FillSaves(saves.ToArray());
    }

    public override void Close()
    {
        base.Close();
    }

    private void Load()
    {
        SceneParameters.AddParam(_horseData);
        SceneParameters.AddParam(_savesPanel.SelectedHorseSave);

        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);
        _loadScenePage.Open(loadingOperation);
    }
}
