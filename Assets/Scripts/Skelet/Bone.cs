using System;
using UnityEngine;

public abstract class Bone : MonoBehaviour
{
    [SerializeField] protected ColorSelectionBone colorSelectionBone;

    public bool IsSelected { get; protected set; }

    public Action OnClicked;
    public Action OnEntered;
    public Action OnExit;
    public Action<bool> OnChangedSelection;

    public virtual void Initiate(GroupBones group)
    {
        IsSelected = false;
    }

    public virtual void SetSelection(bool selected)
    {
        IsSelected = selected;

        OnChangedSelection?.Invoke(IsSelected);
    }

    protected abstract void SetColor(Color color);
}