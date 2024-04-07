using Ford.SaveSystem.Data;
using Ford.SaveSystem.Data.Dtos;
using Ford.SaveSystem.Ver2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ford.SaveSystem
{
    public class StorageSystem
    {
        private List<HorseBase> _horses = new();

        public StorageSystemStateEnum CurrentState { get; set; }
        public StorageSystemStateEnum PrevStateEnum { get; set; }
        private static SaveSystemState _prevState { get; set; }
        private static SaveSystemState _state { get; set; }
        public static SaveSystemState State => _state;
        internal IReadOnlyCollection<HorseBase> Horses => _horses;

        public StorageHistory History => _state.History;

        public event Action<StorageSystemStateEnum> OnReadyStateChanged;

        public StorageSystem()
        {
            switch (_state)
            {
                case OfflineState:
                    CurrentState = StorageSystemStateEnum.Offline;
                    break;
                case AuthorizedState:
                    CurrentState = StorageSystemStateEnum.Authorized;
                    break;
                default:
                    throw new Exception("State has been not initiated");
            }
        }

        public static void Initiate(StorageSystemStateEnum state)
        {
            switch (state)
            {
                case StorageSystemStateEnum.Offline:
                    _state = new OfflineState();
                    break;
                case StorageSystemStateEnum.Authorized:
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

        public async Task<SaveInfo> CreateSave(CreatingSaveDto save)
        {
            var result = await _state.CreateSave(this, save);
            return result;
        }

        public async Task<SaveInfo> UpdateSave(ModifiedSaveDto save)
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

        public void ChangeState(StorageSystemStateEnum state)
        {
            if (state == CurrentState)
            {
                return;
            }

            _prevState = _state;

            switch (state)
            {
                case StorageSystemStateEnum.Authorized:
                    PrevStateEnum = StorageSystemStateEnum.Offline;
                    CurrentState = StorageSystemStateEnum.Authorized;
                    _state = new AuthorizedState();
                    break;
                case StorageSystemStateEnum.Offline:
                    PrevStateEnum = StorageSystemStateEnum.Authorized;
                    CurrentState = StorageSystemStateEnum.Offline;
                    _state = new OfflineState();
                    break;
            }

            _state.GetReady(this, _prevState);
            OnReadyStateChanged?.Invoke(CurrentState);
        }

        public async Task<bool> ApplyTransition()
        {
            var result = await _state.Initiate(this);

            if (result)
            {
                _prevState?.CloseState(this);
            }

            return result;
        }

        public async Task<bool> RawApplyTransition()
        {
            var result = await _state.RawInitiate(this);

            if (result)
            {
                _prevState?.CloseState(this);
            }

            return result;
        }

        public void DeclineTransition()
        {
            _state.CloseState(this);
            _state = _prevState;
        }
    }

    public enum StorageSystemStateEnum
    {
        Authorized,
        Offline,
    }
}
