using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Settings _settings;

    [Range(50f, 90f)][SerializeField] private float _yMinLimit = 88f;
    [Range(50f, 90f)][SerializeField] private float _yMaxLimit = 88f;

    private Vector2 _rotation = Vector2.zero;

    private void Start()
    {
        if (!_settings.InverseRotationCamera)
            _rotation = new(-_player.transform.eulerAngles.y, -_player.transform.eulerAngles.x);
        else
            _rotation = new(_player.transform.eulerAngles.y, _player.transform.eulerAngles.x);

        _settings.OnInverseRotationToggleChanged += InverseRotation;
    }

    private void OnDestroy()
    {
        _settings.OnInverseRotationToggleChanged -= InverseRotation;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse2) && !UIHandler.IsMouseOnUI)
        {
            Look();
        }
    }

    private void Look()
    {
        _rotation += new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * _settings.SensetivityRotationCamera;

        _rotation.y = Mathf.Clamp(_rotation.y, -_yMinLimit, _yMaxLimit);

        var xQuat = Quaternion.AngleAxis(_rotation.x, _settings.InverseRotationCamera ? -Vector3.up : Vector3.up);
        var yQuat = Quaternion.AngleAxis(_rotation.y, _settings.InverseRotationCamera ? -Vector3.left : Vector3.left);

        _player.transform.localRotation = xQuat * yQuat;
    }

    public void InverseRotation(bool value)
    {
        _rotation *= -1;
    }
}
