using System;
using UnityEngine;

public class ActionBoneManager : MonoBehaviour
{
    [SerializeField] private Skeleton _ford;

    public Skeleton Ford { get { return _ford; } }
    public StateActionBone CurrentState { get; private set; }
    public StateActionBone PreviousState { get; private set; }

    private Vector3 _startScale = new();
    private Vector3 _distancePlayer = new();

    public StateActionBone StartState;
    public StateActionBone MovementState;
    public StateActionBone RotateState;

    public event Action OnActionComplited;

    private void Start()
    {
        _startScale = transform.localScale;
        _distancePlayer = Camera.main.transform.position - transform.position;

        StartState.Init(this);
        MovementState.Init(this);
        RotateState.Init(this);

        SetState(StartState);
        CurrentState.Display(false);
    }

    public void SetState(StateActionBone state)
    {
        if (CurrentState != null)
            CurrentState.End();

        PreviousState = CurrentState;

        CurrentState = state;
        CurrentState.Enter();
    }

    public void ToggleState()
    {
        if (CurrentState == RotateState)
            SetState(MovementState);
        else
            SetState(RotateState);
    }

    private void Update()
    {
        CurrentState.Tic();
    }

    private void FixedUpdate()
    {
        CurrentState.FixedTic();
    }

    private void LateUpdate()
    {
        Scale();
        CurrentState.LateTic();
    }

    public void StartAction()
    {
        Ford.DeactivateSelection();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ActivateAction(Vector3 direct)
    {
        CurrentState.ActivateAction(direct);
        OnActionComplited?.Invoke();
    }

    public void EndAction()
    {
        Ford.ActivateSelection();
        Cursor.lockState = CursorLockMode.None;
    }

    public void Move(Vector3 moveTo)
    {
        transform.Translate(moveTo);
    }

    public void SetPosition(Vector3 point)
    {
        transform.position = point;
    }

    private void Scale()
    {
        var newScale = Camera.main.transform.position - transform.position;
        float normal = newScale.magnitude / _distancePlayer.magnitude;
        transform.localScale = new Vector3(_startScale.x, _startScale.y, _startScale.z) * normal;
    }

    public void SetPowerSpeedShift(float powerValue)
    {
        CurrentState.SetPowerSpeedShift(powerValue);
    }
}
