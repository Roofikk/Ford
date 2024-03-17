using Ford.WebApi.Data;
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

        #region horses
        public async Task<ICollection<HorseBase>> GetHorses(int below = 0, int above = 20, bool force = true)
        {
            if (force)
            {
                horses = (await State.GetHorses(this, below, above)).ToList();
                return horses;
            }
            else
            {
                if (horses == null)
                {
                    horses = (await State.GetHorses(this, above, below)).ToList();
                    return horses;
                }
                else
                {
                    return await Task.FromResult(horses);
                }
            }
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
