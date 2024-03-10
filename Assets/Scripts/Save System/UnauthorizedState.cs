using Ford.SaveSystem.Ver2;
using Ford.WebApi;
using Ford.WebApi.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public class UnauthorizedState : SaveSystemState
    {
        public override async Task<ICollection<HorseBase>> GetHorses(StorageSystem storage, int below = 0, int above = 20)
        {
            Storage store = new();
            Task<ICollection<HorseBase>> task = Task.Run(store.GetHorses);
            Task reconnectTask = Reconnect(storage);
            await Task.Run(async () => await Reconnect(storage)).ConfigureAwait(false);
            reconnectTask.Start();

            return await Task.FromResult(task.Result);
        }

        public override async Task<HorseBase> CreateHorse(StorageSystem storage, CreationHorse horse)
        {
            Storage store = new();
            Task<HorseBase> task = Task.Factory.StartNew(() => { return store.CreateHorse(horse); });
            return await Task.FromResult(task.Result);
        }

        public override Task<HorseBase> UpdateHorse(StorageSystem storage, UpdatingHorse horse)
        {
            Storage store = new();
            Task<HorseBase> task = Task.Factory.StartNew(() => { return store.UpdateHorse(horse); });
            return Task.FromResult(task.Result);
        }

        public override Task<bool> DeleteHorse(StorageSystem storage, long horseId)
        {
            Storage store = new();
            Task<bool> task = Task.Factory.StartNew(() => { return store.DeleteHorse(horseId); });
            return Task.FromResult(task.Result);
        }

        private async Task Reconnect(StorageSystem storage)
        {
            FordApiClient client = new();
            Storage store = new();
            var accessToken = store.GetAccessToken();
            var result = await client.GetUserInfoAsync(accessToken);

            if (result.Content != null)
            {
                storage.ChangeState(SaveSystemStateEnum.Autorized);
            }
        }
    }
}
