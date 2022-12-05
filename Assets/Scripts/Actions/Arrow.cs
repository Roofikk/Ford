using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider))]
public class Arrow : ActionBone
{
    private float _powerMovement = 0;
    private BoxCollider _boxCollider;

    public Action<Vector3, float> OnTranslated;

    public override void Awake()
    {
        base.Awake();

        _boxCollider = GetComponent<BoxCollider>();
    }

    public override void Start()
    {
        base.Start();

        NormalState = new NormalState();
        HighlightedState = new HighlightedState();
        SelectedState = new SelectedState();
        DisabledState = new DisabledState();

        OnTranslated += ActionManager.ActivateAction;

        SetState(NormalState);
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
        Vector3 mousePosition = Input.mousePosition;

        OnStartDrag?.Invoke();
    }

    private void OnMouseDrag()
    {
        DragArrow();
    }

    private void OnMouseUp()
    {
        SetState(NormalState);

        OnEndDrag?.Invoke();
    }

    private void DragArrow()
    {
        float vx = Input.GetAxis("Mouse X");
        float vy = Input.GetAxis("Mouse Y");
        Vector2 directMouse = new Vector2(vx, vy);

        Vector3 vEnd = Camera.main.WorldToScreenPoint(transform.position + Direction);
        Vector3 vStart = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 directionScreen = (vEnd - vStart).normalized;
        float angle = Vector2.Angle(directMouse, directionScreen);
        _powerMovement += Mathf.Cos((angle * Mathf.PI) / 180f) * directMouse.magnitude;

        if (Mathf.Abs(_powerMovement * SensetivityDrag) > 1)
        {
            Vector3 direction = (_powerMovement > 0 ? Direction : -Direction);

            OnTranslated?.Invoke(direction, SpeedShift);
            _powerMovement = 0f;
        }
    }

    public override void Enable()
    {
        _boxCollider.enabled = true;
    }

    public override void Disable()
    {
        _boxCollider.enabled = false;
    }
}
