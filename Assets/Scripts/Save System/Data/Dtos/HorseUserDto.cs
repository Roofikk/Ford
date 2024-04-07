namespace Ford.SaveSystem.Data
{
    public class HorseUserDto
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsOwner { get; set; }
        public string AccessRole { get; set; }
    }
}
