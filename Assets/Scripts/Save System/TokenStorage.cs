using System.Security;
using UnityEngine;

public class TokenStorage
{
    private readonly string ACCESS_TOKEN = "ACCESS_TOKEN";
    private readonly string REFRESH_TOKEN = "REFRESH_TOKEN";
    private static SecureString _accessToken = new();
    private static SecureString _refreshToken = new();

    public SecureString GetAccessToken()
    {
        if (_accessToken.Length == 0)
        {
            string token = PlayerPrefs.GetString(ACCESS_TOKEN, "");

            foreach (var c in token)
            {
                _accessToken.AppendChar(c);
            }
        }

        return _accessToken;
    }

    public SecureString GetRefreshToken()
    {
        if (_refreshToken.Length == 0)
        {
            string refreshToken = PlayerPrefs.GetString(REFRESH_TOKEN, "");

            foreach (var c in refreshToken)
            {
                _refreshToken.AppendChar(c);
            }
        }

        return _refreshToken;
    }

    public SecureString SetNewAccessToken(string accessToken)
    {
        PlayerPrefs.SetString(ACCESS_TOKEN, accessToken);
        _accessToken = new();

        foreach (var c in accessToken)
        {
            _accessToken.AppendChar(c);
        }

        return _accessToken;
    }

    public SecureString SetNewRefreshToken(string refreshToken)
    {
        PlayerPrefs.SetString(REFRESH_TOKEN, refreshToken);
        _refreshToken = new();

        foreach (var c in refreshToken)
        {
            _refreshToken.AppendChar(c);
        }

        return _refreshToken;
    }
}
