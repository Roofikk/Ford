using Ford.SaveSystem.Ver2;
using Ford.WebApi;
using Ford.WebApi.Data;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangePasswordPage : Page
{
    [SerializeField] private Page _backPage;

    [Space(10)]
    [SerializeField] private TMP_InputField _currentPasswordInput;
    [SerializeField] private TMP_InputField _newPasswordField;
    [SerializeField] private TMP_InputField _repeatNewPasswordField;
    [SerializeField] private TextMeshProUGUI _exceptionText;

    [Space(10)]
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _applyButton;

    private List<TMP_InputField> _inputs;
    private List<FieldMaskValidate> _validates;

    private void Start()
    {
        _inputs = new(transform.GetComponentsInChildren<TMP_InputField>());
        _validates = new(transform.GetComponentsInChildren<FieldMaskValidate>());

        foreach (var field in _inputs)
        {
            field.onSelect.AddListener((str) => { DisplayException(false); });
        }

        _exitButton.onClick.AddListener(() =>
        {
            PageManager.Instance.ClosePage(this);
            PageManager.Instance.OpenPage(_backPage, 2);
        });

        _applyButton.onClick.AddListener(ApplyChanges);
    }

    private void OnDestroy()
    {
        _applyButton.onClick.RemoveAllListeners();
        _exitButton.onClick.RemoveAllListeners();

        foreach (var field in _inputs)
        {
            field.onSelect.RemoveAllListeners();
        }
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);
    }

    public override void Close()
    {
        base.Close();

        foreach (var input in _inputs)
        {
            input.text = string.Empty;
        }

        foreach (var input in _validates)
        {
            input.DisplayException(false);
        }
    }

    private void ApplyChanges()
    {
        if (!Validate())
        {
            return;
        }

        FordApiClient client = new();
        Storage storage = new();
        var accessToken = storage.GetAccessToken();

        var data = new UpdatingPasswordDto()
        {
            CurrentPassword = _currentPasswordInput.text,
            NewPassword = _newPasswordField.text,
        };

        PageManager.Instance.DisplayLoadingPage(true, 6);

        client.ChangePasswordAsync(accessToken, data).RunOnMainThread((result) =>
        {
            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    storage.SaveAccessToken(result.Content.Token);
                    storage.SaveRefreshToken(result.Content.RefreshToken);

                    ToastMessage.Show("Пароль успешно изменен", transform.parent);
                    PageManager.Instance.DisplayLoadingPage(false);
                    PageManager.Instance.ClosePage(this);
                    PageManager.Instance.OpenPage(_backPage, 2);
                    break;
                case HttpStatusCode.BadRequest:
                    string message = "";
                    foreach (var e in result.Errors)
                    {
                        message += $"{e.Title} -- {e.Message}\n";
                    }
                    DisplayException(true, message);
                    PageManager.Instance.DisplayLoadingPage(false);
                    break;
                case HttpStatusCode.Unauthorized:
                    client.RefreshTokenAndReply(accessToken, client.ChangePasswordAsync, data).RunOnMainThread(result =>
                    {
                        switch (result.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                storage.SaveAccessToken(result.Content.Token);
                                storage.SaveRefreshToken(result.Content.RefreshToken);

                                ToastMessage.Show("Пароль успешно изменен", transform.parent);
                                PageManager.Instance.ClosePage(this);
                                PageManager.Instance.OpenPage(_backPage, 2);
                                break;
                            case HttpStatusCode.BadRequest:
                                string message = "";
                                foreach (var e in result.Errors)
                                {
                                    message += $"{e.Title} -- {e.Message}\n";
                                }
                                DisplayException(true, message);
                                break;
                            case HttpStatusCode.Unauthorized:
                                Player.Logout();
                                ToastMessage.Show("Вы не авторизованы. Войдите еще раз", transform.parent);
                                PageManager.Instance.ClosePage(this);
                                PageManager.Instance.OpenPage(PageManager.Instance.StartPage);
                                break;
                            default:
                                DisplayException(true, "Произошла внутренняя ошибка. Повторите попытку позднее");
                                break;
                        }

                        PageManager.Instance.DisplayLoadingPage(false);
                    });
                    break;
                default:
                    DisplayException(true, "Произошла внутренняя ошибка. Повторите попытку позднее");
                    PageManager.Instance.DisplayLoadingPage(false);
                    break;
            }
        });
    }

    private void DisplayException(bool enable, string message = "")
    {
        _exceptionText.gameObject.SetActive(enable);
        _exceptionText.text = message;
    }

    private bool Validate()
    {
        foreach (var input in _validates)
        {
            if (!input.ValidateInput())
            {
                return false;
            }
        }

        if (_newPasswordField.text != _repeatNewPasswordField.text)
        {
            DisplayException(true, "Пароли не совпадают");
            return false;
        }

        return true;
    }
}
