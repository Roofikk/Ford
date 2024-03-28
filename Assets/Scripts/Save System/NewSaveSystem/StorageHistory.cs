using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ford.SaveSystem.Ver2
{
    public class StorageHistory
    {
        private string _pathSaveHistory;
        public List<StorageAction<IStorageAction>> History { get; private set; }

        public StorageHistory(string pathDir)
        {
            History = new();
            _pathSaveHistory = Path.Combine(pathDir, "storageStack.json");

            if (!File.Exists(_pathSaveHistory))
            {
                var stream = File.Create(_pathSaveHistory);
                stream.Close();
            }
            else
            {
                StreamReader sr = new(_pathSaveHistory);
                string json = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                {
                    var data = JsonConvert.DeserializeObject<List<StorageAction<IStorageAction>>>(json, new StorageHistoryConverter());

                    foreach (var action in data)
                    {
                        History.Add(action);
                    }
                }
                sr.Close();
            }
        }

        public void PushHistory(StorageAction<IStorageAction> action)
        {
            switch (action.ActionType)
            {
                case ActionType.CreateHorse:
                    History.Add(action);
                    break;
                case ActionType.UpdateHorse:
                    var currentHorseActions = History.Where(h => h.Data.HorseId == action.Data.HorseId);
                    var creationHorseAction = currentHorseActions.SingleOrDefault(h => h.ActionType == ActionType.CreateHorse);
                    var updateHorseActions = currentHorseActions.Where(h => h.ActionType == ActionType.UpdateHorse);

                    while (updateHorseActions.Any())
                    {
                        History.Remove(updateHorseActions.First());
                    }

                    if (creationHorseAction != null)
                    {
                        History.Remove(creationHorseAction);
                        action.ActionType = ActionType.CreateHorse;
                    }

                    History.Add(action);
                    break;
                case ActionType.DeleteHorse:
                    currentHorseActions = History.Where(h => h.Data.HorseId == action.Data.HorseId);
                    creationHorseAction = currentHorseActions.SingleOrDefault(h => h.ActionType == ActionType.CreateHorse);
                    updateHorseActions = currentHorseActions.Where(h => h.ActionType == ActionType.UpdateHorse);

                    while (updateHorseActions.Any())
                    {
                        History.Remove(updateHorseActions.First());
                    }

                    if (creationHorseAction != null)
                    {
                        History.Remove(creationHorseAction);
                    }
                    else
                    {
                        History.Add(action);
                    }

                    break;
                case ActionType.CreateSave:
                    History.Add(action);
                    break;
                case ActionType.UpdateSave:
                    var saves = History.Where(s => s.Data is SaveInfo);
                    var currentSaveActions = saves.Where(h => (h.Data as SaveInfo).SaveId == (action.Data as SaveInfo).SaveId);
                    var updateSaveActions = currentSaveActions.Where(h => h.ActionType == ActionType.UpdateSave);
                    var creationSaveAction = currentSaveActions.SingleOrDefault(h => h.ActionType == ActionType.CreateSave);

                    while (updateSaveActions.Any())
                    {
                        History.Remove(updateSaveActions.First());
                    }

                    if (creationSaveAction != null)
                    {
                        var fullAction = new FullSaveInfo((SaveInfo)action.Data);

                        foreach (var bone in ((FullSaveInfo)creationSaveAction.Data).Bones)
                        {
                            fullAction.Bones.Add(bone);
                        }

                        History.Remove(creationSaveAction);
                        action.ActionType = ActionType.CreateSave;
                    }

                    History.Add(action);
                    break;
                case ActionType.DeleteSave:
                    currentSaveActions = History.Where(h => (h.Data as SaveInfo).SaveId == (action.Data as SaveInfo).SaveId);
                    updateSaveActions = currentSaveActions.Where(h => h.ActionType == ActionType.UpdateSave);
                    creationSaveAction = currentSaveActions.SingleOrDefault(h => h.ActionType == ActionType.CreateSave);

                    while (updateSaveActions.Any())
                    {
                        History.Remove(updateSaveActions.First());
                    }

                    if (creationSaveAction != null)
                    {
                        History.Remove(creationSaveAction);
                    }
                    else
                    {
                        History.Add(action);
                    }
                    break;
            }

            WriteChanges();
        }

        public void ClearHistory()
        {
            History.Clear();
            WriteChanges();
        }

        private void WriteChanges()
        {
            using StreamWriter sw = new(_pathSaveHistory);
            var json = JsonConvert.SerializeObject(History.ToList());
            sw.Write(json);
        }
    }
}
