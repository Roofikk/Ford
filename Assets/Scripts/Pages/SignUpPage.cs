using Ford.WebApi;
using Ford.WebApi.Data;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPage : Page
{
    [SerializeField] private TMP_InputField _firstNameField;
    [SerializeField] private TMP_InputField _lastNameField;
    [SerializeField] private TMP_InputField _birthDateField;
    [SerializeField] private TMP_InputField _loginField;
    [SerializeField] private TMP_InputField _emailField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private TMP_InputField _confiemPasswordField;

    [Space(10)]
    [SerializeField] private TextMeshProUGUI _exceptionText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _signUpButton;

    [Space(10)]
    [SerializeField] private Page _backPage;

    private List<TMP_InputField> _fieldsList;
    private List<FieldMaskValidate> _validatorsList;

    private void Start()
    {
        _fieldsList = new(_firstNameField.transform.parent.parent.GetComponentsInChildren<TMP_InputField>());
        _validatorsList = new();

        foreach (var field in _fieldsList)
        {
            field.onSelect.AddListener((text) => { DisplayException(false); });

            if (field.TryGetComponent<FieldMaskValidate>(out var validator))
            {
                _validatorsList.Add(validator);
            }
        }

        _signUpButton.onClick.AddListener(SignUp);
        _closeButton.onClick.AddListener(() =>
        {
            PageManager.Instance.ClosePage(this);
            PageManager.Instance.OpenPage(_backPage, 2);
        });
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);
    }

    public override void Close()
    {
        base.Close();

        foreach (var f in _fieldsList)
        {
            f.text = "";
        }

        DisplayException(false);
    }

    private void DisplayException(bool enable, string message = "")
    {
        _exceptionText.gameObject.SetActive(enable);
        _exceptionText.text = message;
    }

    private void SignUp()
    {
        if (!CheckValidFields(out var message))
        {
            DisplayException(true, message);
            return;
        }

        PageManager.Instance.DisplayLoadingPage(true, 4);

        var user = new RegisterUserDto()
        {
            Login = _loginField.text,
            Email = _emailField.text,
            FirstName = _firstNameField.text,
            LastName = _lastNameField.text,
            BirthDate = _birthDateField.GetComponent<InputFieldDateValidator>().Date,
            Password = _passwordField.text,
        };

        FordApiClient client = new();
        client.SignUpAsync(user)
            .RunOnMainThread((result) =>
            {
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        Debug.Log("Account has been created successly");

                        ToastMessage.Show("Вы успешно зарегистрировались", transform.parent);
                        PageManager.Instance.ClosePage(this);
                        PageManager.Instance.OpenPage(_backPage, 2);
                        break;
                    case HttpStatusCode.BadRequest:
                        Debug.LogError($"Request with status {result.StatusCode}");

                        foreach (var er in result.Errors)
                        {
                            string message = $"Error: {er.Title} -- {er.Message}";
                            Debug.LogError(message);

                            if (er.Title == "DuplicateUserName")
                            {
                                DisplayException(true, "Пользователь с таким никнеймом уже существет");
                                continue;
                            }

                            if (er.Title == "DuplicateEmail")
                            {
                                DisplayException(true, "Пользователь с таким адресом почты уже существует");
                                continue;
                            }

                            DisplayException(true, message);
                        }
                        break;
                    default:
                        Debug.LogError($"Request with status {result.StatusCode}");

                        foreach (var er in result.Errors)
                        {
                            string message = $"Error: {er.Title} -- {er.Message}";
                            DisplayException(true, message);
                            Debug.LogError(message);
                        }

                        break;
                }

                PageManager.Instance.DisplayLoadingPage(false);
            });
    }

    private bool CheckValidFields(out string message)
    {
        message = "";

        foreach (var v in _validatorsList)
        {
            if (!v.ValidateInput())
            {
                message = v.ExceptionMessage;
                return false;
            }
        }
        
        if (_passwordField.text != _confiemPasswordField.text)
        {
            message = "Пароли не совпали";
            return false;
        }

        return true;
    }
}
