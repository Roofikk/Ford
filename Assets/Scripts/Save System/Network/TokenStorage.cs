using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security;
using UnityEngine;
using System.IO;
using System.Text;

namespace Ford.WebApi
{
    public class TokenStorage : IDisposable
    {
        private string ACCESS_TOKEN { get; } = "ACCESS_TOKEN";
        private string REFRESH_TOKEN { get; } = "REFRESH_TOKEN";
        private string ENCRYPTED_KEY { get; } = "ENCRYPTED_KEY";
        private static SecureString _accessToken = new();
        private static SecureString _refreshToken = new();

        public TokenStorage()
        {
            if (PlayerPrefs.HasKey(ACCESS_TOKEN))
            {
                var encryptedToken = PlayerPrefs.GetString(ACCESS_TOKEN, "");

                if (!string.IsNullOrEmpty(encryptedToken))
                {
                    string token = DecryptToken(Convert.FromBase64String(encryptedToken));
                    _accessToken = StringToSecureString(token);
                }
            }

            if (PlayerPrefs.HasKey(REFRESH_TOKEN))
            {
                string encryptedRefreshToken = PlayerPrefs.GetString(REFRESH_TOKEN, "");

                if (!string.IsNullOrEmpty(encryptedRefreshToken))
                {
                    string refreshToken = DecryptToken(Convert.FromBase64String(encryptedRefreshToken));
                    _refreshToken = StringToSecureString(refreshToken);
                }
            }
        }

        public string GetAccessToken()
        {
            return SecureStringToString(_accessToken);
        }

        public string GetRefreshToken()
        {
            return SecureStringToString(_refreshToken);
        }

        public void SetNewAccessToken(string accessToken)
        {
            var encryptedToken = EncryptToken(accessToken);
            PlayerPrefs.SetString(ACCESS_TOKEN, Convert.ToBase64String(encryptedToken));
            _accessToken = StringToSecureString(accessToken);
        }

        public void SetNewRefreshToken(string refreshToken)
        {
            var encryptedRefreshToken = EncryptToken(refreshToken);
            PlayerPrefs.SetString(REFRESH_TOKEN, Convert.ToBase64String(encryptedRefreshToken));
            _refreshToken = StringToSecureString(refreshToken);
        }

        private string SecureStringToString(SecureString ss)
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(ss);
            return Marshal.PtrToStringBSTR(bstr);
        }

        private SecureString StringToSecureString(string s)
        {
            SecureString ss = new();

            foreach (var c in s)
            {
                ss.AppendChar(c);
            }

            return ss;
        }

        private void SetSecureKey(byte[] bytes)
        {
            string key = Convert.ToBase64String(bytes);
            PlayerPrefs.SetString(ENCRYPTED_KEY, key);
        }

        private byte[] GetSecureKey()
        {
            string key = PlayerPrefs.GetString(ENCRYPTED_KEY, "");

            if (key.Length == 0)
            {
                var bytes = GenerateSecureKey();
                SetSecureKey(bytes);
                return bytes;
            }

            return Convert.FromBase64String(key);
        }

        private byte[] GenerateSecureKey()
        {
            using Aes aes = Aes.Create();
            aes.GenerateKey();
            return aes.Key;
        }

        private byte[] EncryptToken(string token)
        {
            using Aes aes = Aes.Create();
            aes.Key = GetSecureKey();
            aes.IV = new byte[16];

            using MemoryStream ms = new();
            using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
                cs.Write(tokenBytes, 0, tokenBytes.Length);
            }

            return ms.ToArray();
        }

        private string DecryptToken(byte[] encryptedToken)
        {
            using Aes aes = Aes.Create();
            aes.Key = GetSecureKey();
            aes.IV = new byte[16];

            // Расшифровка токена
            using MemoryStream ms = new(encryptedToken);
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(cs);

            return reader.ReadToEnd();
        }

        public void Dispose()
        {
            _accessToken.Dispose();
            _refreshToken.Dispose();
        }
    }
}
