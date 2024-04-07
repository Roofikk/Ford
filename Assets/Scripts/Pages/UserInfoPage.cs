using Ford.WebApi;
using Ford.WebApi.Data;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPage : Page
{
    [SerializeField] private Button _closeButton;

    [Space(10)]
    [SerializeField] private LoadFieldText _fullNameText;
    [SerializeField] private LoadFieldText _locationText;
    [SerializeField] private LoadFieldText _usernameText;
    [SerializeField] private LoadFieldText _phoneNumberText;
    [SerializeField] private LoadFieldText _countryText;
    [SerializeField] private LoadFieldText _birthDateText;

    private List<LoadFieldText> _loadFields = new();

    private void Start()
    {
        _closeButton.onClick.AddListener(() =>
        {
            PageManager.Instance.ClosePage(this);
        });
    }

    public override void Open<T>(T param, int popUpLevel = 0)
    {
        base.Open(param, popUpLevel);

        if (param is not UserIdentity userParam)
        {
            throw new System.Exception("Wrong param type. Expected UserPageParam");
        }

        if (userParam.UserId == null && string.IsNullOrEmpty(userParam.UserName))
        {
            throw new System.Exception("Cannot being all fields empty");
        }

        var client = new FordApiClient();
        using var tokenStorage = new TokenStorage();

        client.CheckTokenAsync(tokenStorage.GetAccessToken()).RunOnMainThread(result =>
        {
            using var tokenStorage = new TokenStorage();
            if (result.StatusCode != HttpStatusCode.OK)
            {
                client.RefreshTokenAndReply(tokenStorage.GetAccessToken(), client.GetUserInfoAsync, userParam).RunOnMainThread(result =>
                {
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        SetUserInfo(result.Content);
                    }
                    else
                    {
                        ToastMessage.Show("Не удалось получить информацию о пользователе\n" +
                            "Повторите попытку позднее");
                    }
                });
            }
            else
            {

                client.GetUserInfoAsync(tokenStorage.GetAccessToken(), userParam).RunOnMainThread(result =>
                {
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        SetUserInfo(result.Content);
                    }
                    else
                    {
                        ToastMessage.Show("Не удалось получить информацию о пользователе\n" +
                            "Повторите попытку позднее");
                    }
                });
            }
        });
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        if (_loadFields.Count == 0)
        {
            _loadFields.Add(_fullNameText);
            _loadFields.Add(_locationText);
            _loadFields.Add(_usernameText);
            _loadFields.Add(_phoneNumberText);
            _loadFields.Add(_countryText);
            _loadFields.Add(_birthDateText);
        }

        DisplayFields(false);
    }

    public override void Close()
    {
        base.Close();

        DisplayFields(false);
    }

    private void SetUserInfo(UserDto userData)
    {
        DisplayFields(true);

        _fullNameText.SetInfo($"{userData.FirstName} {userData.LastName}");

        if (string.IsNullOrEmpty(userData.Region) && string.IsNullOrEmpty(userData.City))
        {
            _locationText.SetInfo("Неизвестно");
        }
        else if (string.IsNullOrEmpty(userData.Region))
        {
            _locationText.SetInfo(userData.City);
        }   
        else if (string.IsNullOrEmpty(userData.City))
        {
            _locationText.SetInfo(userData.Region);
        }
        else
        {
            _locationText.SetInfo($"{userData.Region}, {userData.City}");
        }

        _usernameText.SetInfo(userData.UserName);

        if (!string.IsNullOrEmpty(userData.PhoneNumber))
        {
            _phoneNumberText.SetInfo(userData.PhoneNumber);
        }
        else
        {
            _phoneNumberText.SetInfo("Неизвестно");
        }

        if (!string.IsNullOrEmpty(userData.Country))
        {
            _countryText.SetInfo(userData.Country);
        }
        else
        {
            _countryText.SetInfo("Неизвестно");
        }

        if (userData.BirthDate != null)
        {
            _birthDateText.SetInfo(userData.BirthDate.Value.ToString("dd.MM.yyyy"));
        }
        else
        {
            _birthDateText.SetInfo("Неизвестно");
        }
    }

    private void DisplayFields(bool enable)
    {
        foreach (var field in _loadFields)
        {
            field.DisplayEffect(!enable);
        }
    }
}
