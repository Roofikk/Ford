using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountInfoPage : Page
{
    [SerializeField] private Page _editUserInfoPage;
    [SerializeField] private Page _signInPage;
    [SerializeField] private Page _changePasswordPage;

    [Space(10)]
    [SerializeField] private TextMeshProUGUI _fullNameText;
    [SerializeField] private TextMeshProUGUI _regionAndCityText;
    [SerializeField] private TextMeshProUGUI _roleText;
    [SerializeField] private TextMeshProUGUI _loginText;
    [SerializeField] private TextMeshProUGUI _emailText;
    [SerializeField] private TextMeshProUGUI _phoneText;
    [SerializeField] private TextMeshProUGUI _countryText;
    [SerializeField] private TextMeshProUGUI _birthDateText;
    [SerializeField] private TextMeshProUGUI _lastUpdateText;
    [SerializeField] private TextMeshProUGUI _signUpDateText;

    [Space(10)]
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _changePasswordButton;
    [SerializeField] private Button _logoutButton;
    [SerializeField] private Button _closeButton;

    public void Start()
    {
        _logoutButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenWarningPage(new("Выйти из учетной записи?", 
                "Вы уверены, что хотите выйти из своей учетной записи?\nВсе сохранения будут утеряны до повторного входа.", Logout),4);
        });

        _editButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenPage(_editUserInfoPage, 2);
        });

        _closeButton.onClick.AddListener(() =>
        {
            PageManager.Instance.ClosePage(this);
            PageManager.Instance.OpenPage(PageManager.Instance.StartPage);
        });

        _changePasswordButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenPage(_changePasswordPage, 4);
        });
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        var data = Player.UserData;

        _fullNameText.text = $"{data.FirstName} {data.LastName}";

        if (string.IsNullOrEmpty(data.Region) && string.IsNullOrEmpty(data.City))
        {
            _regionAndCityText.text = "Неизвестно";
        }
        else
        {
            _regionAndCityText.text = $"{data.Region}, {data.City}";
        }

        _loginText.text = data.UserName;
        _emailText.text = data.Email;
        _phoneText.text = data.PhoneNumber;
        _countryText.text = data.Country;

        if (data.BirthDate.HasValue)
        {
            _birthDateText.text = data.BirthDate.Value.ToString("dd.MM.yyyy");
        }
        else
        {
            _birthDateText.text = "Не задано";
        }

        _lastUpdateText.text = data.LastUpdatedDate.ToString("dd.MM.yyyy");
        _signUpDateText.text = data.CreationDate.ToString("dd.MM.yyyy");
    }

    private void Logout()
    {
        Player.Logout();

        PageManager.Instance.ClosePage(this);
        PageManager.Instance.OpenPage(_signInPage, 2);
    }

    public override void Close()
    {
        base.Close();
    }
}
