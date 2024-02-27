using Ford.WebApi;
using Ford.WebApi.Data;
using System;
using UnityEngine;

public class TestFordApi : MonoBehaviour
{
    private void Start()
    {

        //SignUp();
    }

    private void SingIn()
    {
        Debug.Log("Логин");

        FordApiClient fordApi = new();
        fordApi.SignInAsync(new LoginRequestDto()
        {
            Login = "user",
            Password = "user3211"
        }).RunOnMainThread((result) =>
        {
            var content = result.Content;
            if (content != null)
            {
                Debug.Log(content.Token);
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
        FordApiClient fordApi = new();
        fordApi.SignUpAsync(new()
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
}
