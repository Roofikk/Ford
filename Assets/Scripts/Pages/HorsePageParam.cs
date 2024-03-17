using Ford.SaveSystem;

public class HorsePageParam
{
    public PageMode HorsePageMode { get; }
    public HorseBase Horse { get; }

    public HorsePageParam(PageMode mode, HorseBase horse)
    {
        HorsePageMode = mode;
        Horse = horse;
    }
}

public enum PageMode
{
    Read,
    Write
}
