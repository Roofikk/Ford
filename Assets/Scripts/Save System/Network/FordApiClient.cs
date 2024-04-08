using Ford.SaveSystem.Data;
using Ford.SaveSystem.Data.Dtos;
using Ford.SaveSystem.Ver2;
using Ford.WebApi.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ford.WebApi
{
    public class FordApiClient
    {
        private static Uri _hostDomain;

        private string UsersPath { get; } = "api/users";

        private string IdentityPath { get; } = "api/identity";
        private string SignUpPath => $"{IdentityPath}/sign-up";
        private string SignInPath => $"{IdentityPath}/sign-in";
        private string RefreshTokenPath => $"{IdentityPath}/refresh-token";
        private string AccountPath => $"{IdentityPath}/account";
        private string PasswordChangePath => $"{AccountPath}/password";

        private string HorsesPath { get; } = "api/horses";
        private string UpdateHorseOwnersPath { get; } = "api/horseOwners";
        private string ChangeOwnerRolePath => $"{UpdateHorseOwnersPath}/change-owner-role";

        private string SavesPath { get; } = "api/saves";

        public static void SetHost(string host)
        {
            _hostDomain = new Uri(host);
        }

        #region Users
        /// <summary>
        /// Find user by user name<br/>
        /// GET request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<ResponseResult<UserDto>> FindUserAsync(string accessToken, string login)
        {
            Uri uri = new(_hostDomain, $"{UsersPath}/search?userName={login}");
            var result = await GetRequest<UserDto>(uri, accessToken);
            return result;
        }

        /// <summary>
        /// Get information about user<br/>
        /// GET request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseResult<UserDto>> GetUserInfoAsync(string accessToken, long userId)
        {
            Uri uri = new(_hostDomain, $"{UsersPath}?userId={userId}");
            var result = await GetRequest<UserDto>(uri, accessToken);
            return result;
        }

        public async Task<ResponseResult<UserDto>> GetUserInfoAsync(string accessToken, UserIdentity userData)
        {
            if (userData.UserId == null && string.IsNullOrEmpty(userData.UserName))
            {
                return new ResponseResult<UserDto>(null, HttpStatusCode.BadRequest);
            }

            if (userData.UserId != null)
            {
                return await GetUserInfoAsync(accessToken, userData.UserId.Value);
            }
            else
            {
                return await FindUserAsync(accessToken, userData.UserName);
            }
        }
        #endregion

        #region Account
        /// <summary>
        /// Sign up user<br/>
        /// POST request<br/>
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        public async Task<ResponseResult<AccountDto>> SignUpAsync(RegisterUserDto registerUser)
        {
            Uri uri = new(_hostDomain, SignUpPath);
            var result = await PostRequest<AccountDto>(uri, registerUser);
            return result;
        }

        /// <summary>
        /// Get access token<br/>
        /// POST request<br/>
        /// </summary>
        /// <param name="loginRequestData"></param>
        /// <returns></returns>
        public async Task<ResponseResult<TokenDto>> SignInAsync(LoginRequestDto loginRequestData)
        {
            Uri uri = new(_hostDomain, SignInPath);
            var result = await PostRequest<TokenDto>(uri, loginRequestData);
            return result;
        }

        public async Task<ResponseResult> CheckTokenAsync(string accessToken)
        {
            Uri uri = new(_hostDomain, $"{IdentityPath}/check-token");
            var result = await GetRequest(uri, accessToken);
            return result;
        }

        /// <summary>
        /// Refresh token<br/>
        /// POST request
        /// </summary>
        /// <param name="requestToken"></param>
        /// <returns></returns>
        public async Task<ResponseResult<TokenDto>> RefreshTokenAsync(TokenDto requestToken)
        {
            Uri uri = new(_hostDomain, RefreshTokenPath);
            var result = await PostRequest<TokenDto>(uri, requestToken);
            return result;
        }

        /// <summary>
        /// Get logining user info<br/>
        /// GET request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<ResponseResult<AccountDto>> GetAccountInfoAsync(string accessToken)
        {
            Uri uri = new(_hostDomain, AccountPath);
            var account = await GetRequest<AccountDto>(uri, accessToken);
            return account;
        }

        /// <summary>
        /// Update user info<br/>
        /// POST request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="requestAccount"></param>
        /// <returns></returns>
        public async Task<ResponseResult<AccountDto>> UpdateUserInfoAsync(string accessToken, UpdatingAccountDto requestAccount)
        {
            Uri uri = new(_hostDomain, AccountPath);
            var result = await PostRequest<AccountDto>(uri, requestAccount, accessToken);
            return result;
        }

        /// <summary>
        /// Change password<br/>
        /// POST request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="passwordRequest"></param>
        /// <returns></returns>
        public async Task<ResponseResult<TokenDto>> ChangePasswordAsync(string accessToken, UpdatingPasswordDto passwordRequest)
        {
            Uri uri = new(_hostDomain, PasswordChangePath);
            var result = await PostRequest<TokenDto>(uri, passwordRequest, accessToken);
            return result;
        }
        #endregion

        #region Horse
        /// <summary>
        /// Get horse<br/>
        /// GET request
        /// </summary>
        /// <param name="accessToken">Access token</param>
        /// <param name="horseId">Horse id</param>
        /// <returns>Horse object</returns>
        public async Task<ResponseResult<HorseBase>> GetHorseAsync(string accessToken, long horseId)
        {
            Uri uri = new(_hostDomain, $"{HorsesPath}?horseId={horseId}");
            var horse = await GetRequest<HorseBase>(uri, accessToken);
            return horse;
        }

        /// <summary>
        /// Get horses<br/>
        /// GET request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<ResponseResult<ICollection<HorseBase>>> GetHorsesAsync(string accessToken, int below, int amount,
            string orderByDate = "desc", string orderByName = "false")
        {
            Uri uri = new(_hostDomain, $"{HorsesPath}?below={below}&amount={amount}&orderByDate={orderByDate}&orderByName={orderByName}");
            var result = await GetRequest<ICollection<HorseBase>>(uri, accessToken);
            return new ResponseResult<ICollection<HorseBase>>(result.Content, result.StatusCode, result.Errors);
        }

        /// <summary>
        /// Create horse<br/>
        /// POST request
        /// </summary>
        /// <param name="accessToken">Access token</param>
        /// <param name="horse">Horse object for creation</param>
        /// <returns></returns>
        public async Task<ResponseResult<HorseBase>> CreateHorseAsync(string accessToken, CreationHorse horse)
        {
            Uri uri = new(_hostDomain, HorsesPath);
            var result = await PostRequest<HorseBase>(uri, horse, accessToken);
            return result;
        }

        /// <summary>
        /// Update horse object data<br/>
        /// PUT request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="horse"></param>
        /// <returns></returns>
        public async Task<ResponseResult<HorseBase>> UpdateHorseAsync(string accessToken, UpdatingHorse horse)
        {
            Uri uri = new(_hostDomain, HorsesPath);
            var result = await PutRequest<HorseBase>(uri, horse, accessToken);
            return result;
        }
        
        // не проверил
        /// <summary>
        /// Delete horse<br/>
        /// DELETE request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="horseId"></param>
        /// <returns></returns>
        public async Task<ResponseResult> DeleteHorseAsync(string accessToken, long horseId)
        {
            Uri uri = new(_hostDomain, $"{HorsesPath}?horseId={horseId}");
            var result = await DeleteRequest(uri, accessToken);
            return result;
        }

        /// <summary>
        /// Push history while user was offline<br/>
        /// POST request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        public async Task<ResponseResult> PushHistory(string accessToken, ICollection<StorageAction> history)
        {
            Uri uri = new(_hostDomain, HorsesPath + "/history");
            var result = await PostRequest(uri, history);
            return result;
        }
        #endregion

        #region HorseOwner
        /// <summary>
        /// Update horse owners<br/>
        /// POST request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="horseOwners"></param>
        /// <returns></returns>
        public async Task<ResponseResult<HorseBase>> UpdateHorseOwnersAsync(string accessToken, UpdatingHorseOwnersDto horseOwners)
        {
            Uri uri = new(_hostDomain, UpdateHorseOwnersPath);
            var horse = await PostRequest<HorseBase>(uri, horseOwners, accessToken);
            return horse;
        }

        /// <summary>
        /// Delete horse owner<br/>
        /// DELETE request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="horseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseResult> DeleteHorseOwnerAsync(string accessToken, long horseId, long userId)
        {
            Uri uri = new(_hostDomain, $"{HorsesPath}?userId={userId}&horseId={horseId}");
            var result = await DeleteRequest(uri, accessToken);
            return result;
        }

        /// <summary>
        /// Add owner to horse<br/>
        /// POST request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="horseOwner"></param>
        /// <returns></returns>
        public async Task<ResponseResult<HorseUserDto>> AddOwnerAsync(string accessToken, AddingHorseOwner horseOwner)
        {
            Uri uri = new(_hostDomain, UpdateHorseOwnersPath);
            var ownerDto = new AddingHorseOwnerDto(horseOwner);
            var result = await PostRequest<HorseUserDto>(uri, ownerDto, accessToken);
            return result;
        }

        /// <summary>
        /// Change owner role of horse<br/>
        /// POST request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="horseOwner"></param>
        /// <returns></returns>
        public async Task<ResponseResult<HorseUserDto>> ChangeOwnerRoleAsync(string accessToken, AddingHorseOwner horseOwner)
        {
            Uri uri = new(_hostDomain, ChangeOwnerRolePath);
            var ownerDto = new AddingHorseOwnerDto(horseOwner);
            var result = await PostRequest<HorseUserDto>(uri, ownerDto, accessToken);
            return result;
        }
        #endregion

        #region Saves
        public async Task<ResponseResult<ICollection<SaveInfo>>> GetSavesAsync(string accessToken, long horseId, int below = 0, int amount = 20)
        {
            Uri uri = new Uri(_hostDomain, $"{SavesPath}?horseId={horseId}&below={below}&amount={amount}");
            var result = await GetRequest<ICollection<SaveInfo>>(uri, accessToken);
            return result;
        }

        public async Task<ResponseResult<FullSaveInfo>> GetSaveAsync(string accessToken, long horseId, long saveId)
        {
            Uri uri = new Uri(_hostDomain, $"{SavesPath}?horseId={horseId}&saveId={saveId}");
            var result = await GetRequest<FullSaveInfo>(uri, accessToken);
            return result;
        }

        public async Task<ResponseResult<SaveInfo>> CreateSaveAsync(string accessToken, CreatingSaveDto fullSave)
        {
            Uri uri = new Uri(_hostDomain, $"{SavesPath}");
            var result = await PostRequest<SaveInfo>(uri, fullSave, accessToken);
            return result;
        }

        public async Task<ResponseResult<SaveInfo>> UpdateSaveInfoAsync(string accessToken, ModifiedSaveDto save)
        {
            Uri uri = new Uri(_hostDomain, SavesPath);
            var result = await PutRequest<SaveInfo>(uri, save, accessToken);
            return result;
        }

        public async Task<ResponseResult> DeleteSaveAsync(string accessToken, long saveId)
        {
            Uri uri = new Uri(_hostDomain, $"{SavesPath}&saveId={saveId}");
            var result = await DeleteRequest(uri, accessToken);
            return result;
        }
        #endregion

        private async Task<ResponseResult<T>> GetRequest<T>(Uri uri, string accessToken = "") where T : class
        {
            HttpClient client = new();
            HttpRequestMessage request = new(HttpMethod.Get, uri);

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            try
            {
                var response = await client.SendAsync(request);
                string responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<T>(responseText);
                    return new ResponseResult<T>(result);
                }

                return ReturnBadResponse<T>(responseText, response.StatusCode);
            }
            catch (Exception)
            {
                return new ResponseResult<T>(null, HttpStatusCode.InternalServerError, new List<ResponseError>()
                {
                    new()
                    {
                        Title = "ConnectionFailure",
                        Message = "Check your connection or repeat the request later"
                    }
                });
            }
        }

        private async Task<ResponseResult> GetRequest(Uri uri, string accessToken = "")
        {
            HttpClient client = new();
            HttpRequestMessage request = new(HttpMethod.Get, uri);

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            try
            {
                var response = await client.SendAsync(request);
                var result = new ResponseResult(response.StatusCode, new());

                foreach (var header in response.Headers)
                {
                    result.Errors.Add(new(header.Key, string.Join("\n------------------\n", header.Value)));
                }

                return result;
            }
            catch (Exception)
            {
                return new ResponseResult(HttpStatusCode.InternalServerError);
            }
        }

        private async Task<ResponseResult<T>> PostRequest<T>(Uri uri, object transferObject, string accessToken = "") where T : class
        {
            string json = JsonConvert.SerializeObject(transferObject, Formatting.Indented);
            var jsonSettings = new JsonSerializerSettings();
            StringContent content = new(json, Encoding.UTF8, "application/json");

            HttpClient client = new();

            HttpRequestMessage request = new(HttpMethod.Post, uri)
            {
                Content = content
            };

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            try
            {
                var response = await client.SendAsync(request);
                string responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var retrieveHorse = JsonConvert.DeserializeObject<T>(responseText);
                    return new ResponseResult<T>(retrieveHorse);
                }

                return ReturnBadResponse<T>(responseText, response.StatusCode);
            }
            catch (Exception)
            {
                return new ResponseResult<T>(null, HttpStatusCode.InternalServerError, new List<ResponseError>()
                {
                    new()
                    {
                        Title = "ConnectionFailure",
                        Message = "Check your connection or repeat the request later"
                    }
                });
            }
        }

        private async Task<ResponseResult> PostRequest(Uri uri, object transferObject, string accessToken = "")
        {
            string json = JsonConvert.SerializeObject(transferObject, Formatting.Indented);
            var jsonSettings = new JsonSerializerSettings();
            StringContent content = new(json, Encoding.UTF8, "application/json");

            HttpClient client = new();

            HttpRequestMessage request = new(HttpMethod.Post, uri)
            {
                Content = content
            };

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            try
            {
                var response = await client.SendAsync(request);
                string responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var retrieveHorse = JsonConvert.DeserializeObject(responseText);
                    return new ResponseResult();
                }

                return ReturnBadResponse(responseText, response.StatusCode);
            }
            catch (Exception)
            {
                return new ResponseResult(HttpStatusCode.InternalServerError, new List<ResponseError>()
                {
                    new()
                    {
                        Title = "ConnectionFailure",
                        Message = "Check your connection or repeat the request later"
                    }
                });
            }
        }

        private async Task<ResponseResult<T>> PutRequest<T>(Uri uri, object transferObject, string accessToken = "") where T : class
        {
            string json = JsonConvert.SerializeObject(transferObject);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpClient client = new();

            HttpRequestMessage request = new(HttpMethod.Put, uri)
            {
                Content = content
            };

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            try
            {
                var response = await client.SendAsync(request);
                string responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var retrieveHorse = JsonConvert.DeserializeObject<T>(responseText);
                    return new ResponseResult<T>(retrieveHorse);
                }

                return ReturnBadResponse<T>(responseText, response.StatusCode);
            }
            catch (Exception)
            {
                return new ResponseResult<T>(null, HttpStatusCode.InternalServerError, new List<ResponseError>()
                {
                    new()
                    {
                        Title = "ConnectionFailure",
                        Message = "Check your connection or repeat the request later"
                    }
                });
            }
        }

        private async Task<ResponseResult> DeleteRequest(Uri uri, string accessToken = "")
        {
            HttpClient client = new();
            HttpRequestMessage request = new(HttpMethod.Delete, uri);

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            try
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseResult(response.StatusCode);
                }

                string responseText = await response.Content.ReadAsStringAsync();
                return ReturnBadResponse(responseText, response.StatusCode);
            }
            catch (Exception)
            {
                return new ResponseResult(HttpStatusCode.InternalServerError, new List<ResponseError>()
                {
                    new()
                    {
                        Title = "ConnectionFailure",
                        Message = "Check your connection or repeat the request later"
                    }
                });
            }
        }

        private ResponseResult<T> ReturnBadResponse<T>(string responseText, HttpStatusCode code) where T : class
        {
            try
            {
                BadResponseDto badResponse = JsonConvert.DeserializeObject<BadResponseDto>(responseText);
                return new ResponseResult<T>(null, code, badResponse?.Errors);
            }
            catch (JsonSerializationException)
            {
                return new ResponseResult<T>(null, code, new List<ResponseError>()
                {
                    new()
                    {
                        Title = "JsonSerializer",
                        Message = $"Text could not be serialized\nText: {responseText}"
                    }
                });
            }
        }

        private ResponseResult ReturnBadResponse(string responseText, HttpStatusCode code)
        {
            var badResponse = JsonConvert.DeserializeObject<BadResponseDto>(responseText);
            if (badResponse != null)
            {
                return new ResponseResult(code, badResponse?.Errors);
            }

            return new ResponseResult(code, new List<ResponseError>()
            {
                new()
                {
                    Title = "Unknown error",
                    Message = "Unknown error"
                }
            });
        }
    }
}
