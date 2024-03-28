using Ford.SaveSystem.Ver2;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChildHistoryElement : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _headerText;
    [SerializeField] protected TextMeshProUGUI _descriptionText;
    [SerializeField] protected Toggle _toggle;

    protected HistoryElement _parent;
    public ActionType ActionType { get; protected set; }
    public bool ToggleValue => _toggle.isOn;

    public virtual ChildHistoryElement Initiate(HistoryElement parent, StorageAction<IStorageAction> action,
        string header, ToggleGroup toggleGroup)
    {
        _toggle.onValueChanged.AddListener((value) =>
        {
            if (CanToggleOff())
            {
                ToastMessage.Show("¬ы не можете убрать единственное сохранение");
                _toggle.isOn = true;
            }
        });

        _headerText.text = header;
        _descriptionText.text = action.Data.ActionDescription;
        _toggle.group = toggleGroup;

        _parent = parent;

        return this;
    }

    public void SwitchToggle(bool value)
    {
        _toggle.isOn = value;
    }

    private bool CanToggleOff()
    {
        if (_parent == null)
        {
            return true;
        }

        if (!_parent.ToggleValue)
        {
            return true;
        }

        var select = _parent.ChildHistories.Where(sh => sh.ActionType == ActionType.CreateSave && sh.ToggleValue);

        if (select.Count() < 1)
        {
            return false;
        }

        return true;
    }
}
