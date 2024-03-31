using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleThreeState : MonoBehaviour
{
    [SerializeField] private Image _checkMark;

    [Space(10)]
    [SerializeField] private Sprite _stateOnImage;
    [SerializeField] private Sprite _stateOffImage;
    [SerializeField] private Sprite _statePartialImage;
    private Toggle _toggle;

    public ToggleState State { get; private set; }
    public event Action<ToggleState> OnStateChanged;

    private void Awake()
    {
        _toggle ??= GetComponent<Toggle>();

        _toggle.onValueChanged.AddListener(value =>
        {
            SetState(value ? ToggleState.On : ToggleState.Off);
        });
    }

    public void SetState(ToggleState state)
    {
        if (state == State)
        {
            return;
        }

        State = state;
        
        switch (State)
        {
            case ToggleState.Off:
                _checkMark.color = Color.white;
                _toggle.isOn = false;
                _checkMark.sprite = _stateOffImage;
                break;
            case ToggleState.On:
                _checkMark.color = Color.white;
                _toggle.isOn = true;
                _checkMark.sprite = _stateOnImage;
                break;
            case ToggleState.Partial:
                _checkMark.color = Color.black;
                _checkMark.sprite = _statePartialImage;
                break;
        }

        OnStateChanged?.Invoke(state);
    }
}

public enum ToggleState
{
    Off,
    On,
    Partial
}