using Ford.SaveSystem.Ver2;
using Ford.SaveSystem.Ver2.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public class UnauthorizedState : SaveSystemState
    {
        public override Task GetHorses(StorageSystem storage, Action<IEnumerable<HorseBase>> onRetrieve)
        {
            Storage store = new();
            store.GetHorses();
            onRetrieve?.Invoke(new List<HorseData>());
            return Task.CompletedTask;
        }
    }
}
