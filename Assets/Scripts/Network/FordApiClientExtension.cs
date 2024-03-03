using Ford.SaveSystem.Ver2;
using Ford.WebApi;
using Ford.WebApi.Data;
using System;
using System.Threading.Tasks;
using UnityEngine;

public static class FordApiClientExtension
{
    public static async Task<ResponseResult<T>> RefreshTokenAndReply<T>(this FordApiClient client,
        string token, Func<string, Task<ResponseResult<T>>> func)
        where T : class
    {
        var storage = new Storage();
        var refreshToken = storage.GetRefreshToken();

        TokenDto tokenDto = new()
        {
            Token = token,
            RefreshToken = refreshToken,
        };

        var result = await client.RefreshTokenAsync(tokenDto);

        if (result.Content == null)
        {
            return new ResponseResult<T>(null, result.StatusCode, result.Errors);
        }

        storage.SaveAccessToken(result.Content.Token);
        storage.SaveRefreshToken(result.Content.RefreshToken);

        Debug.Log("Token has been refreshed");

        var response = await func(result.Content.Token);
        return response;
    }

    public static async Task<ResponseResult<T>> RefreshTokenAndReply<T, T1>(this FordApiClient client, 
        string token, Func<string, T1, Task<ResponseResult<T>>> func, T1 param1) 
        where T : class
        where T1 : class
    {
        var storage = new Storage();
        var refreshToken = storage.GetRefreshToken();

        TokenDto tokenDto = new()
        {
            Token = token,
            RefreshToken = refreshToken,
        };

        var result = await client.RefreshTokenAsync(tokenDto);

        if (result.Content == null)
        {
            return new ResponseResult<T>(null, result.StatusCode, result.Errors);
        }

        storage.SaveAccessToken(result.Content.Token);
        storage.SaveRefreshToken(result.Content.RefreshToken);

        Debug.Log("Token has been refreshed");

        var response = await func(result.Content.Token, param1);
        return response;
    }
}
