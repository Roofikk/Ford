using UnityEngine;

public class YandexDiskToken
{
    private string _token = string.Empty;
    public string Token
    {
        get
        {
            if (string.IsNullOrEmpty(_token))
            {
                LoadToken();
            }

            return _token;
        }
    }

    private void LoadToken()
    {
        _token = PlayerPrefs.GetString("token", string.Empty);
    }

    public void SaveToken(string token)
    {
        PlayerPrefs.SetString("token", token);
        _token = token;
    }
}