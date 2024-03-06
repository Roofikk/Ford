namespace Ford.SaveSystem.Data
{
    internal class StorageSettingsData
    {
        public string LastSaveFileName { get; set; } = null!;
        public long IncrementSave { get; set; }
        public long IncrementHorse { get; set; }
    }
}
