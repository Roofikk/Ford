namespace Ford.SaveSystem.Ver2
{
    public class StorageAction<T> where T : IStorageAction
    {
        public ActionType ActionType { get; set; }
        public T Data { get; private set; }

        public StorageAction(ActionType action, T data)
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
