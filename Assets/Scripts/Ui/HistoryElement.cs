using Ford.SaveSystem.Ver2;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryElement : MonoBehaviour
{
    [SerializeField] private HistoryElement _subhistoryElementPrefab;

    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private TextMeshProUGUI _headerHistoryText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private Transform _subhistoryContainer;

    private List<HistoryElement> _subHistories = new();
    private HistoryElement _parent;
    public ActionType ActionType { get; private set; }
    public bool ToggleValue => _toggle.isOn;
    public IReadOnlyCollection <HistoryElement> SubHistories => _subHistories;

    private Dictionary<ActionType, string> _actionTypeToText = new Dictionary<ActionType, string>()
    {
        {ActionType.CreateHorse, "Создание лошади"},
        {ActionType.UpdateHorse, "Изменение лошади"},
        {ActionType.DeleteHorse, "Удаление лошади"},
        {ActionType.CreateSave, "Создание сохранения"},
        {ActionType.UpdateSave, "Изменение сохранения"},
        {ActionType.DeleteSave, "Удаление сохранения"},
    };

    public HistoryElement Initiate(HistoryElement parent, StorageAction<IStorageAction> action, 
        ICollection<StorageAction<IStorageAction>> subactions)
    {
        _toggle.onValueChanged.AddListener((value) =>
        {
            if (CanToggleOff())
            {
                ToastMessage.Show("Вы не можете убрать единственное сохранение");
                _toggle.isOn = true;
            }
        });

        _headerHistoryText.text = _actionTypeToText[action.ActionType];
        _descriptionText.text = action.Data.ActionDescription;
        _parent = parent;

        // group history
        // don't forget about contains one or more save in horse

        foreach (var sub in subactions)
        {
            var subhistory = Instantiate(_subhistoryElementPrefab, _subhistoryContainer)
                .Initiate(parent, sub, Array.Empty<StorageAction<IStorageAction>>());

            _subHistories.Add(subhistory);
        }

        return this;
    }

    private bool CanToggleOff()
    {
        // Checking for at least one available save
        if (_parent != null)
        {
            if (_parent.ActionType == ActionType.CreateHorse)
            {
                var select = _parent.SubHistories.Where(sh => sh.ActionType == ActionType.CreateSave && sh.ToggleValue);

                if (select.Count() < 1)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
