using UnityEngine;

public class CameraRotateAround : MonoBehaviour
{
    public Camera cam;
    public Transform viewPoint;
    public Transform center;
    public float speedX = 360f;
    public float speedY = 240f;
    public float limitY = 80f;
    public float minDistance = 1.5f;
    public float distanceCam;
    public float speedCam;
    public float scrollSpeed;
    public float speedTranslate;
    public float _distanceCamera;

    private float angle = 0f;
    private Vector3 dSpeedCamera = new Vector3();
    private Vector3 dSpeedTarget = new Vector3();
    private bool _isTranslate = false;
    private float _maxDistance = 500f;
    private float _minDistance = 12f;
    private Vector3 _localPosition;
    internal float _currentYRotation;
    private Vector3 _newTarget;

    private Vector3 _position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    void Start()
    {
        //_localPosition = viewPoint.InverseTransformPoint(_position);
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !EventMouseAction.CursourOnUi)
        {
            float input = Input.GetAxis("Mouse ScrollWheel");
            ScrollCamera(input);
        }

        if (_isTranslate)
        {
            Translate();
        }
    }

    public void TranslateCamera(Vector3 newTarget)
    {
        CalculateNewTarget(newTarget, _distanceCamera);
        _isTranslate = true;
    }

    public void TranslateCamera(Vector3 newTarget, float distanceCamera)
    {
        CalculateNewTarget(newTarget, distanceCamera);
        _isTranslate = true;
    }

    void CalculateNewTarget(Vector3 newTarget, float distanceCamera)
    {
        _newTarget = newTarget;
        Vector3 translateTarger = newTarget - viewPoint.position;
        Vector3 translateCam = newTarget + (new Vector3(3.0f, 0, 4.0f) / 5 * distanceCamera) - transform.position;
        dSpeedTarget = translateTarger / speedTranslate;
        dSpeedCamera = translateCam / speedTranslate;

        angle = -_currentYRotation / speedTranslate;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(2))
        {
            if (cam.orthographic)
                cam.orthographic = false;
            CameraRotation();
            _localPosition = viewPoint.InverseTransformPoint(_position);
        }

        transform.LookAt(viewPoint);
    }

    void ScrollCamera(float input)
    {
        float step;

        if (input > 0)
        {
            step = -scrollSpeed;
        }
        else
        {
            step = scrollSpeed;
        }
    }

    void CameraRotation()
    {
        var mx = Input.GetAxis("Mouse X");
        var my = Input.GetAxis("Mouse Y");

        if (my != 0)
        {
            var tmp = Mathf.Clamp(_currentYRotation - my * speedY * Time.deltaTime, -limitY, limitY);
            if (tmp != _currentYRotation)
            {
                var rot = tmp - _currentYRotation;
                transform.RotateAround(viewPoint.position, transform.right, rot);
                _currentYRotation = tmp;
            }
        }

        if (mx != 0)
        {
            transform.RotateAround(viewPoint.position, Vector3.up, mx * speedX * Time.deltaTime);
        }

        transform.LookAt(viewPoint);
    }

    void Translate()
    {
        if (Vector3.Distance(_newTarget, viewPoint.position) > 0.001f)
        {
            viewPoint.transform.Translate(dSpeedTarget, Space.World);
            Camera.main.transform.Translate(dSpeedCamera, Space.World);
            _currentYRotation += angle;
        }
        else
        {
            _isTranslate = false;
        }
    }
}
