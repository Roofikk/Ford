using Ford.SaveSystem.Ver2;
using Ford.WebApi;
using Ford.WebApi.Data;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditUserInfoPage : Page
{
    [SerializeField] private Page _backPage;

    [Space(10)]
    [SerializeField] private TMP_InputField _firstNameInput;
    [SerializeField] private TMP_InputField _lastNameInput;
    [SerializeField] private TMP_InputField _phoneNumberInput;
    [SerializeField] private TMP_InputField _birthDateInput;
    [SerializeField] private TMP_InputField _cityInput;
    [SerializeField] private TMP_InputField _regionInput;
    [SerializeField] private TMP_InputField _countryInput;

    [Space(10)]
    [SerializeField] private TextMeshProUGUI _exceptionText;

    [Space(10)]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _applyButton;

    private List<TMP_InputField> _inputsList;
    private List<FieldMaskValidate> _validatorsList;

    private void Start()
    {
        _inputsList = new(transform.GetComponentsInChildren<TMP_InputField>());
        _validatorsList = new();

        foreach (var input in _inputsList)
        {
            if (input.TryGetComponent<FieldMaskValidate>(out var validator))
            {
                _validatorsList.Add(validator);
            }
        }

        _closeButton.onClick.AddListener(() =>
        {
            PageManager.Instance.ClosePage(this);
            PageManager.Instance.OpenPage(_backPage, 2);
        });

        _applyButton.onClick.AddListener(() =>
        {
            Apply();
        });
    }

    private void OnDestroy()
    {
        _applyButton.onClick.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        var data = Player.UserData;

        _firstNameInput.text = data.FirstName;
        _lastNameInput.text = data.LastName;
        _phoneNumberInput.text = data.PhoneNumber;
        _birthDateInput.text = data.BirthDate?.ToString("dd.MM.yyyy");
        _cityInput.text = data.City;
        _regionInput.text = data.Region;
        _countryInput.text = data.Country;
    }

    public override void Close()
    {
        base.Close();
    }

    private void DisplayException(bool enable, string exception = "")
    {
        _exceptionText.gameObject.SetActive(enable);
        _exceptionText.text = exception;
    }

    private void Apply()
    {
        if (!ValidInputs())
        {
            return;
        }

        FordApiClient client = new();
        Storage storage = new();
        var accessToken = storage.GetAccessToken();

        UpdatingAccountDto data = new()
        {
            FirstName = _firstNameInput.text,
            LastName = _lastNameInput.text,
            PhoneNumber = _phoneNumberInput.text,
            BirthDate = _birthDateInput.GetComponent<InputFieldDateValidator>().Date,
            City = _cityInput.text,
            Region = _regionInput.text,
            Country = _countryInput.text
        };
        PageManager.Instance.DisplayLoadingPage(true, 6);

        client.UpdateUserInfoAsync(accessToken, data).RunOnMainThread(result =>
        {
            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    Player.UpdateUserInfo(result.Content);

                    ToastMessage.Show("Данные изменены", transform.parent);
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
                    client.RefreshTokenAndReply(accessToken, client.UpdateUserInfoAsync, data).RunOnMainThread(result =>
                    {
                        switch (result.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                Player.UpdateUserInfo(result.Content);

                                ToastMessage.Show("Данные изменены", transform.parent);
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

    private bool ValidInputs()
    {
        foreach (var input in _validatorsList)
        {
            if (!input.ValidateInput())
            {
                return false;
            }
        }

        return true;
    }
}
