using Ford.SaveSystem.Ver2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryElement : ChildHistoryElement
{
    [SerializeField] private ChildHistoryElement _subHistoryElementPrefab;

    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private Transform _subHistoryContainer;
    [SerializeField] private Toggle _showHideToggle;
    [SerializeField] private Texture _showToggleImage;
    [SerializeField] private Texture _hideToggleImage;

    private List<ChildHistoryElement> _childHistories = new();
    public IReadOnlyCollection <ChildHistoryElement> ChildHistories => _childHistories;

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
        ICollection<StorageAction<IStorageAction>> subActions, ToggleGroup toggleGroup)
    {
        _toggle.onValueChanged.AddListener((value) =>
        {
            foreach (var child in ChildHistories)
            {
                child.SwitchToggle(value);
            }
        });

        _showHideToggle.onValueChanged.AddListener((value) =>
        {
            _showHideToggle.graphic.material.mainTexture = value ? _showToggleImage : _hideToggleImage;
            
            foreach (var child in ChildHistories)
            {
                child.gameObject.SetActive(value);
            }
        });

        _toggle.group = toggleGroup;
        _headerText.text = ActionTypeToText[action.ActionType];
        _descriptionText.text = action.Data.ActionDescription;
        _parent = parent;

        foreach (var sub in subActions)
        {
            var subhistory = Instantiate(_subHistoryElementPrefab, _subHistoryContainer)
                .Initiate(this, sub, ActionTypeToText[sub.ActionType], _toggleGroup);

            _childHistories.Add(subhistory);
        }

        return this;
    }
}
