using Ford.WebApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public class StorageSystem
    {
        private List<HorseBase> _horses;

        public SaveSystemStateEnum CurrentState { get; set; }
        public event Action<SaveSystemStateEnum> OnReadyStateChanged;
        public SaveSystemStateEnum PrevStateEnum { get; set; }
        private static SaveSystemState PrevState { get; set; }
        private static SaveSystemState State { get; set; }
        internal IReadOnlyCollection<HorseBase> Horses => _horses;

        public StorageSystem()
        {
            switch (State)
            {
                case UnauthorizedState:
                    CurrentState = SaveSystemStateEnum.Unauthorized;
                    break;
                case AuthorizedState:
                    CurrentState = SaveSystemStateEnum.Autorized;
                    break;
                default:
                    throw new Exception("State has been not initiated");
            }
        }

        public static void Initiate(SaveSystemStateEnum state)
        {
            switch (state)
            {
                case SaveSystemStateEnum.Unauthorized:
                    State = new UnauthorizedState();
                    break;
                case SaveSystemStateEnum.Autorized:
                    State = new AuthorizedState();
                    break;
            }
        }

        #region horses
        public async Task<ICollection<HorseBase>> GetHorses(int below = 0, int above = 20, bool force = true)
        {
            if (force)
            {
                _horses = (await State.GetHorses(this, below, above)).ToList();
            }
            else
            {
                _horses ??= (await State.GetHorses(this, above, below)).ToList();
            }

            return _horses;
        }

        public async Task<HorseBase> CreateHorse(CreationHorse horse)
        {
            var createdHorse = await State.CreateHorse(this, horse);
            return createdHorse;
        }

        public async Task<HorseBase> UpdateHorse(UpdatingHorse horse)
        {
            var updatingHorse = await State.UpdateHorse(this, horse);
            return updatingHorse;
        }

        public async Task<bool> DeleteHorse(long horseId)
        {
            bool result = await State.DeleteHorse(this, horseId);
            return result;
        }
        #endregion

        #region Save
        public async Task<ICollection<SaveInfo>> GetSaves(long horseId, int below = 0, int amount = 20)
        {
            var result = await State.GetSaves(this, horseId, below, amount);
            return result;
        }

        public async Task<FullSaveInfo> GetSave(long horseId, long saveId)
        {
            var result = await State.GetFullSave(this, horseId, saveId);
            return result;
        }

        public async Task<SaveInfo> CreateSave(FullSaveInfo save)
        {
            var result = await State.CreateSave(this, save);
            return result;
        }

        public async Task<SaveInfo> UpdateSave(SaveInfo save)
        {
            var result = await State.UpdateSave(this, save);
            return result;
        }

        public async Task<bool> DeleteSave(long saveId)
        {
            var result = await State.DeleteSave(this, saveId);
            return result;
        }
        #endregion

        protected internal void ChangeState(SaveSystemStateEnum state)
        {
            switch (state)
            {
                case SaveSystemStateEnum.Autorized:
                    PrevState = State;
                    PrevStateEnum = SaveSystemStateEnum.Unauthorized;

                    CurrentState = SaveSystemStateEnum.Autorized;
                    State = new AuthorizedState();
                    State.GetReady(this, PrevState);
                    OnReadyStateChanged?.Invoke(CurrentState);
                    break;
                case SaveSystemStateEnum.Unauthorized:
                    PrevState = State;
                    PrevStateEnum = SaveSystemStateEnum.Autorized;

                    CurrentState = SaveSystemStateEnum.Unauthorized;
                    State = new UnauthorizedState();
                    State.GetReady(this, PrevState);
                    OnReadyStateChanged?.Invoke(CurrentState);
                    break;
            }
        }

        public async Task<bool> ApplyTransition()
        {
            return await State.Initiate(this);
        }

        public async Task<bool> DeclineTransition()
        {
            ChangeState(PrevStateEnum);
            return await State.Initiate(this);
        }
    }

    public enum SaveSystemStateEnum
    {
        Autorized,
        Unauthorized,
    }
}
