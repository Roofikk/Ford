using Ford.SaveSystem.Ver2;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HistoryPage : Page
{
    [SerializeField] private HistoryElement _historyElementPrefab;

    [Space(10)]
    [Header("UI Elements")]
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _declineButton;

    private StorageHistory _history;
    public StorageHistory History => _history;

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