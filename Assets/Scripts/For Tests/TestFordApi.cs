using Ford.WebApi;
using Ford.WebApi.Data;
using System;
using UnityEngine;

public class TestFordApi : MonoBehaviour
{
    [SerializeField] private string _accessToken = "";

    private void Start()
    {
        SignIn();
    }

    private void SignIn()
    {
        Debug.Log("Логин");

        FordApiClient apiClient = new();
        apiClient.SignInAsync(new LoginRequestDto()
        {
            Login = "user",
            Password = "user321"
        }).RunOnMainThread((result) =>
        {
            var content = result.Content;
            if (content != null)
            {
                if (string.IsNullOrEmpty(_accessToken))
                {
                    _accessToken = content.Token;
                }

                GetUserInfo(_accessToken);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Debug.LogError($"Title: {error.Title}. Message: {error.Message}");
                }
            }
        });
    }

    private void SignUp()
    {
        FordApiClient apiClient = new();
        apiClient.SignUpAsync(new()
        {
            Login = "Reg_From_App",
            Password = "fromApp123",
            Email = "FromApp@mail.com",
            BirthDate = new DateTime(1999, 8, 6),
            FirstName = "Ford",
            LastName = "App"
        }).RunOnMainThread((result) =>
        {
            var content = result.Content;
            if (content != null)
            {
                Debug.Log("Саксес получается");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Debug.LogError($"Title: {error.Title}. Message: {error.Message}");
                }
            }
        });
    }

    private void GetUserInfo(string accessToken)
    {
        FordApiClient apiClient = new();
        apiClient.GetUserInfoAsync(accessToken)
            .RunOnMainThread((result) =>
            {
                var content = result.Content;
                if (content != null)
                {
                    Debug.Log("Получили инфу о пользователе");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.LogError($"Title: {error.Title}. Message: {error.Message}");
                    }
                }
            });
    }
}
