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
                    var actions = History.Where(h => h.ActionType == ActionType.UpdateHorse &&
                        (h.Data as HorseBase).HorseId == (action.Data as HorseBase).HorseId);

                    while (actions.Any())
                    {
                        History.Remove(actions.First());
                    }
                    History.Add(action);
                    break;
                case ActionType.DeleteHorse:
                    var updateActions = History.Where(h => h.ActionType == ActionType.UpdateHorse &&
                        (h.Data as HorseBase).HorseId == (action.Data as HorseBase).HorseId);

                    while (updateActions.Any())
                    {
                        History.Remove(updateActions.First());
                    }

                    var createAction = History.SingleOrDefault(h => h.ActionType == ActionType.CreateHorse &&
                        (h.Data as HorseBase).HorseId == (action.Data as HorseBase).HorseId);

                    if (createAction != null)
                    {
                        History.Remove(createAction);
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
                    actions = History.Where(h => h.ActionType == ActionType.UpdateSave &&
                        (h.Data as SaveInfo).SaveId == (action.Data as SaveInfo).SaveId);

                    while (actions.Any())
                    {
                        History.Remove(actions.First());
                    }

                    History.Add(action);
                    break;
                case ActionType.DeleteSave:
                    updateActions = History.Where(h => h.ActionType == ActionType.UpdateSave &&
                        (h.Data as SaveInfo).SaveId == (action.Data as SaveInfo).SaveId);

                    while (updateActions.Any())
                    {
                        History.Remove(updateActions.First());
                    }

                    createAction = History.SingleOrDefault(h => h.ActionType == ActionType.CreateSave &&
                        (h.Data as SaveInfo).SaveId == (action.Data as SaveInfo).SaveId);

                    if (createAction != null)
                    {
                        History.Remove(createAction);
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
