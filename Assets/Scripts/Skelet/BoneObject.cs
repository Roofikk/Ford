using System;
using UnityEditor;
using UnityEngine;
using CustomVector3 = Ford.SaveSystem.Ver2.Data.Vector3;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(MeshCollider))]
public class BoneObject : Bone, IMouseTouchLocker
{
    private MeshCollider _meshCollider;
    private Outline _outline;
    private bool _isMouseOverBone;

    [SerializeField] private BoneData _boneData;
    [SerializeField] string _groupId;

    public event Action<string> OnBoneNameChanged;
    public BoneData BoneData { get { return _boneData; } }
    public Vector3 DefaultPosition { get { return BoneData.Position; } }
    public Vector3 DefaultRotation { get { return BoneData.Rotation; } }

    public Vector3 ShiftPosition
    {
        get
        {
            return transform.position - DefaultPosition;
        }
    }
    
    public Vector3 ShiftRotation
    {
        get
        {
            return transform.rotation.eulerAngles - DefaultRotation;
        }
    }

    public Action<Vector3> OnMoved;
    public Action<Vector3> OnRotated;

    private void Awake()
    {
        _meshCollider = GetComponent<MeshCollider>();
    }

    private void Start()
    {
        if (_outline == null)
            _outline = GetComponent<Outline>();

        _isMouseOverBone = false;
    }

    private void Update()
    {
        if (_isMouseOverBone && UIHandler.IsMouseOnUI)
        {
            OnMouseExit();
        }
    }

    public override void Initiate(GroupBones group)
    {
        base.Initiate(group);

        _groupId = group.Id;

        if (_outline == null)
            _outline = GetComponent<Outline>();

        SetColor(colorSelectionBone.DefaultColor);

        _outline.enabled = IsSelected;
    }

    public void SetPosition(CustomVector3 position)
    {
        transform.position = new Vector3(
            position.x,
            position.y,
            position.z);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = new Vector3(
            position.x,
            position.y,
            position.z);
    }

    public void SetRotation(CustomVector3 rotation)
    {
        transform.rotation = Quaternion.Euler(
            rotation.x,
            rotation.y,
            rotation.z);
    }

    public void SetRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(
            rotation.x,
            rotation.y,
            rotation.z);
    }

    private void OnMouseOver()
    {
        if (!IsSelected)
        {
            _isMouseOverBone = true;
            EnterBone();
        }
    }

    private void OnMouseDown()
    {
        OnClicked?.Invoke();
        SetSelection(!IsSelected);
    }

    private void OnMouseExit()
    {
        _isMouseOverBone = false;
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
    }

    public void Rotate(Vector3 eulerAngle, float angle)
    {
        transform.Rotate(eulerAngle, angle, Space.World);
    }

    public void UpdateName(string name)
    {
        BoneData.Name = name;
    }

    public void LockTouch()
    {
        _meshCollider.enabled = false;
    }

    public void UnlockTouch()
    {
        _meshCollider.enabled = true;
    }

    public void EditBoneName(string name)
    {
        _boneData.Name = name;
        OnBoneNameChanged?.Invoke(name);
    }

#if UNITY_EDITOR
    [ContextMenu("Create File")]
    public void CreateFile()
    {
        _boneData = ScriptableObject.CreateInstance<BoneData>();

        _boneData = new BoneData(name, _groupId, name, transform.position, Quaternion.identity.eulerAngles);
        string path = $"Assets/ScriptableObjects/BoneData/{name}.asset";
        AssetDatabase.CreateAsset(_boneData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
}
