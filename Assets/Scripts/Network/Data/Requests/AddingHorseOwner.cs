namespace Ford.WebApi.Data
{
    internal class AddingHorseOwner
    {
        public long UserId { get; set; }
        public long HorseId { get; set; }
        public string OwnerAccessRole { get; set; }
    }
}
