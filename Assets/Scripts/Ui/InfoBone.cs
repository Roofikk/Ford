using TMPro;
using UnityEngine;

public class InfoBone : MonoBehaviour
{
    [SerializeField] private ActionBoneManager _actionManager;

    [Space(5)]
    [SerializeField] private TextMeshProUGUI _nameBone;

    [Space(5)]
    [SerializeField] private TextMeshProUGUI _xTranslationText;
    [SerializeField] private TextMeshProUGUI _yTranslationText;
    [SerializeField] private TextMeshProUGUI _zTranslationText;

    [Space(5)]
    [SerializeField] private TextMeshProUGUI _xRotationText;
    [SerializeField] private TextMeshProUGUI _yRotationText;
    [SerializeField] private TextMeshProUGUI _zRotationText;

    private void Start()
    {
        SetDefault();
        SubscribeAction();
    }

    public void SetDefault()
    {
        _nameBone.text = "Кость не выделена или выделено болеше одной.";

        _xTranslationText.text = "-";
        _yTranslationText.text = "-";
        _zTranslationText.text = "-";

        _xRotationText.text = "-";
        _yRotationText.text = "-";
        _zRotationText.text = "-";
    }

    private void SubscribeAction()
    {
        _actionManager.OnActionComplited += SetInfo;

        foreach (var group in _actionManager.Ford.GroupBones)
        {
            foreach (var bone in group.Bones)
            {
                bone.OnChangedSelection += (isSelected) => 
                { 
                    if (isSelected)
                        SetInfo(); 
                    else
                        SetDefault();
                };
            }
        }
    }

    public void SetInfo()
    {
        if (_actionManager.Ford.SelectingBones.Count > 1)
        {
            SetDefault();
            return;
        }

        BoneObject bone = (BoneObject)_actionManager.Ford.SelectingBones[0];

        _nameBone.text = bone.BoneData.Name;

        SetTranslation(bone.ShiftPosition);
        SetRotation(bone.ShiftRotation);
    }

    private void SetTranslation(Vector3 translation)
    {
        _xTranslationText.text = translation.x.ToString("F2");
        _yTranslationText.text = translation.y.ToString("F2");
        _zTranslationText.text = translation.z.ToString("F2");
    }

    private void SetRotation(Vector3 eulerRotation)
    {
        _xRotationText.text = eulerRotation.x.ToString("F2");
        _yRotationText.text = eulerRotation.y.ToString("F2");
        _zRotationText.text = eulerRotation.z.ToString("F2");
    }

    private void OnDestroy()
    {
        UnsubscribeActions();
    }

    private void UnsubscribeActions()
    {
        _actionManager.OnActionComplited -= SetInfo;

        foreach (var group in _actionManager.Ford.GroupBones)
        {
            foreach (var bone in group.Bones)
            {
                bone.OnChangedSelection += (isSelected) =>
                {
                    if (isSelected)
                        SetInfo();
                    else
                        SetDefault();
                };
            }
        }
    }
}