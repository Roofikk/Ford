using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollRect : ScrollRect
{
    public LoadIcon LoadIconPrefab;
    public float DistanceForPagination = 120f;
    public LockCustomScroll scrollLocker;

    private LoadIcon _loadIcon;

    public event Action OnRefreshFocused;
    public event Action OnUploadFocused;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        onValueChanged.RemoveListener(FillAmount);
        onValueChanged.RemoveListener(CheckUploadFocus);
    }

    public void SetLoadIcon()
    {
        if (_loadIcon == null)
        {
            _loadIcon = Instantiate(LoadIconPrefab, content);
            _loadIcon.transform.SetSiblingIndex(0);
            onValueChanged.AddListener(FillAmount);
        }
        else
        {
            _loadIcon.transform.SetSiblingIndex(0);
        }
    }

    public void DisableRefreshElement()
    {
        _loadIcon.EnableLayout(false);
        _loadIcon.EnableFreeRotate(false);
    }

    public void ActivatePagination(bool enable)
    {
        if (enable)
        {
            onValueChanged.AddListener(CheckUploadFocus);
        }
        else
        {
            onValueChanged.RemoveListener(CheckUploadFocus);
        }
    }

    private void FillAmount(Vector2 vector2)
    {
        if (content.anchoredPosition.y < 0)
        {
            float percentage = MathF.Abs(content.anchoredPosition.y) / _loadIcon.MinHeight;
            _loadIcon.FillIcon(percentage);
        }
    }

    private void CheckRefreshFocus(Vector2 vector2)
    {
        if (content.anchoredPosition.y < 0)
        {
            float percentage = MathF.Abs(content.anchoredPosition.y) / _loadIcon.MinHeight;

            if (percentage > 1f)
            {
                _loadIcon.EnableLayout(true);
                _loadIcon.EnableFreeRotate(true);
                OnRefreshFocused?.Invoke();
            }
        }
    }

    private void CheckUploadFocus(Vector2 vector2)
    {
        if (content.rect.height - viewport.rect.height < 0)
        {
            return;
        }

        var delta = DistanceForPagination / (content.rect.height - viewport.rect.height);

        if (vector2.y < delta && !scrollLocker.IsLocked)
        {
            scrollLocker.Lock();
            OnUploadFocused?.Invoke();
        }
    }

    public void LockScroll(bool enable)
    {
        if (enable)
        {
            scrollLocker.Lock();
        }
        else
        {
            scrollLocker.Unlock();
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if (_loadIcon != null)
        {
            CheckRefreshFocus(normalizedPosition);
        }
    }
}
