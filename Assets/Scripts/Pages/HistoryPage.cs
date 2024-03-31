using Ford.SaveSystem;
using Ford.SaveSystem.Ver2;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HistoryPage : Page
{
    [SerializeField] private HistoryElement _historyElementPrefab;

    [Space(10)]
    [Header("UI Elements")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _declineButton;

    private StorageHistory _storageHistory;
    public StorageHistory StorageHistory => _storageHistory;

    private void Start()
    {
        _applyButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenWarningPage(new WarningData(
                "��������������",
                "�� �������, ��� ������ ��������� ������� ����������?\n������� ����������� ��������� ����� ��� ����������",
                Apply));
        });

        _declineButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenWarningPage(new WarningData(
                "��������������",
                "�� ����� �������, ��� ������ �������� �������?\n" +
                "��� ����������, ����������� �� ����� ������������ ����������, ����� ������� ��������",
                Decline));
        });
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        // don't forget about grouping history
    }

    public override void Open<T>(T param, int popUpLevel = 0)
    {
        base.Open(param, popUpLevel);

        if (param is not HistoryPageParam historyParam)
        {
            throw new Exception($"Param should be {nameof(HistoryPageParam)}");
        }

        // don't forget about grouping history
        _storageHistory = historyParam.History;
        var groupedHistoryByHorseId = _storageHistory.History
            .GroupBy(x => x.Data.HorseId);
        
        foreach (var group in groupedHistoryByHorseId)
        {
            var horseAction = group.SingleOrDefault(h => h.ActionType == ActionType.CreateHorse || 
                h.ActionType == ActionType.UpdateHorse || h.ActionType == ActionType.DeleteHorse);

            var saveActions = group.Where(h => h != horseAction);

            var historyElement = Instantiate(_historyElementPrefab, _scrollRect.content.transform)
                .Initiate(null, horseAction, saveActions.ToList());
        }
    }

    public override void Close()
    {
        base.Close();
    }

    private void Apply()
    {
        var storage = new StorageSystem();
        PageManager.Instance.DisplayLoadingPage(true, 8);
        storage.ApplyTransition().RunOnMainThread((result) =>
        {
            if (result)
            {
                ToastMessage.Show("�� ������� ��������� ������ � ������������ � ����. � ������������!");
            }
            else
            {
                storage.DeclineTransition();
                ToastMessage.Show("��������� ������ ��� �������.\n��������� ���������� ����������� � ��������� ���� ��������� � ����������� ����������.");
            }

            PageManager.Instance.DisplayLoadingPage(false);
        });
    }

    private void Decline()
    {
        var storage = new StorageSystem();
        PageManager.Instance.DisplayLoadingPage(true, 8);
        storage.RawApplyTransition().RunOnMainThread((result) =>
        {
            if (result)
            {
                ToastMessage.Show("�� ������� ������������ � ����.\n� ������������!");
            }
            else
            {
                storage.DeclineTransition();
                ToastMessage.Show("��������� ������ ��� �������.\n��������� ���������� ����������� � ��������� ���� ��������� � ����������� ����������.");
            }

            PageManager.Instance.DisplayLoadingPage(false);
        });
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