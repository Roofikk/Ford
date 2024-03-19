using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollRect : ScrollRect
{
    public LoadIcon LoadIconPrefab;

    private RectTransform _rectTransform;
    private LoadIcon _loadIcon;
    public event Action OnUpdateFocused;

    protected override void Start()
    {
        base.Start();

        _rectTransform = GetComponent<RectTransform>();
        onValueChanged.AddListener(FillAmount);
    }

    public void SetLoadIcon()
    {
        if (_loadIcon == null)
        {
            _loadIcon = Instantiate(LoadIconPrefab, content);
        }
        else
        {
            _loadIcon.transform.SetSiblingIndex(content.childCount - 1);
        }
    }

    private void FillAmount(Vector2 vector2)
    {
        var percentage = _loadIcon.MinHeight / (content.rect.height - viewport.rect.height);

        if (vector2.y < 0)
        {
            var delta = Mathf.Abs(vector2.y) / percentage;
            _loadIcon.FillIcon(delta);
        }
    }

    private void CheckUpdateFocus(Vector2 vector2)
    {
        var percentage = _loadIcon.MinHeight / (content.rect.height - viewport.rect.height);
        var result = 0f - percentage;

        if (result > vector2.y)
        {
            _loadIcon.EnableLayout(true);
            _loadIcon.EnableFreeRotate(true);
            Debug.Log("There should be update");
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        CheckUpdateFocus(normalizedPosition);
    }

    public void PaginationUpload<TResult>(Task<TResult> task, Action<TResult> action)
    {
        task.RunOnMainThread(result =>
        {
            action?.Invoke(result);
        });
    }
}
