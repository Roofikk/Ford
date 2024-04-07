using Ford.SaveSystem.Data;

public class UserPageData
{
    public long UserId { get; }
    public HorseUserDto User { get; }

    public UserPageData(long userId)
    {
        UserId = userId;
    }

    public UserPageData(HorseUserDto horseUserDto)
    {
        User = horseUserDto;
        UserId = horseUserDto.UserId;
    }
}
