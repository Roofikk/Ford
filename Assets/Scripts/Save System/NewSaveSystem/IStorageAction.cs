namespace Ford.SaveSystem.Ver2
{
    public interface IStorageAction
    {
        public long HorseId { get; set; }
        public string ActionDescription { get; }
    }
}
