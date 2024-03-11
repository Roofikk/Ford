using Ford.SaveSystem;

public class HorsePageParam
{
    public HorsePageMode HorsePageMode { get; }
    public HorseBase Horse { get; }

    public HorsePageParam(HorsePageMode mode, HorseBase horse)
    {
        HorsePageMode = mode;
        Horse = horse;
    }
}

public enum HorsePageMode
{
    Read,
    Write
}
