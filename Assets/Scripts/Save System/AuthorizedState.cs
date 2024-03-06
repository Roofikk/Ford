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
        public override async Task GetHorses(StorageSystem storage, Action<IEnumerable<HorseBase>> onRetrieve)
        {
            FordApiClient client = new FordApiClient();
            Storage store = new();
            var accessToken = store.GetAccessToken();

            var result = await client.GetHorsesAsync(accessToken);
            var newResult = await RefreshTokenAndRetrieveResult(result, accessToken, client.GetHorsesAsync);

            if (newResult.StatusCode == HttpStatusCode.Unauthorized)
            {
                storage.ChangeState(SaveSystemStateEnum.Unauthorized);
            }

            onRetrieve?.Invoke(newResult.Content);
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
    }
}
