using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BoneUI : Bone, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IMouseLockerUI
{
    [SerializeField] private TextMeshProUGUI _boneNameText;
    [SerializeField] private Button _editDevButton;

    private Image _image;
    private BoneObject _boneObj;

    public string Id { get { return _boneObj.BoneData.Id; } }
    public string Name { get { return _boneObj.BoneData.Name; } }

    private void Awake()
    {
        _image = GetComponent<Image>();

        if (_editDevButton != null)
        {
            _editDevButton.onClick.AddListener(() => { PageManager.Instance.OpenPage(UiManager.Instance.EditBonePage, _boneObj); });
        }
    }

    private void OnDestroy()
    {
        if (_editDevButton != null)
        {
            _editDevButton.onClick.RemoveAllListeners();
        }
    }

    public void Initiate(BoneObject bone, GroupBones group)
    {
        base.Initiate(group);

        _boneObj = bone;
        _boneNameText.text = _boneObj.BoneData.Name;

        SetColor(colorSelectionBone.DefaultColor);
        _boneObj.OnChangedSelection += SetSelection;
        _boneObj.OnEntered += Highlight;
        _boneObj.OnExit += ExitPoint;

        OnClicked += () =>
        {
            _boneObj.OnClicked?.Invoke();
        };

        _boneObj.OnBoneNameChanged += EditName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
        _boneObj.SetSelection(!IsSelected);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _boneObj.EnterBone();
        Highlight();
        OnEntered?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _boneObj.ExitBone();
        ExitPoint();
        OnExit?.Invoke();
    }

    private void Highlight()
    {
        if (!IsSelected)
            SetColor(colorSelectionBone.HighlightColor);
    }

    private void ExitPoint()
    {
        if (!IsSelected)
            SetColor(colorSelectionBone.DefaultColor);
    }

    public override void SetSelection(bool selected)
    {
        base.SetSelection(selected);

        SetColor(selected ? colorSelectionBone.SelectionColor : colorSelectionBone.DefaultColor);
    }

    protected override void SetColor(Color color)
    {
        _image.color = color;
    }

    public void Lock()
    {
        _image.raycastTarget = false;
        _boneNameText.raycastTarget = false;
    }

    public void Unlock()
    {
        _image.raycastTarget = true;
        _boneNameText.raycastTarget = true;
    }

    public void EditName(string newName)
    {
        _boneNameText.text = newName;
    }
}
