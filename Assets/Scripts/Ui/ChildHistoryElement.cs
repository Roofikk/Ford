using Ford.SaveSystem.Ver2;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChildHistoryElement : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _headerText;
    [SerializeField] protected TextMeshProUGUI _descriptionText;
    [SerializeField] private Toggle _toggle;

    protected HistoryElement _parent;
    private bool ToggleValue { get => _toggle.isOn; }
    protected StorageAction<IStorageAction> _storageAction;
    public IStorageAction ActionData => _storageAction.Data;
    public ActionType ActionType => _storageAction.ActionType;

    public virtual ChildHistoryElement Initiate(HistoryElement parent, StorageAction<IStorageAction> action,
        string header)
    {
        _storageAction = action;
        _toggle.isOn = true;

        _headerText.text = header;
        _descriptionText.text = action.Data.ActionDescription;

        _parent = parent;

        if (_parent.ActionType == ActionType.DeleteHorse)
        {
            _toggle.interactable = false;
        }

        _toggle.onValueChanged.AddListener((value) =>
        {
            ChangeParentToggleState();
        });

        return this;
    }

    public void SwitchToggle(bool value)
    {
        _toggle.isOn = value;
    }

    private void ChangeParentToggleState()
    {
        switch (_parent.ActionType)
        {
            case ActionType.CreateHorse:
                var activeToggles = _parent.ChildHistories.Where(x => x.ToggleValue);

                if (activeToggles.Count() == _parent.ChildHistories.Count)
                {
                    _parent.ChangeState(ToggleState.On);
                }
                else if (activeToggles.Count() == 0)
                {
                    _parent.ChangeState(ToggleState.Off);
                }
                else
                {
                    _parent.ChangeState(ToggleState.Partial);
                }

                break;
            case ActionType.UpdateHorse:

                break;
            case ActionType.DeleteHorse:

                break;
        }
    }
}
