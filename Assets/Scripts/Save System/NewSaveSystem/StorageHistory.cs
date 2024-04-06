using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ford.SaveSystem.Ver2
{
    public class StorageHistory
    {
        private string _pathSaveHistory;
        private List<StorageAction> _history;
        public IReadOnlyCollection<StorageAction> History => _history;

        public StorageHistory(string pathDir)
        {
            _history = new();
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
                    var data = JsonConvert.DeserializeObject<List<StorageAction>>(json, new StorageHistoryConverter());

                    foreach (var action in data)
                    {
                        _history.Add(action);
                    }
                }
                sr.Close();
            }
        }

        public ICollection<StorageAction> RewriteHistory(StorageAction[] newHistory)
        {
            _history = new List<StorageAction>(newHistory);
            return History.ToList();
        }

        public void PushHistory(StorageAction action)
        {
            switch (action.ActionType)
            {
                case ActionType.CreateHorse:
                    _history.Add(action);
                    break;
                case ActionType.UpdateHorse:
                    var currentHorseActions = _history.Where(h => h.Data.HorseId == action.Data.HorseId);
                    var creationHorseAction = currentHorseActions.SingleOrDefault(h => h.ActionType == ActionType.CreateHorse);
                    var updateHorseActions = currentHorseActions.Where(h => h.ActionType == ActionType.UpdateHorse);

                    while (updateHorseActions.Any())
                    {
                        _history.Remove(updateHorseActions.First());
                    }

                    if (creationHorseAction != null)
                    {
                        _history.Remove(creationHorseAction);
                        action.ActionType = ActionType.CreateHorse;
                    }

                    _history.Add(action);
                    break;
                case ActionType.DeleteHorse:
                    currentHorseActions = _history.Where(h => h.Data.HorseId == action.Data.HorseId);
                    creationHorseAction = currentHorseActions.SingleOrDefault(h => h.ActionType == ActionType.CreateHorse);
                    updateHorseActions = currentHorseActions.Where(h => h.ActionType == ActionType.UpdateHorse);

                    while (updateHorseActions.Any())
                    {
                        _history.Remove(updateHorseActions.First());
                    }

                    if (creationHorseAction != null)
                    {
                        _history.Remove(creationHorseAction);
                    }
                    else
                    {
                        _history.Add(action);
                    }

                    break;
                case ActionType.CreateSave:
                    _history.Add(action);
                    break;
                case ActionType.UpdateSave:
                    var saves = _history.Where(s => s.Data is ISaveInfo);
                    var currentSaveActions = saves.Where(h => (h.Data as ISaveInfo).SaveId == (action.Data as ISaveInfo).SaveId);
                    var updateSaveActions = currentSaveActions.Where(h => h.ActionType == ActionType.UpdateSave);
                    var creationSaveAction = currentSaveActions.SingleOrDefault(h => h.ActionType == ActionType.CreateSave);

                    while (updateSaveActions.Any())
                    {
                        _history.Remove(updateSaveActions.First());
                    }

                    if (creationSaveAction != null)
                    {
                        var fullAction = new FullSaveInfo((SaveInfo)action.Data);

                        foreach (var bone in ((FullSaveInfo)creationSaveAction.Data).Bones)
                        {
                            fullAction.Bones.Add(bone);
                        }

                        _history.Remove(creationSaveAction);
                        action.Data = fullAction;
                        action.ActionType = ActionType.CreateSave;
                    }

                    _history.Add(action);
                    break;
                case ActionType.DeleteSave:
                    saves = _history.Where(s => s.Data is ISaveInfo);
                    currentSaveActions = _history.Where(h => (h.Data as ISaveInfo).SaveId == (action.Data as ISaveInfo).SaveId);
                    updateSaveActions = currentSaveActions.Where(h => h.ActionType == ActionType.UpdateSave);
                    creationSaveAction = currentSaveActions.SingleOrDefault(h => h.ActionType == ActionType.CreateSave);

                    while (updateSaveActions.Any())
                    {
                        _history.Remove(updateSaveActions.First());
                    }

                    if (creationSaveAction != null)
                    {
                        _history.Remove(creationSaveAction);
                    }
                    else
                    {
                        _history.Add(action);
                    }
                    break;
            }

            WriteChanges();
        }

        public void ClearHistory()
        {
            _history.Clear();
            WriteChanges();
        }

        private void WriteChanges()
        {
            using StreamWriter sw = new(_pathSaveHistory);
            var json = JsonConvert.SerializeObject(_history.ToList());
            sw.Write(json);
        }
    }
}
