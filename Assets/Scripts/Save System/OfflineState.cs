using Ford.SaveSystem.Data;
using Ford.SaveSystem.Data.Dtos;
using Ford.SaveSystem.Ver2;
using Ford.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public class OfflineState : SaveSystemState
    {
        Storage _storage;
        public override StorageHistory History => _storage.History;

        public OfflineState()
        {
            _storage = new();
        }

        internal override void GetReady(StorageSystem storage, SaveSystemState fromState)
        {
        }

        internal override async Task<bool> Initiate(StorageSystem storage)
        {
            _storage.PushAllHorses(storage.Horses.ToList());
            return await Task.FromResult(true);
        }

        internal override async Task<bool> RawInitiate(StorageSystem storage)
        {
            return await Initiate(storage);
        }

        internal override async Task<ICollection<HorseBase>> GetHorses(StorageSystem storage, int below = 0, int above = 20,
            string orderByDate = "desc", string orderByName = "false")
        {
            var result = await Task.Factory.StartNew(_storage.GetHorses);
            return result;
        }

        internal override async Task<HorseBase> CreateHorse(StorageSystem storage, CreationHorse horse)
        {
            Task<HorseBase> task = Task.Factory.StartNew(() => { return _storage.CreateHorse(horse); });
            await task;
            return await Task.FromResult(task.Result);
        }

        internal override Task<HorseBase> UpdateHorse(StorageSystem storage, UpdatingHorse horse)
        {
            Task<HorseBase> task = Task.Factory.StartNew(() => { return _storage.UpdateHorse(horse); });
            return Task.FromResult(task.Result);
        }

        internal override Task<bool> DeleteHorse(StorageSystem storage, long horseId)
        {
            Task<bool> task = Task.Factory.StartNew(() => { return _storage.DeleteHorse(horseId); });
            return Task.FromResult(task.Result);
        }

        internal override Task<ICollection<SaveInfo>> GetSaves(StorageSystem storage, long horseId, int below = 0, int amount = 20)
        {
            Task<ICollection<SaveInfo>> task = Task.Factory.StartNew(() => { return _storage.GetSaves(horseId, below, amount); });
            return Task.FromResult(task.Result);
        }

        internal override Task<FullSaveInfo> GetFullSave(StorageSystem storage, long horseId, long saveId)
        {
            Task<FullSaveInfo> task = Task.Factory.StartNew(() => { return _storage.GetFullSave(horseId, saveId); });
            return Task.FromResult(task.Result);
        }

        internal override Task<SaveInfo> CreateSave(StorageSystem storage, CreatingSaveDto save)
        {
            Task<SaveInfo> task = Task.Factory.StartNew(() => { return _storage.CreateSave(save); });
            return Task.FromResult(task.Result);
        }

        internal override Task<SaveInfo> UpdateSave(StorageSystem storage, ModifiedSaveDto save)
        {
            Task<SaveInfo> task = Task.Factory.StartNew(() => { return _storage.UpdateSave(save); });
            return Task.FromResult(task.Result);
        }

        internal override Task<bool> DeleteSave(StorageSystem storage, long saveId)
        {
            Task<bool> task = Task.Factory.StartNew(() => { return _storage.DeleteSave(saveId); });
            return Task.FromResult(task.Result);
        }

        internal override async Task<bool> CanChangeState(StorageSystem storage)
        {
            FordApiClient client = new();
            using var tokenStorage = new TokenStorage();
            var result = await client.GetUserInfoAsync(tokenStorage.GetAccessToken());

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var newResult = await client.RefreshTokenAndReply(tokenStorage.GetAccessToken(), client.GetUserInfoAsync);

                if (newResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return false;
                }
            }

            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }

        internal override void CloseState(StorageSystem storage)
        {
            _storage.ClearStorage();
        }
    }
}
