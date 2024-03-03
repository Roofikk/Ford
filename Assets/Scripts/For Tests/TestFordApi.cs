using Ford.WebApi;
using Ford.WebApi.Data;
using System;
using UnityEngine;

public class TestFordApi : MonoBehaviour
{
    [SerializeField] private string _accessToken = "";
    private FordApiClient _client;

    private void Start()
    {

    }

    private void SignIn(Action callback)
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
                callback?.Invoke();
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

    private void UpdateAccout()
    {
        FordApiClient client = new();
        client.UpdateUserInfoAsync(_accessToken, new()
        {
            FirstName = "Check",
            LastName = "Update",
            BirthDate = new DateTime(1999, 8, 6),
            Country = "Russia",
            Region = "Karelia",
            City = "Petrozavodsk"
        }).RunOnMainThread((result) =>
        {
            var content = result.Content;
            if (content != null)
            {
                Debug.Log("User has been updated");
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

    private void ChangePassword()
    {
        _client.ChangePasswordAsync(_accessToken, new UpdatingPasswordDto()
        {
            CurrentPassword = "user321",
            NewPassword = "user321"
        }).RunOnMainThread((result) =>
        {
            var content = result.Content;
            if (content != null)
            {
                Debug.Log("Password has been changed");
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

    private void GetHorse(long horseId)
    {
        _client.GetHorseAsync(_accessToken, horseId)
            .RunOnMainThread((result) =>
            {
                var horse = result.Content;
                if (horse != null)
                {
                    Debug.Log($"Horse info: {horse.Name}, {horse.CreationDate:dd.MM.yyyy}");
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

    private void GetHorses()
    {
        _client.GetHorsesAsync(_accessToken)
            .RunOnMainThread((result =>
            {
                var horses = result.Content;
                if (horses != null)
                {
                    foreach (var horse in horses)
                    {
                        Debug.Log($"Horse info: {horse.HorseId}, {horse.Name}");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.LogError($"Title: {error.Title}. Message: {error.Message}");
                    }
                }
            }));
    }

    private void CreateHorse()
    {
        _client.CreateHorseAsync(_accessToken, new()
        {
            Name = "Ford from App",
            Description = "Hello. I'm come from app",
            BirthDate = new DateTime(2024, 1, 1),
            Sex = "Male",
            City = "Ptz",
            Region = "Karelia",
            Country = "Russia",
            HorseOwners = null
        }).RunOnMainThread((result) =>
        {
            var horse = result.Content;
            if (horse != null)
            {
                Debug.Log($"Horse created: {horse.HorseId}, {horse.Name}");
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

    private void UpdateHorse()
    {
        _client.UpdateHorseAsync(_accessToken, new()
        {
            HorseId = 1,
            Name = "Rename from App",
            Description = "Editing from application",
            BirthDate = new DateTime(2024, 1, 1),
            Sex = "Female",
            City = "Knd",
            Region = "Murmansk",
            Country = "Russia"
        }).RunOnMainThread((result) =>
        {
            var horse = result.Content;
            if (horse != null)
            {
                Debug.Log($"Horse has been update: {horse.HorseId}, {horse.Name}");
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
