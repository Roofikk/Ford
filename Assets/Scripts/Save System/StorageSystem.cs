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
        private static SaveSystemState _prevState { get; set; }
        private static SaveSystemState _state { get; set; }
        public static SaveSystemState State => _state;
        public static SaveSystemState PrevState => _prevState;
        internal IReadOnlyCollection<HorseBase> Horses => _horses;

        public StorageSystem()
        {
            switch (_state)
            {
                case UnauthorizedState:
                    CurrentState = SaveSystemStateEnum.Unauthorized;
                    break;
                case AuthorizedState:
                    CurrentState = SaveSystemStateEnum.Authorized;
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
                    _state = new UnauthorizedState();
                    break;
                case SaveSystemStateEnum.Authorized:
                    _state = new AuthorizedState();
                    break;
            }
        }

        #region horses
        public async Task<ICollection<HorseBase>> GetHorses(int below = 0, int above = 20, bool force = true)
        {
            if (force)
            {
                _horses = (await _state.GetHorses(this, below, above)).ToList();
            }
            else
            {
                _horses ??= (await _state.GetHorses(this, above, below)).ToList();
            }

            return _horses;
        }

        public async Task<HorseBase> CreateHorse(CreationHorse horse)
        {
            var createdHorse = await _state.CreateHorse(this, horse);
            return createdHorse;
        }

        public async Task<HorseBase> UpdateHorse(UpdatingHorse horse)
        {
            var updatingHorse = await _state.UpdateHorse(this, horse);
            return updatingHorse;
        }

        public async Task<bool> DeleteHorse(long horseId)
        {
            bool result = await _state.DeleteHorse(this, horseId);
            return result;
        }
        #endregion

        #region Save
        public async Task<ICollection<SaveInfo>> GetSaves(long horseId, int below = 0, int amount = 20)
        {
            var result = await _state.GetSaves(this, horseId, below, amount);
            return result;
        }

        public async Task<FullSaveInfo> GetSave(long horseId, long saveId)
        {
            var result = await _state.GetFullSave(this, horseId, saveId);
            return result;
        }

        public async Task<SaveInfo> CreateSave(FullSaveInfo save)
        {
            var result = await _state.CreateSave(this, save);
            return result;
        }

        public async Task<SaveInfo> UpdateSave(SaveInfo save)
        {
            var result = await _state.UpdateSave(this, save);
            return result;
        }

        public async Task<bool> DeleteSave(long saveId)
        {
            var result = await _state.DeleteSave(this, saveId);
            return result;
        }
        #endregion

        public async Task<bool> CanChangeState()
        {
            var result = await _state.CanChangeState(this);
            return result;
        }

        public void ChangeState(SaveSystemStateEnum state)
        {
            switch (state)
            {
                case SaveSystemStateEnum.Authorized:
                    _prevState = _state;
                    PrevStateEnum = SaveSystemStateEnum.Unauthorized;

                    CurrentState = SaveSystemStateEnum.Authorized;
                    _state = new AuthorizedState();
                    _state.GetReady(this, _prevState);
                    OnReadyStateChanged?.Invoke(CurrentState);
                    break;
                case SaveSystemStateEnum.Unauthorized:
                    _prevState = _state;
                    PrevStateEnum = SaveSystemStateEnum.Authorized;

                    CurrentState = SaveSystemStateEnum.Unauthorized;
                    _state = new UnauthorizedState();
                    _state.GetReady(this, _prevState);
                    OnReadyStateChanged?.Invoke(CurrentState);
                    break;
            }
        }

        public async Task<bool> ApplyTransition()
        {
            _prevState.CloseState(this);
            return await _state.Initiate(this);
        }

        public async Task<bool> DeclineTransition()
        {
            _prevState.CloseState(this);
            return await _state.RawInitiate(this);
        }
    }

    public enum SaveSystemStateEnum
    {
        Authorized,
        Unauthorized,
    }
}
