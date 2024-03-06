using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public abstract class SaveSystemState
    {
        public abstract Task GetHorses(StorageSystem storage, Action<IEnumerable<HorseBase>> onRetrieve);
    }
}
