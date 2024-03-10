using Ford.WebApi.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public abstract class SaveSystemState
    {
        public abstract Task<ICollection<HorseBase>> GetHorses(StorageSystem storage, int below = 0, int above = 20);
        public abstract Task<HorseBase> CreateHorse(StorageSystem storage, CreationHorse horse);
        public abstract Task<HorseBase> UpdateHorse(StorageSystem storage, UpdatingHorse horse);
        public abstract Task<bool> DeleteHorse(StorageSystem storage, long horseId);
    }
}
