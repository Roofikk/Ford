using Ford.SaveSystem.Ver2;
using System.Collections.Generic;
using UnityEngine;

public class HistoryElement : ChildHistoryElement
{
    [SerializeField] private ChildHistoryElement _subHistoryElementPrefab;
    [SerializeField] private ToggleThreeState _stateToggle;

    [SerializeField] private Transform _subHistoryContainer;
    [SerializeField] private Shutter _shutter;

    private List<ChildHistoryElement> _childHistories = new();
    public IReadOnlyCollection <ChildHistoryElement> ChildHistories => _childHistories;
    public ToggleState ToggleState => _stateToggle.State;

    public readonly Dictionary<ActionType, string> ActionTypeToText = new Dictionary<ActionType, string>()
    {
        {ActionType.CreateHorse, "�������� ������"},
        {ActionType.UpdateHorse, "��������� ������"},
        {ActionType.DeleteHorse, "�������� ������"},
        {ActionType.CreateSave, "�������� ����������"},
        {ActionType.UpdateSave, "��������� ����������"},
        {ActionType.DeleteSave, "�������� ����������"},
    };

    public HistoryElement Initiate(HistoryElement parent, StorageAction<IStorageAction> action, 
        ICollection<StorageAction<IStorageAction>> subActions)
    {
        _storageAction = action;
        _stateToggle.SetState(ToggleState.On);

        _headerText.text = ActionTypeToText[action.ActionType];
        _descriptionText.text = action.Data.ActionDescription;
        _parent = parent;

        foreach (var sub in subActions)
        {
            var subhistory = Instantiate(_subHistoryElementPrefab, _subHistoryContainer)
                .Initiate(this, sub, ActionTypeToText[sub.ActionType]);

            _childHistories.Add(subhistory);
        }

        _stateToggle.OnStateChanged += (value) =>
        {
            if (value == ToggleState.On)
            {
                foreach (var child in ChildHistories)
                {
                    child.SwitchToggle(true);
                }
            }

            if (value == ToggleState.Off)
            {
                foreach (var child in ChildHistories)
                {
                    child.SwitchToggle(false);
                }
            }
        };

        _shutter.Initiate((value) =>
        {
            foreach (var child in ChildHistories)
            {
                child.gameObject.SetActive(value);
            }
        });

        return this;
    }

    public void ChangeState(ToggleState state)
    {
        _stateToggle.SetState(state);
    }
}
