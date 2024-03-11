using Ford.WebApi.Data;
using System.Collections.Generic;

public class OwnerPanelParam
{
    public OwnerPanelMode Mode { get; }
    public List<HorseUserDto> Users { get; }

    public OwnerPanelParam(OwnerPanelMode mode, List<HorseUserDto> users)
    {
        Mode = mode;
        Users = users;
    }
}

public enum OwnerPanelMode
{
    Read,
    Write
}
