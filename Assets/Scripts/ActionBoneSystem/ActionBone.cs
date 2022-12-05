using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Outline))]
public abstract class ActionBone : MonoBehaviour, IMouseTouchLocker
{
    [SerializeField] private ActionBoneManager _actionManager;
    public ActionBoneManager ActionManager { get { return _actionManager; } }

    [SerializeField] private Vector3 _direction;
    public Vector3 Direction { get { return _direction; } }

    [Header("Settings drag")]

    [SerializeField] private float _sensetivityDrag = 2f;
    public float SensetivityDrag { get { return _sensetivityDrag; } }

    [SerializeField] private float _speedShift;
    public float SpeedShift { get { return _speedShift; } }

    [Header("Colors")]
    [SerializeField] private ColorAction _colors;
    public ColorAction Colors { get { return _colors; } }

    public ActionStateBase CurrentState { get; set; }
    public ActionStateBase NormalState { get; set; }
    public ActionStateBase HighlightedState { get; set; }
    public ActionStateBase SelectedState { get; set; }
    public ActionStateBase DisabledState { get; set; }

    private MeshRenderer _meshRenderer;
    private Outline _outline;

    public MeshRenderer Mesh { get { return _meshRenderer; } }
    public Outline Outline { get { return _outline; } }

    public Action OnStartDrag;
    public Action OnEndDrag;

    public virtual void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _outline = GetComponent<Outline>();
    }

    public virtual void Start()
    {
        OnStartDrag += UIHandler.LockMouseUI;
        OnEndDrag += UIHandler.UnlockMouseUI;
    }

    public virtual void OnDestroy()
    {
        OnStartDrag -= UIHandler.LockMouseUI;
        OnEndDrag -= UIHandler.UnlockMouseUI;
    }

    public void SetState(ActionStateBase state)
    {
        if (CurrentState != null)
            CurrentState.Exit(this);

        CurrentState = state;
        CurrentState.Enter(this);
    }

    public virtual void SetColor(Color color)
    {
        Mesh.material.color = color;
        _outline.OutlineColor = color;
    }

    public abstract void Enable();

    public abstract void Disable();

    public void LockTouch()
    {
        Disable();
    }

    public void UnlockTouch()
    {
        Enable();
    }
}
