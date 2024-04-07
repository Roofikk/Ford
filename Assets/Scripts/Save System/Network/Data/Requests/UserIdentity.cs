namespace Ford.WebApi.Data
{
    public class UserIdentity
    {
        public long? UserId { get; set; }
        public string UserName { get; set; }

        public UserIdentity(long userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }

        public UserIdentity(long userId)
        {
            UserId = userId;
        }

        public UserIdentity(string userName)
        {
            UserName = userName;
        }
    }
}
