using UnityEngine;
using UnityEngine.UI;

public class ButtonBar : MonoBehaviour
{
    [SerializeField] private Button _exitProject;
    [SerializeField] private Button _settingPageOpen;
    [SerializeField] private Button _savePageOpen;

    [Space(10f)]
    [Header("Pages")]
    [SerializeField] private WarningPage _warningExitPage;
    [SerializeField] private SettingsPage _settingsPage;
    [SerializeField] private SavePanel _savePage;

    private void Start()
    {
        _exitProject.onClick.AddListener(OnExitProjectClicked);
        _settingPageOpen.onClick.AddListener(OnSettingPageOpenClicked);
        _savePageOpen.onClick.AddListener(OnSaveButtonClicked);
    }

    private void OnDestroy()
    {
        _exitProject.onClick.RemoveAllListeners();
        _settingPageOpen.onClick.RemoveAllListeners();
        _savePageOpen.onClick.RemoveAllListeners();
    }

    private void OnExitProjectClicked()
    {
        var warningData = new WarningData(
            "Внимание",
            "Уверены, что хотите завершить проект?\r\nНесохраненные данные будут утеряны. Убедитесь, что вы сохранились",
            GameManager.Instance.ExitProject,
            null,
            null
        );

        PageManager.Instance.OpenPage(_warningExitPage, warningData, 1);
    }

    private void OnSettingPageOpenClicked()
    {
        PageManager.Instance.OpenPage(_settingsPage, 1);
    }

    private void OnSaveButtonClicked()
    {
        PageManager.Instance.OpenPage(_savePage, 1);
    }
}
