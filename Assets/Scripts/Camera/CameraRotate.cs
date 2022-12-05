using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private bool _inverse;

    [Range(1f, 4f)]
    [SerializeField] private float _sensetivity = 200f;
    [SerializeField] private float _smoothing = 2f;
    private Vector2 _currentLook;
    private Vector2 smoothedVelocity;

    private void Start()
    {
        _currentLook = new(_player.transform.localRotation.eulerAngles.x, _player.transform.localRotation.eulerAngles.y);
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse2) && !UIHandler.IsMouseOnUI)
        {
            Look();
        }
    }

    private void Look()
    {
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        inputVector = Vector2.Scale(inputVector, new Vector2(_sensetivity * _smoothing, _sensetivity * _smoothing));

        smoothedVelocity.x = Mathf.Lerp(smoothedVelocity.x, inputVector.x, 1f / _smoothing);
        smoothedVelocity.y = Mathf.Lerp(smoothedVelocity.y, inputVector.y, 1f / _smoothing);

        _currentLook.x += _inverse ? smoothedVelocity.y : -smoothedVelocity.y;
        _currentLook.y += _inverse ? -smoothedVelocity.x : smoothedVelocity.x;

        _currentLook.x = Mathf.Clamp(_currentLook.x, -90f, 90f);

        _player.transform.localRotation = Quaternion.Euler(_currentLook.x, _currentLook.y, 0f);
    }
}
