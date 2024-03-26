using Ford.SaveSystem.Ver2;
using Ford.WebApi.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public abstract class SaveSystemState
    {
        public virtual StorageHistory History { get; protected set; }
        internal abstract void GetReady(StorageSystem storage, SaveSystemState fromState);
        internal abstract Task<bool> Initiate(StorageSystem storage);
        internal abstract Task<bool> RawInitiate(StorageSystem storage);
        internal abstract Task<ICollection<HorseBase>> GetHorses(StorageSystem storage, int below = 0, int amount = 20, 
            string orderByDate = "desc", string orderByName = "false");
        internal abstract Task<HorseBase> CreateHorse(StorageSystem storage, CreationHorse horse);
        internal abstract Task<HorseBase> UpdateHorse(StorageSystem storage, UpdatingHorse horse);
        internal abstract Task<bool> DeleteHorse(StorageSystem storage, long horseId);
        internal abstract Task<ICollection<SaveInfo>> GetSaves(StorageSystem storage, long horseId, int below = 0, int amount = 20);
        internal abstract Task<FullSaveInfo> GetFullSave(StorageSystem storage, long horseId, long saveId);
        internal abstract Task<SaveInfo> CreateSave(StorageSystem storage, FullSaveInfo save);
        internal abstract Task<SaveInfo> UpdateSave(StorageSystem storage, SaveInfo save);
        internal abstract Task<bool> DeleteSave(StorageSystem storage, long saveId);
        internal abstract Task<bool> CanChangeState(StorageSystem storage);
        internal abstract void CloseState(StorageSystem storage);
    }
}
