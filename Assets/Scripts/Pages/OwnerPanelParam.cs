using Ford.WebApi.Data;
using System.Collections.Generic;

public class OwnerPanelParam
{
    public PageMode Mode { get; }
    public HorseUserDto Self { get; }
    public List<HorseUserDto> Users { get; }
    public string OwnerName { get; }
    public string OwnerPhoneNumber { get; }

    public OwnerPanelParam(PageMode mode, List<HorseUserDto> users)
    {
        Mode = mode;
        Users = users;
    }

    public OwnerPanelParam(PageMode mode, HorseUserDto self, List<HorseUserDto> users, string ownerName, string ownerPhoneNumber)
    {
        Mode = mode;
        Self = self;
        Users = users;
        OwnerName = ownerName;
        OwnerPhoneNumber = ownerPhoneNumber;
    }
}
