using Ford.WebApi;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInPage : Page
{
    [SerializeField] private ToastMessage _toastPrefab;
    [SerializeField] private Page _backPage;
    [SerializeField] private Page _signUpPage;

    [Space(10)]
    [SerializeField] private TMP_InputField _loginField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private TextMeshProUGUI _exceptionText;

    [Space(10)]
    [SerializeField] private Button _signInButton;
    [SerializeField] private Button _signUpButton;
    [SerializeField] private Button _closeButton;

    private FieldMaskValidate _loginValidator;
    private FieldMaskValidate _passwordValidator;

    private void Start()
    {
        _loginField.onSelect.AddListener((str) => DisplayException(false));
        _passwordField.onSelect.AddListener((str) => DisplayException(false));

        _loginValidator = _loginField.GetComponent<FieldMaskValidate>();
        _passwordValidator = _passwordField.GetComponent<FieldMaskValidate>();

        _signInButton.onClick.AddListener(SignIn);
        _signUpButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenPage(_signUpPage, 2);
            PageManager.Instance.ClosePage(this);
        });
        _closeButton.onClick.AddListener(() => { PageManager.Instance.ClosePage(this); });
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);
    }

    public override void Close()
    {
        _loginField.text = "";
        _passwordField.text = "";
        _exceptionText.gameObject.SetActive(false);

        base.Close();
    }

    private void DisplayException(bool enable, string message = "")
    {
        _exceptionText.gameObject.SetActive(enable);

        if (!string.IsNullOrEmpty(message))
        {
            _exceptionText.text = message;
        }
    }

    private void SignIn()
    {
        if (!ValidateInputs(out string exception))
        {
            _exceptionText.gameObject.SetActive(true);
            _exceptionText.text = exception;
            return;
        }

        PageManager.Instance.DisplayLoadingPage(true, 5);
        FordApiClient client = new();
        client.SignInAsync(new()
        {
            UserName = _loginField.text,
            Password = _passwordField.text
        }).RunOnMainThread(result =>
        {
            var content = result.Content;

            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    Player.Authorize(content, () =>
                    {
                        PageManager.Instance.OpenPage(_backPage);
                        PageManager.Instance.ClosePage(this);

                        ToastMessage.Show($"Добро пожаловать, {Player.UserData.FirstName}");

                        PageManager.Instance.DisplayLoadingPage(false);
                    });

                    break;
                case HttpStatusCode.Unauthorized:
                    _exceptionText.gameObject.SetActive(true);
                    _exceptionText.text = "Логин или пароль неверны";

                    PageManager.Instance.DisplayLoadingPage(false);
                    break;
                default:
                    _exceptionText.gameObject.SetActive(true);
                    _exceptionText.text = "Произошла ошибка. Попробуйте позже";

                    PageManager.Instance.DisplayLoadingPage(false);
                    break;
            }
        });
    }

    private bool ValidateInputs(out string exception)
    {
        exception = string.Empty;
        if (_loginValidator.ValidateInput() && _passwordValidator.ValidateInput())
        {
            return true;
        }
        else
        {
            exception = _loginValidator.ExceptionMessage;
            return false;
        }
    }
}
