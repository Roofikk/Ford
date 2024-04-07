using Ford.SaveSystem.Data;

namespace Ford.WebApi.Data
{
    public class AddingHorseOwner
    {
        public long UserId { get; set; }
        public long HorseId { get; set; }
        public UserAccessRole OwnerAccessRole { get; set; }
    }

    internal class AddingHorseOwnerDto
    {
        public long UserId { get; set; }
        public long HorseId { get; set; }
        public string OwnerAccessRole { get; set; }

        public AddingHorseOwnerDto(AddingHorseOwner owner)
        {
            UserId = owner.UserId;
            HorseId = owner.HorseId;
            OwnerAccessRole = owner.OwnerAccessRole.ToString();
        }
    }
}
