using Ford.SaveSystem.Ver2;
using Ford.WebApi;
using Ford.WebApi.Data;
using System;
using System.Threading.Tasks;
using UnityEngine;

public static class FordApiClientExtension
{
    public static async Task<ResponseResult> RefreshTokenAndReply(this FordApiClient client,
        string token, Func<string, Task<ResponseResult>> func)
    {
        using var tokenStorage = new TokenStorage();
        var refreshToken = tokenStorage.GetRefreshToken();

        TokenDto tokenDto = new()
        {
            Token = token,
            RefreshToken = refreshToken.ToString(),
        };

        var result = await client.RefreshTokenAsync(tokenDto);

        if (result.Content == null)
        {
            return new ResponseResult(result.StatusCode, result.Errors);
        }

        tokenStorage.SetNewAccessToken(result.Content.Token);
        tokenStorage.SetNewRefreshToken(result.Content.RefreshToken);

        Debug.Log("Token has been refreshed");

        var response = await func(result.Content.Token);
        return response;
    }

    public static async Task<ResponseResult<T>> RefreshTokenAndReply<T>(this FordApiClient client,
        string token, Func<string, Task<ResponseResult<T>>> func)
        where T : class
    {
        using var tokenStorage = new TokenStorage();
        var refreshToken = tokenStorage.GetRefreshToken();

        TokenDto tokenDto = new()
        {
            Token = token,
            RefreshToken = refreshToken.ToString(),
        };

        var result = await client.RefreshTokenAsync(tokenDto);

        if (result.Content == null)
        {
            return new ResponseResult<T>(null, result.StatusCode, result.Errors);
        }

        tokenStorage.SetNewAccessToken(result.Content.Token);
        tokenStorage.SetNewRefreshToken(result.Content.RefreshToken);

        Debug.Log("Token has been refreshed");

        var response = await func(result.Content.Token);
        return response;
    }

    public static async Task<ResponseResult> RefreshTokenAndReply<TParam>(this FordApiClient client,
        string token, Func<string, TParam, Task<ResponseResult>> func, TParam param1)
    {
        using var tokenStorage = new TokenStorage();
        var refreshToken = tokenStorage.GetRefreshToken();

        TokenDto tokenDto = new()
        {
            Token = token,
            RefreshToken = refreshToken.ToString(),
        };

        var result = await client.RefreshTokenAsync(tokenDto);

        if (result.Content == null)
        {
            return new ResponseResult(result.StatusCode, result.Errors);
        }

        tokenStorage.SetNewAccessToken(result.Content.Token);
        tokenStorage.SetNewRefreshToken(result.Content.RefreshToken);

        Debug.Log("Token has been refreshed");

        var response = await func(result.Content.Token, param1);
        return response;
    }

    public static async Task<ResponseResult<TResult>> RefreshTokenAndReply<TParam, TResult>(this FordApiClient client,
        string token, Func<string, TParam, Task<ResponseResult<TResult>>> func, TParam param1)
        where TResult : class
    {
        using var tokenStorage = new TokenStorage();
        var refreshToken = tokenStorage.GetRefreshToken();

        TokenDto tokenDto = new()
        {
            Token = token,
            RefreshToken = refreshToken.ToString(),
        };

        var result = await client.RefreshTokenAsync(tokenDto);

        if (result.Content == null)
        {
            return new ResponseResult<TResult>(null, result.StatusCode, result.Errors);
        }

        tokenStorage.SetNewAccessToken(result.Content.Token);
        tokenStorage.SetNewRefreshToken(result.Content.RefreshToken);

        Debug.Log("Token has been refreshed");

        var response = await func(result.Content.Token, param1);
        return response;
    }

    public static async Task<ResponseResult<TResult>> RefreshTokenAndReply<TParam1, TParam2, TResult>(this FordApiClient client,
        string token, Func<string, TParam1, TParam2, Task<ResponseResult<TResult>>> func, TParam1 param1, TParam2 param2)
        where TResult : class
    {
        using var tokenStorage = new TokenStorage();
        var refreshToken = tokenStorage.GetRefreshToken();

        TokenDto tokenDto = new()
        {
            Token = token,
            RefreshToken = refreshToken.ToString(),
        };

        var result = await client.RefreshTokenAsync(tokenDto);

        if (result.Content == null)
        {
            return new ResponseResult<TResult>(null, result.StatusCode, result.Errors);
        }

        tokenStorage.SetNewAccessToken(result.Content.Token);
        tokenStorage.SetNewRefreshToken(result.Content.RefreshToken);

        Debug.Log("Token has been refreshed");

        var response = await func(result.Content.Token, param1, param2);
        return response;
    }

    public static async Task<ResponseResult<TResult>> RefreshTokenAndReply<TParam1, TParam2, TParam3, TResult>(this FordApiClient client,
        string token, Func<string, TParam1, TParam2, TParam3, Task<ResponseResult<TResult>>> func, TParam1 param1, TParam2 param2, TParam3 param3)
        where TResult : class
    {
        using var tokenStorage = new TokenStorage();
        var refreshToken = tokenStorage.GetRefreshToken();

        TokenDto tokenDto = new()
        {
            Token = token,
            RefreshToken = refreshToken.ToString(),
        };

        var result = await client.RefreshTokenAsync(tokenDto);

        if (result.Content == null)
        {
            return new ResponseResult<TResult>(null, result.StatusCode, result.Errors);
        }

        tokenStorage.SetNewAccessToken(result.Content.Token);
        tokenStorage.SetNewRefreshToken(result.Content.RefreshToken);

        Debug.Log("Token has been refreshed");

        var response = await func(result.Content.Token, param1, param2, param3);
        return response;
    }

    public static async Task<ResponseResult<TResult>> RefreshTokenAndReply<TParam1, TParam2, TParam3, TParam4, TResult>(this FordApiClient client,
        string token, Func<string, TParam1, TParam2, TParam3, TParam4, Task<ResponseResult<TResult>>> func, 
        TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        where TResult : class
    {
        using var tokenStorage = new TokenStorage();
        var refreshToken = tokenStorage.GetRefreshToken();

        TokenDto tokenDto = new()
        {
            Token = token,
            RefreshToken = refreshToken.ToString(),
        };

        var result = await client.RefreshTokenAsync(tokenDto);

        if (result.Content == null)
        {
            return new ResponseResult<TResult>(null, result.StatusCode, result.Errors);
        }

        tokenStorage.SetNewAccessToken(result.Content.Token);
        tokenStorage.SetNewRefreshToken(result.Content.RefreshToken);

        Debug.Log("Token has been refreshed");

        var response = await func(result.Content.Token, param1, param2, param3, param4);
        return response;
    }
}
