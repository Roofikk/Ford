using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ford.SaveSystem

{
    public class StorageSystem
    {
        private List<HorseBase> horses;
        public SaveSystemStateEnum CurrentState { get; set; }
        private SaveSystemState State { get; set; }

        public StorageSystem()
        {
            if (Player.IsLoggedIn)
            {
                CurrentState = SaveSystemStateEnum.Autorized;
                State = new AuthorizedState();
            }
            else
            {
                CurrentState = SaveSystemStateEnum.Unauthorized;
                State = new UnauthorizedState();
            }
        }

        public Task GetHorses(Action<IEnumerable<HorseBase>> onRetrieve, bool force = false)
        {
            if (force)
            {
                return State.GetHorses(this, (result) =>
                {
                    horses = result.ToList();
                    onRetrieve(result);
                });
            }
            else
            {
                if (horses == null)
                {
                    return State.GetHorses(this, (result) =>
                    {
                        horses = result.ToList();
                        onRetrieve(result);
                    });
                }
                else
                {
                    onRetrieve?.Invoke(horses);
                    return Task.CompletedTask;
                }
            }
        }

        internal void ChangeState(SaveSystemStateEnum state)
        {
            switch (state)
            {
                case SaveSystemStateEnum.Autorized:
                    CurrentState = SaveSystemStateEnum.Autorized;
                    State = new AuthorizedState();
                    break;
                case SaveSystemStateEnum.Unauthorized:
                    CurrentState = SaveSystemStateEnum.Unauthorized;
                    State = new UnauthorizedState();
                    break;
            }
        }
    }

    public enum SaveSystemStateEnum
    {
        Autorized,
        Unauthorized,
    }
}
