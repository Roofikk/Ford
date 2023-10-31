using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleActionBoneUI : MonoBehaviour
{
    [SerializeField] ActionBoneManager _actionSystem;
    [SerializeField] SettingPowerAction _settingPowerAction;

    [Header("Спрайты действия")]
    [SerializeField] private Image _imageMovement;
    [SerializeField] private Image _imageRotation;

    private Toggle _toggle;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = false;
    }

    private void Start()
    {
        if (_actionSystem.StartState == _actionSystem.MovementState)
        {
            _imageMovement.gameObject.SetActive(true);
            _imageRotation.gameObject.SetActive(false);
        }
        else
        {
            _imageMovement.gameObject.SetActive(false);
            _imageRotation.gameObject.SetActive(true);
        }

        _settingPowerAction.OnPowerValueChanged += _actionSystem.SetPowerSpeedShift;
        _settingPowerAction.SetSetting(_actionSystem.CurrentState.MinSpeedShift, _actionSystem.CurrentState.MaxSpeedShift, _actionSystem.CurrentState.SpeedShift);

        _toggle.onValueChanged.AddListener(ToggleAction);
    }

    private void ToggleAction(bool value)
    {
        _actionSystem.ToggleState();

        _settingPowerAction.SetSetting(
            _actionSystem.CurrentState.MinSpeedShift,
            _actionSystem.CurrentState.MaxSpeedShift,
            _actionSystem.CurrentState.SpeedShift);

        _imageMovement.gameObject.SetActive(!_imageMovement.IsActive());
        _imageRotation.gameObject.SetActive(!_imageRotation.IsActive());
    }
}
