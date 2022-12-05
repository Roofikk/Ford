using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(MeshCollider))]

public class BoneObject : Bone, IMouseTouchLocker
{
    public BoneData BoneData;
    private MeshCollider _meshCollider;
    private Outline _outline;
    private bool _isMouseOverBone;

    public Vector3 DefaultPosition { get; private set; }
    public Vector3 DefaultRotation { get; private set; }
    public Vector3 ShiftPosition { get; private set; }
    public Vector3 ShiftRotation { get; private set; }

    public Action<Vector3> OnMoved;
    public Action<Vector3> OnRotated;

    private void Awake()
    {
        DefaultPosition = transform.position;
        DefaultRotation = transform.rotation.eulerAngles;
        _meshCollider = GetComponent<MeshCollider>();
    }

    private void Start()
    {
        if (_outline == null)
            _outline = GetComponent<Outline>();
    }

    private void Update()
    {
        if (_isMouseOverBone && UIHandler.IsMouseOnUI)
        {
            OnMouseExit();
        }
    }

    public override void Initiate(string name = "Default")
    {
        base.Initiate(name);

        if (_outline == null)
            _outline = GetComponent<Outline>();

        SetColor(colorSelectionBone.DefaultColor);

        _outline.enabled = IsSelected;
        _outline.OutlineWidth = 8f;
    }

    public void ReturnToDefault()
    {
        transform.position = DefaultPosition;
        transform.rotation = Quaternion.identity;
    }

    private void OnMouseOver()
    {
        if (!UIHandler.IsMouseOnUI)
            if (!IsSelected)
            {
                _isMouseOverBone = true;
                EnterBone();
            }
    }

    private void OnMouseDown()
    {
        if (!UIHandler.IsMouseOnUI)
        {
            OnClicked?.Invoke();
            SetSelection(!IsSelected);
        }
    }

    private void OnMouseExit()
    {
        _isMouseOverBone = true;
        ExitBone();
    }

    public override void SetSelection(bool select)
    {
        base.SetSelection(select);

        SetColor(IsSelected ? colorSelectionBone.SelectionColor : colorSelectionBone.DefaultColor);
    }

    public void EnterBone()
    {
        if (!IsSelected)
            SetColor(colorSelectionBone.HighlightColor);

        OnEntered?.Invoke();
    }

    public void ExitBone()
    {
        if (!IsSelected)
            SetColor(colorSelectionBone.DefaultColor);

        OnExit?.Invoke();
    }

    protected override void SetColor(Color color)
    {
        if (color == colorSelectionBone.DefaultColor)
        {
            _outline.enabled = false;
            return;
        }

        _outline.enabled = true;
        _outline.OutlineColor = color;
    }

    public void Translate(Vector3 direction, float speedShift)
    {
        transform.Translate(direction * speedShift, Space.World);
        ShiftPosition += direction * speedShift;
    }

    public void Rotate(Vector3 eulerAngle, float angle)
    {
        transform.Rotate(eulerAngle, angle, Space.World);
        ShiftRotation = transform.rotation.eulerAngles;
    }

    public void LockTouch()
    {
        _meshCollider.enabled = false;
        
    }

    public void UnlockTouch()
    {
        _meshCollider.enabled = true;
    }

    [ContextMenu("Create File")]
    public void CreateFile()
    {
        BoneData = ScriptableObject.CreateInstance<BoneData>();

        BoneData = new BoneData(name, name, transform.position, Quaternion.identity.eulerAngles);
        string path = $"Assets/ScriptableObjects/BoneData/{name}.asset";
        AssetDatabase.CreateAsset(BoneData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
