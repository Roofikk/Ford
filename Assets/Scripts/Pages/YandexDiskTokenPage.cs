using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YandexDiskSDK;

public class YandexDiskTokenPage : Page
{
    [SerializeField] private YandexDiskClient _diskClient;

    [SerializeField] private TMP_InputField _tokenInput;

    [Space(10f)]
    [Header("Buttons")]
    [SerializeField] private Button _getInfoDiksButton;
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _cancelButton;

    private void Start()
    {
        _applyButton.onClick.AddListener(OnApplyClicked);
        _cancelButton.onClick.AddListener(OnCancelClicked);
        _getInfoDiksButton.onClick.AddListener(OnClickedGetInfoDisk);
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        if (_diskClient.IsAuthorized)
        {
            _getInfoDiksButton.gameObject.SetActive(true);
            _tokenInput.text = "Вы успешно авторизованы в Яндекс.Диске";
            _tokenInput.readOnly = true;
            _applyButton.gameObject.SetActive(false);
        }
        else
        {
            _getInfoDiksButton.gameObject.SetActive(false);
            _tokenInput.text = "";
            _tokenInput.readOnly = false;
            _diskClient.OpenAuthorizationPage();
            _applyButton.gameObject.SetActive(true);
        }
    }

    public override void Close()
    {
        base.Close();

        _tokenInput.text = "";
    }

    private async void OnApplyClicked()
    {
        PersonData person = await _diskClient.Authorize(_tokenInput.text);
        
        if (person != null)
        {
            Debug.Log("Token successful");
            ToastMessage.Show("Авторизация успешно завершена", transform.parent);
            YandexDiskToken yandexDiskToken = new();
            yandexDiskToken.SaveToken(_tokenInput.text);
            PageManager.Instance.ClosePage(this);
        }
        else
        {
            Debug.Log("Token failed");
            ToastMessage.Show("Не удалось авторизоваться. Проверьте ключ", transform.parent);
        }
    }

    private void OnCancelClicked()
    {
        PageManager.Instance.ClosePage(this);
    }

    private async void OnClickedGetInfoDisk()
    {
        DiskInfo diskInfo = await _diskClient.GetDiskInfo();

        if (diskInfo != null)
        {
            Debug.Log("Информация о диске получена");
        }
    }
}
