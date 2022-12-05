using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BoneUI : Bone, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IMouseLockerUI
{
    [SerializeField] private TextMeshProUGUI _boneNameText;
    private Image _image;

    private BoneObject _boneObj;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Initiate(BoneObject bone)
    {
        Initiate(bone.BoneData.Name);
        
        _boneNameText.text = bone.name;
        SetColor(colorSelectionBone.DefaultColor);

        _boneObj = (BoneObject)bone;
        _boneObj.OnChangedSelection += SetSelection;
        _boneObj.OnEntered += Highlight;
        _boneObj.OnExit += ExitPoint;

        OnClicked += () =>
        {
            _boneObj.OnClicked?.Invoke();
        };
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
}
