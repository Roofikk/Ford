using Ford.SaveSystem.Ver2;
using System;

public class HistoryPage : Page
{
    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);


    }

    public override void Open<T>(T param, int popUpLevel = 0)
    {
        base.Open(param, popUpLevel);

        if (param is not HistoryPageParam historyParam)
        {
            throw new Exception($"Param should be {nameof(HistoryPageParam)}");
        }


    }

    public override void Close()
    {
        base.Close();


    }
}

public class HistoryPageParam
{
    public StorageHistory History { get; }

    public HistoryPageParam(StorageHistory history)
    {
        History = history;
    }
}