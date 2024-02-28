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
        private Uri _hostUri; 

        private readonly string _signUpUri = "api/identity/sign-up";
        private readonly string _signInUri = "api/identity/login";
        private readonly string _accountUri = "api/identity/account";
        private readonly string _passwordChangeUri = "api/identity/account/password";

        private readonly string _horsesUri = "api/horses";
        private readonly string _updateHorseOwnersUri = "api/horses/owners";
        private readonly string _addHorseOwnersUri = "api/horses/add-owner";
        private readonly string _changeOwnerRoleUri = "api/horses/change-owner-role";

        private readonly string _savesUri = "api/saves";

        public FordApiClient(string hostUrl = "https://localhost:5000")
        {
            _hostUri= new Uri(hostUrl);
        }

        #region Account
        /// <summary>
        /// Sign up user<br/>
        /// POST request<br/>
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        public async Task<ResponseResult<AccountDto>> SignUpAsync(RegisterUserDto registerUser)
        {
            string json = JsonConvert.SerializeObject(registerUser);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpClient client = new();
            var response = await client.PostAsync(new Uri(_hostUri, _signUpUri), content);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var account = JsonConvert.DeserializeObject<AccountDto>(responseText);
                return new ResponseResult<AccountDto>(account);
            }

            return ReturnBadResponse<AccountDto>(responseText, response.StatusCode);
        }

        /// <summary>
        /// Get access token<br/>
        /// POST request<br/>
        /// </summary>
        /// <param name="loginRequestData"></param>
        /// <returns></returns>
        public async Task<ResponseResult<LoginResponseDto>> SignInAsync(LoginRequestDto loginRequestData)
        {
            string json = JsonConvert.SerializeObject(loginRequestData);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpClient client = new();
            var response = await client.PostAsync(new Uri(_hostUri, _signInUri), content);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var token = JsonConvert.DeserializeObject<LoginResponseDto>(responseText);
                return new ResponseResult<LoginResponseDto>(token);
            }

            return ReturnBadResponse<LoginResponseDto>(responseText, response.StatusCode);
        }

        /// <summary>
        /// Get logining user info<br/>
        /// GET request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<ResponseResult<AccountDto>> GetUserInfoAsync(string accessToken)
        {
            HttpClient client = new();
            Uri uri = new(_hostUri, _accountUri);
            HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var account = JsonConvert.DeserializeObject<AccountDto>(responseText);
                return new ResponseResult<AccountDto>(account);
            }

            return ReturnBadResponse<AccountDto>(responseText, response.StatusCode);
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
            string json = JsonConvert.SerializeObject(requestAccount);

            Uri uri = new(_hostUri, _accountUri);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpClient client = new();

            HttpRequestMessage request = new(HttpMethod.Post, uri)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var newAccount = JsonConvert.DeserializeObject<AccountDto>(responseText);
                return new ResponseResult<AccountDto>(newAccount);
            }

            return ReturnBadResponse<AccountDto>(responseText, response.StatusCode);
        }

        /// <summary>
        /// Change password<br/>
        /// POST request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="passwordRequest"></param>
        /// <returns></returns>
        public async Task<ResponseResult<LoginResponseDto>> ChangePasswordAsync(string accessToken, UpdatingPasswordDto passwordRequest)
        {
            string json = JsonConvert.SerializeObject(passwordRequest);

            Uri uri = new(_hostUri, _passwordChangeUri);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpClient client = new();

            HttpRequestMessage request = new(HttpMethod.Post, uri)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var token = JsonConvert.DeserializeObject<LoginResponseDto>(responseText);
                return new ResponseResult<LoginResponseDto>(token);
            }

            return ReturnBadResponse<LoginResponseDto>(responseText, response.StatusCode);
        }
        #endregion

        #region Horse
        // не проверено
        /// <summary>
        /// Get horse<br/>
        /// GET request
        /// </summary>
        /// <param name="accessToken">Access token</param>
        /// <param name="horseId">Horse id</param>
        /// <returns>Horse object</returns>
        public async Task<ResponseResult<HorseRetrieveDto>> GetHorseAsync(string accessToken, long horseId)
        {
            HttpClient client = new();
            Uri uri = new(_hostUri, $"{_horsesUri}?horseId={horseId}");
            HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var horse = JsonConvert.DeserializeObject<HorseRetrieveDto>(responseText);
                return new ResponseResult<HorseRetrieveDto>(horse);
            }

            return ReturnBadResponse<HorseRetrieveDto>(responseText, response.StatusCode);
        }

        // не проверено
        /// <summary>
        /// Get horses<br/>
        /// GET request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<ResponseResult<IEnumerable<HorseRetrieveDto>>> GetHorsesAsync(string accessToken)
        {
            HttpClient client = new();
            Uri uri = new(_hostUri, _horsesUri);
            HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var horse = JsonConvert.DeserializeObject<RetrieveArray<HorseRetrieveDto>>(responseText);
                return new ResponseResult<IEnumerable<HorseRetrieveDto>>(horse.Items);
            }

            return ReturnBadResponse<IEnumerable<HorseRetrieveDto>>(responseText, response.StatusCode);
        }

        // не проверено
        /// <summary>
        /// Create horse<br/>
        /// POST request
        /// </summary>
        /// <param name="accessToken">Access token</param>
        /// <param name="horse">Horse object for creation</param>
        /// <returns></returns>
        public async Task<ResponseResult<HorseRetrieveDto>> CreateHorseAsync(string accessToken, CreationHorseDto horse)
        {
            string json = JsonConvert.SerializeObject(horse);

            Uri uri = new(_hostUri, _horsesUri);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpClient client = new();

            HttpRequestMessage request = new(HttpMethod.Post, uri)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var retrieveHorse = JsonConvert.DeserializeObject<HorseRetrieveDto>(responseText);
                return new ResponseResult<HorseRetrieveDto>(retrieveHorse);
            }

            return ReturnBadResponse<HorseRetrieveDto>(responseText, response.StatusCode);
        }

        /// <summary>
        /// Update horse object data<br/>
        /// PUT request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="horse"></param>
        /// <returns></returns>
        public async Task<ResponseResult<HorseRetrieveDto>> UpdateHorseAsync(string accessToken, UpdatingHorseDto horse)
        {
            string json = JsonConvert.SerializeObject(horse);

            Uri uri = new(_hostUri, _horsesUri);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpClient client = new();

            HttpRequestMessage request = new(HttpMethod.Put, uri)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var retrieveHorse = JsonConvert.DeserializeObject<HorseRetrieveDto>(responseText);
                return new ResponseResult<HorseRetrieveDto>(retrieveHorse);
            }

            return ReturnBadResponse<HorseRetrieveDto>(responseText, response.StatusCode);
        }

        /// <summary>
        /// Delete horse<br/>
        /// DELETE request
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="horseId"></param>
        /// <returns></returns>
        public async Task<ResponseResult> DeleteHorseAsync(string accessToken, long horseId)
        {
            HttpClient client = new();
            Uri uri = new(_hostUri, $"{_horsesUri}?horseId={horseId}");
            HttpRequestMessage request = new(HttpMethod.Delete, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return new ResponseResult(response.StatusCode);
            }

            string responseText = await response.Content.ReadAsStringAsync();
            return ReturnBadResponse(responseText, response.StatusCode);
        }
        #endregion

        private ResponseResult<T> ReturnBadResponse<T>(string responseText, HttpStatusCode code) where T : class
        {
            var badResponse = JsonConvert.DeserializeObject<BadRequestDto>(responseText);
            if (badResponse != null)
            {
                return new ResponseResult<T>(null, (HttpStatusCode)badResponse.Status, badResponse.Errors);
            }

            return new ResponseResult<T>(null, code, new List<ResponseError>()
            {
                new()
                {
                    Title = "Unknown error",
                    Message = "Unknown error"
                }
            });
        }

        private ResponseResult ReturnBadResponse(string responseText, HttpStatusCode code)
        {
            var badResponse = JsonConvert.DeserializeObject<BadRequestDto>(responseText);
            if (badResponse != null)
            {
                return new ResponseResult((HttpStatusCode)badResponse.Status, badResponse.Errors);
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
