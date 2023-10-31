using System.Collections.Generic;
using UnityEngine;

public abstract class StateActionBone : MonoBehaviour
{
    public ActionBoneManager ActionManager { get; private set; }

    [SerializeField] private List<ActionBone> _actionList = new List<ActionBone>();

    [Header("Settings drag")]
    [SerializeField] private float _minSpeedShift = 0.1f;
    [SerializeField] private float _maxSpeedShift = 1f;
    [SerializeField] private float _speedShift;

    public float MinSpeedShift { get { return _minSpeedShift; } }
    public float MaxSpeedShift { get { return _maxSpeedShift; } }
    public float SpeedShift { get { return _speedShift; } }

    public IReadOnlyCollection<ActionBone> ActionList => _actionList.AsReadOnly();

    public virtual void Init(ActionBoneManager stateMachine)
    {
        ActionManager = stateMachine;

        foreach (var action in ActionList)
        {
            action.OnStartDrag += OnStartAction;
            action.OnStartDrag += () => { SetInteractive(true); };

            action.OnEndDrag += OnEndAction;
            action.OnEndDrag += () => { SetInteractive(false); };
        }

        Display(false);
    }

    public void Display(bool isOn)
    {
        gameObject.SetActive(isOn);
    }

    public virtual void Enter()
    {
        Display(true);
    }

    public virtual void End()
    {
        Display(false);
    }

    public virtual void Tic()
    {
    }

    public virtual void FixedTic()
    {
    }

    public virtual void LateTic()
    {
        if (Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            SetInteractive(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftCommand) || Input.GetKeyUp(KeyCode.LeftControl))
        {
            SetInteractive(false);
        }
    }

    public virtual void OnStartAction()
    {
        ActionManager.StartAction();
    }

    public abstract void ActivateAction(Vector3 v);

    public virtual void OnEndAction()
    {
        ActionManager.EndAction();
    }

    public virtual void SetInteractive(bool isOn)
    {
        foreach (var action in ActionList)
        {
            if (isOn)
            {
                if (action.CurrentState != action.SelectedState)
                    action.SetState(action.DisabledState);
            }
            else
                action.SetState(action.NormalState);
        }
    }

    public void SetPowerSpeedShift(float value)
    {
        _speedShift = value;
    }
}
