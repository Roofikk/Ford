namespace Ford.SaveSystem.Ver2
{
    public class StorageAction
    {
        public ActionType ActionType { get; set; }
        public IStorageData Data { get; set; }

        public StorageAction(ActionType action, IStorageData data)
        {
            ActionType = action;
            Data = data;
        }
    }

    public enum ActionType
    {
        CreateHorse,
        UpdateHorse,
        DeleteHorse,
        CreateSave,
        UpdateSave,
        DeleteSave,
    }
}
