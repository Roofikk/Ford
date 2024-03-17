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

        public override Task<ICollection<SaveInfo>> GetSaves(StorageSystem storage, long horseId, int below = 0, int amount = 20)
        {
            Storage store = new();
            Task<ICollection<SaveInfo>> task = Task.Factory.StartNew(() => { return store.GetSaves(horseId, below, amount); });
            return Task.FromResult(task.Result);
        }

        public override Task<FullSaveInfo> GetFullSave(StorageSystem storage, long horseId, long saveId)
        {
            Storage store = new();
            Task<FullSaveInfo> task = Task.Factory.StartNew(() => { return store.GetFullSave(horseId, saveId); });
            return Task.FromResult(task.Result);
        }

        public override Task<SaveInfo> CreateSave(StorageSystem storage, FullSaveInfo save)
        {
            Storage store = new();
            Task<SaveInfo> task = Task.Factory.StartNew(() => { return store.CreateSave(save); });
            return Task.FromResult(task.Result);
        }

        public override Task<SaveInfo> UpdateSave(StorageSystem storage, SaveInfo save)
        {
            Storage store = new();
            Task<SaveInfo> task = Task.Factory.StartNew(() => { return store.UpdateSave(save); });
            return Task.FromResult(task.Result);
        }

        public override Task<bool> DeleteSave(StorageSystem storage, long saveId)
        {
            Storage store = new();
            Task<bool> task = Task.Factory.StartNew(() => { return store.DeleteSave(saveId); });
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
