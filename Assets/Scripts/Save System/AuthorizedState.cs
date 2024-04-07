using Ford.SaveSystem.Data;
using Ford.SaveSystem.Data.Dtos;
using Ford.SaveSystem.Ver2;
using Ford.WebApi;
using Ford.WebApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public class AuthorizedState : SaveSystemState
    {
        public FordApiClient ApiClient { get; set; }
        public override StorageHistory History { get; protected set; }

        public AuthorizedState()
        {
            ApiClient = new();
        }

        internal override void GetReady(StorageSystem storage, SaveSystemState fromState)
        {
            if (fromState is OfflineState unauthorizedState)
            {
                History = unauthorizedState.History;
            }
        }

        internal override async Task<bool> Initiate(StorageSystem storage)
        {
            if (History.History.Count == 0)
            {
                History.ClearHistory();
                return true;
            }

            var newResult = new ResponseResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<ResponseError>()
                {
                    new ResponseError("ResponseNotRecieved", "Response not recieved")
                }
            };

            using (var tokenStorage = new TokenStorage())
            {
                var result = await ApiClient.PushHistory(tokenStorage.GetAccessToken(), History.History.ToList());
                newResult = await RefreshTokenAndRetrieveResult(
                    result, tokenStorage.GetAccessToken(), ApiClient.PushHistory, History.History.ToList());
            }

            if (newResult.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            History.ClearHistory();
            return newResult.StatusCode == HttpStatusCode.OK;
        }

        internal override async Task<bool> RawInitiate(StorageSystem storage)
        {
            History.ClearHistory();
            return await Task.FromResult(true);
        }

        internal override async Task<ICollection<HorseBase>> GetHorses(StorageSystem storage, int below = 0, int amount = 20,
            string orderByDate = "desc", string orderByName = "false")
        {
            var newResult = new ResponseResult<ICollection<HorseBase>>(null)
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<ResponseError>()
                {
                    new ResponseError("ResponseNotRecieved", "Response not recieved")
                }
            };

            using (var tokenStorage = new TokenStorage())
            {
                var result = await ApiClient.GetHorsesAsync(tokenStorage.GetAccessToken(), below, amount, orderByDate, orderByName);
                newResult = await RefreshTokenAndRetrieveResult(
                    result, tokenStorage.GetAccessToken(), ApiClient.GetHorsesAsync, below, amount, orderByDate, orderByName);
            }

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(StorageSystemStateEnum.Offline);
                await storage.ApplyTransition();
                return await storage.GetHorses();
            }

            if (newResult.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            return newResult.Content;
        }

        internal override async Task<HorseBase> CreateHorse(StorageSystem storage, CreationHorse horse)
        {
            var newResult = new ResponseResult<HorseBase>(null)
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<ResponseError>()
                {
                    new ResponseError("ResponseNotRecieved", "Response not recieved")
                }
            };

            using (var tokenStorage = new TokenStorage())
            {
                var result = await ApiClient.CreateHorseAsync(tokenStorage.GetAccessToken(), horse);
                newResult = await RefreshTokenAndRetrieveResult(result, tokenStorage.GetAccessToken(), ApiClient.CreateHorseAsync, horse);
            }

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(StorageSystemStateEnum.Offline);
                await storage.ApplyTransition();
                return await storage.CreateHorse(horse);
            }

            if (newResult.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            return newResult.Content;
        }

        internal override async Task<HorseBase> UpdateHorse(StorageSystem storage, UpdatingHorse horse)
        {
            var newResult = new ResponseResult<HorseBase>(null)
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<ResponseError>()
                {
                    new ResponseError("ResponseNotRecieved", "Response not recieved")
                }
            };

            using (var tokenStorage = new TokenStorage())
            {
                var result = await ApiClient.UpdateHorseAsync(tokenStorage.GetRefreshToken(), horse);
                newResult = await RefreshTokenAndRetrieveResult(result, tokenStorage.GetAccessToken(), ApiClient.UpdateHorseAsync, horse);
            }

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(StorageSystemStateEnum.Offline);
                await storage.ApplyTransition();
                return await storage.CreateHorse(new CreationHorse()
                {
                    Name = horse.Name,
                    Description = horse.Description,
                    Sex = horse.Sex,
                    BirthDate = horse.BirthDate,
                    City = horse.City,
                    Region = horse.Region,
                    Country = horse.Country,
                    OwnerName = "Безхозная",
                    OwnerPhoneNumber = "Неизвестно",
                });
            }

            if (newResult.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            return newResult.Content;
        }

        internal override async Task<bool> DeleteHorse(StorageSystem storage, long horseId)
        {
            var newResult = new ResponseResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<ResponseError>()
                {
                    new ResponseError("ResponseNotRecieved", "Response not recieved")
                }
            };

            using (var tokenStorage = new TokenStorage())
            {
                var result = await ApiClient.DeleteHorseAsync(tokenStorage.GetAccessToken(), horseId);
                newResult = await RefreshTokenAndRetrieveResult(result, tokenStorage.GetAccessToken(), ApiClient.DeleteHorseAsync, horseId);
            }

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(StorageSystemStateEnum.Offline);
                return false;
            }

            if (newResult.StatusCode == HttpStatusCode.BadRequest)
            {
                return false;
            }

            return true;
        }

        internal override async Task<ICollection<SaveInfo>> GetSaves(StorageSystem storage, long horseId, int below = 0, int amount = 20)
        {
            var newResult = new ResponseResult<ICollection<SaveInfo>>(null)
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<ResponseError>()
                {
                    new ResponseError("ResponseNotRecieved", "Response not recieved")
                }
            };

            using (var tokenStorage = new TokenStorage())
            {
                var result = await ApiClient.GetSavesAsync(tokenStorage.GetAccessToken(), horseId, below, amount);
                newResult = await RefreshTokenAndRetrieveResult(
                    result, tokenStorage.GetAccessToken(), ApiClient.GetSavesAsync, horseId, below, amount);
            }

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(StorageSystemStateEnum.Offline);
                await storage.ApplyTransition();
                return await storage.GetSaves(horseId, below, amount);
            }

            if (newResult.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            return newResult.Content;
        }

        internal override async Task<FullSaveInfo> GetFullSave(StorageSystem storage, long horseId, long saveId)
        {
            var newResult = new ResponseResult<FullSaveInfo>(null)
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<ResponseError>()
                {
                    new ResponseError("ResponseNotRecieved", "Response not recieved")
                }
            };

            using (var tokenStorage = new TokenStorage())
            {
                var result = await ApiClient.GetSaveAsync(tokenStorage.GetAccessToken(), horseId, saveId);
                newResult = await RefreshTokenAndRetrieveResult(
                    result, tokenStorage.GetAccessToken(), ApiClient.GetSaveAsync, horseId, saveId);
            }

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(StorageSystemStateEnum.Offline);
                await storage.ApplyTransition();
                return await storage.GetSave(horseId, saveId);
            }

            if (newResult.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            return newResult.Content;
        }

        internal override async Task<SaveInfo> CreateSave(StorageSystem storage, CreatingSaveDto save)
        {
            var newResult = new ResponseResult<SaveInfo>(null)
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<ResponseError>()
                {
                    new ResponseError("ResponseNotRecieved", "Response not recieved")
                }
            };

            using (var tokenStorage = new TokenStorage())
            {
                var result = await ApiClient.CreateSaveAsync(tokenStorage.GetAccessToken(), save);
                newResult = await RefreshTokenAndRetrieveResult(result, tokenStorage.GetAccessToken(), ApiClient.CreateSaveAsync, save);
            }

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(StorageSystemStateEnum.Offline);
                await storage.ApplyTransition();
                return await storage.CreateSave(save);
            }

            if (newResult.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            return newResult.Content;
        }

        internal override async Task<SaveInfo> UpdateSave(StorageSystem storage, ModifiedSaveDto save)
        {
            var newResult = new ResponseResult<SaveInfo>(null)
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<ResponseError>()
                {
                    new ResponseError("ResponseNotRecieved", "Response not recieved")
                }
            };

            using (var tokenStorage = new TokenStorage())
            {
                var result = await ApiClient.UpdateSaveInfoAsync(tokenStorage.GetAccessToken(), save);
                newResult = await RefreshTokenAndRetrieveResult(result, tokenStorage.GetAccessToken(), ApiClient.UpdateSaveInfoAsync, save);
            }

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(StorageSystemStateEnum.Offline);
                await storage.ApplyTransition();
                return await storage.UpdateSave(save);
            }

            if (newResult.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            return newResult.Content;
        }

        internal override async Task<bool> DeleteSave(StorageSystem storage, long saveId)
        {
            var newResult = new ResponseResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<ResponseError>()
                {
                    new ResponseError("ResponseNotRecieved", "Response not recieved")
                }
            };

            using (var tokenStorage = new TokenStorage())
            {
                var result = await ApiClient.DeleteSaveAsync(tokenStorage.GetAccessToken(), saveId);
                newResult = await RefreshTokenAndRetrieveResult(result, tokenStorage.GetAccessToken(), ApiClient.DeleteSaveAsync, saveId);
            }

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(StorageSystemStateEnum.Offline);
                await storage.ApplyTransition();
                return false;
            }

            if (newResult.StatusCode == HttpStatusCode.BadRequest)
            {
                return false;
            }

            return true;
        }

        internal override async Task<bool> CanChangeState(StorageSystem storage)
        {
            return await Task.FromResult(true);
        }

        internal override void CloseState(StorageSystem storage)
        {
            ApiClient = null;
            History = null;
        }

        #region Help methods
        private async Task<ResponseResult> RefreshTokenAndRetrieveResult(ResponseResult result,
            string accessToken, Func<string, Task<ResponseResult>> repeat)
        {
            if (result.StatusCode != HttpStatusCode.Unauthorized)
            {
                return await Task.FromResult(result);
            }

            FordApiClient client = new();
            return await client.RefreshTokenAndReply(accessToken, repeat);
        }

        private async Task<ResponseResult<T>> RefreshTokenAndRetrieveResult<T>(ResponseResult<T> result, 
            string accessToken, Func<string, Task<ResponseResult<T>>> repeat) where T : class
        {
            if (result.StatusCode != HttpStatusCode.Unauthorized)
            {
                return await Task.FromResult(result);
            }

            return await ApiClient.RefreshTokenAndReply(accessToken, repeat);
        }

        private async Task<ResponseResult> RefreshTokenAndRetrieveResult<TParam>(ResponseResult result,
            string accessToken, Func<string, TParam, Task<ResponseResult>> repeat, TParam param)
        {
            if (result.StatusCode != HttpStatusCode.Unauthorized)
            {
                return await Task.FromResult(result);
            }

            FordApiClient client = new();
            return await client.RefreshTokenAndReply(accessToken, repeat, param);
        }

        private async Task<ResponseResult<TResult>> RefreshTokenAndRetrieveResult<TParam, TResult>(ResponseResult<TResult> result,
            string accessToken, Func<string, TParam, Task<ResponseResult<TResult>>> repeat, TParam param)
            where TResult : class
        {
            if (result.StatusCode != HttpStatusCode.Unauthorized)
            {
                return await Task.FromResult(result);
            }

            FordApiClient client = new();
            return await client.RefreshTokenAndReply(accessToken, repeat, param);
        }

        private async Task<ResponseResult<TResult>> RefreshTokenAndRetrieveResult<TParam1, TParam2, TResult>(ResponseResult<TResult> result,
            string accessToken, Func<string, TParam1, TParam2, Task<ResponseResult<TResult>>> repeat, TParam1 param1, TParam2 param2)
            where TResult : class
        {
            if (result.StatusCode != HttpStatusCode.Unauthorized)
            {
                return await Task.FromResult(result);
            }

            FordApiClient client = new();
            return await client.RefreshTokenAndReply(accessToken, repeat, param1, param2);
        }

        private async Task<ResponseResult<TResult>> RefreshTokenAndRetrieveResult<TParam1, TParam2, TParam3, TResult>(ResponseResult<TResult> result,
            string accessToken, Func<string, TParam1, TParam2, TParam3, Task<ResponseResult<TResult>>> repeat, TParam1 param1, TParam2 param2, TParam3 param3)
            where TResult : class
        {
            if (result.StatusCode != HttpStatusCode.Unauthorized)
            {
                return await Task.FromResult(result);
            }

            FordApiClient client = new();
            return await client.RefreshTokenAndReply(accessToken, repeat, param1, param2, param3);
        }
        private async Task<ResponseResult<TResult>> RefreshTokenAndRetrieveResult<TParam1, TParam2, TParam3, TParam4, TResult>(
            ResponseResult<TResult> result, string accessToken, Func<string, TParam1, TParam2, TParam3, TParam4, Task<ResponseResult<TResult>>> repeat, 
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            where TResult : class
        {
            if (result.StatusCode != HttpStatusCode.Unauthorized)
            {
                return await Task.FromResult(result);
            }

            FordApiClient client = new();
            return await client.RefreshTokenAndReply(accessToken, repeat, param1, param2, param3, param4);
        }
        #endregion
    }
}
