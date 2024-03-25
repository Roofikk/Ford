using Ford.WebApi.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public abstract class SaveSystemState
    {
        public abstract Task<bool> Initiate(StorageSystem storage, SaveSystemState fromState);
        public abstract Task<ICollection<HorseBase>> GetHorses(StorageSystem storage, int below = 0, int amount = 20, 
            string orderByDate = "desc", string orderByName = "false");
        public abstract Task<HorseBase> CreateHorse(StorageSystem storage, CreationHorse horse);
        public abstract Task<HorseBase> UpdateHorse(StorageSystem storage, UpdatingHorse horse);
        public abstract Task<bool> DeleteHorse(StorageSystem storage, long horseId);
        public abstract Task<ICollection<SaveInfo>> GetSaves(StorageSystem storage, long horseId, int below = 0, int amount = 20);
        public abstract Task<FullSaveInfo> GetFullSave(StorageSystem storage, long horseId, long saveId);
        public abstract Task<SaveInfo> CreateSave(StorageSystem storage, FullSaveInfo save);
        public abstract Task<SaveInfo> UpdateSave(StorageSystem storage, SaveInfo save);
        public abstract Task<bool> DeleteSave(StorageSystem storage, long saveId);
        public abstract Task<bool> TryChangeState(StorageSystem storage);
    }
}
