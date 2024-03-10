using Ford.SaveSystem.Ver2;
using Ford.WebApi;
using Ford.WebApi.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public class AuthorizedState : SaveSystemState
    {
        public override async Task<ICollection<HorseBase>> GetHorses(StorageSystem storage, int below = 0, int above = 20)
        {
            FordApiClient client = new FordApiClient();
            Storage store = new();
            var accessToken = store.GetAccessToken();

            var result = await client.GetHorsesAsync(accessToken);
            var newResult = await RefreshTokenAndRetrieveResult(result, accessToken, client.GetHorsesAsync, below, above);

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(SaveSystemStateEnum.Unauthorized);
                return await storage.GetHorses();
            }

            return newResult.Content;
        }

        public override async Task<HorseBase> CreateHorse(StorageSystem storage, CreationHorse horse)
        {
            FordApiClient client = new();
            Storage store = new();
            var accessToken = store.GetAccessToken();

            var result = await client.CreateHorseAsync(accessToken, horse);
            var newResult = await RefreshTokenAndRetrieveResult(result, accessToken, client.CreateHorseAsync, horse);

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(SaveSystemStateEnum.Unauthorized);
                return await storage.CreateHorse(horse);
            }

            return newResult.Content;
        }

        public override async Task<HorseBase> UpdateHorse(StorageSystem storage, UpdatingHorse horse)
        {
            FordApiClient client = new();
            Storage store = new();
            var accessToken = store.GetAccessToken();

            var result = await client.UpdateHorseAsync(accessToken, horse);
            var newResult = await RefreshTokenAndRetrieveResult(result, accessToken, client.UpdateHorseAsync, horse);

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(SaveSystemStateEnum.Unauthorized);
            }

            return newResult.Content;
        }

        public override async Task<bool> DeleteHorse(StorageSystem storage, long horseId)
        {
            FordApiClient client = new();
            Storage store = new();
            var accessToken = store.GetAccessToken();

            var result = await client.DeleteHorseAsync(accessToken, horseId);
            var newResult = await RefreshTokenAndRetrieveResult(result, accessToken, client.DeleteHorseAsync, horseId);

            if (newResult.StatusCode == HttpStatusCode.Unauthorized || newResult.StatusCode == HttpStatusCode.InternalServerError)
            {
                storage.ChangeState(SaveSystemStateEnum.Unauthorized);
                return false;
            }

            return true;
        }

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

            FordApiClient client = new();
            return await client.RefreshTokenAndReply(accessToken, repeat);
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
    }
}
