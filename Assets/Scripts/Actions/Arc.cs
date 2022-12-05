using System;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class Arc : ActionBone
{
    [SerializeField] private Vector3 _normal;
    [SerializeField] private Vector3 _plane;

    private MeshCollider _collider;

    private Vector3 _startRotation;
    private float _powerMovement;
    private Vector2 _directionOnScreen;

    public Action<Vector3, float> OnRotated;

    public override void Awake()
    {
        base.Awake();

        _collider = GetComponent<MeshCollider>();
    }

    public override void Start()
    {
        base.Start();

        NormalState = new NormalState();
        HighlightedState = new HighlightedState();
        SelectedState = new SelectedState();
        DisabledState = new DisabledState();

        OnRotated += ActionManager.ActivateAction;

        SetState(NormalState);
    }

    private void LateUpdate()
    {
        if (CurrentState != SelectedState)
            LookAtMe();
    }

    public void LookAtMe()
    {
        Vector3 lookAt = Player.Instance.transform.position;
        lookAt = Vector3.Scale(lookAt, _plane);
        lookAt += Vector3.Scale(transform.position, _normal);
        transform.LookAt(lookAt, _normal);
    }

    private void OnMouseEnter()
    {
        if (CurrentState != SelectedState)
            SetState(HighlightedState);
    }

    private void OnMouseExit()
    {
        if (CurrentState != SelectedState)
            SetState(NormalState);
    }

    private void OnMouseDown()
    {
        SetState(SelectedState);
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            _startRotation = Vector3.Scale(hit.point, _plane) + Vector3.Scale(transform.position, _normal);
            Vector3 v = _startRotation - transform.position;
            Vector3 vectorMove = Vector3.Cross(_normal, v); 
            
            Vector2 directMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            Vector2 vStart = Camera.main.WorldToScreenPoint(_startRotation);
            Vector2 vEnd = Camera.main.WorldToScreenPoint(_startRotation + vectorMove);
            _directionOnScreen = (vEnd - vStart).normalized;
        }

        OnStartDrag?.Invoke();
    }

    private void OnMouseUp()
    {
        SetState(NormalState);
        OnEndDrag?.Invoke();
    }

    private void OnMouseDrag()
    {
        DragTorus();
    }

    private void DragTorus()
    {
        Vector2 directMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        float angle = Vector2.Angle(directMouse, _directionOnScreen);
        _powerMovement += Mathf.Cos((angle * Mathf.PI) / 180f) * directMouse.magnitude;

        if (Mathf.Abs(_powerMovement * SensetivityDrag) > 1)
        {
            transform.rotation *= Quaternion.Euler(_powerMovement > 0 ? Vector3.up : -Vector3.up);

            OnRotated?.Invoke(_powerMovement > 0 ? _normal : - _normal, SpeedShift);
            _powerMovement = 0f;
        }
    }

    public override void Enable()
    {
        _collider.enabled = true;
    }

    public override void Disable()
    {
        _collider.enabled = false;
    }
}
