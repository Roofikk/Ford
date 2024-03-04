using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Canvas))]
public class Page : MonoBehaviour
{
    private Image _image;
    private Canvas _canvas;

    public bool IsOpen { get; private set; }

    public virtual void Open(int popUpLevel = 0)
    {
        gameObject.SetActive(true);
        IsOpen = true;

        if (popUpLevel > 0)
        {
            if (TryGetComponent(out _image))
            {
                _image.enabled = true;
            }

            if (TryGetComponent(out _canvas))
            {
                _canvas.enabled = true;
                _canvas.overrideSorting = true;
                _canvas.sortingOrder = popUpLevel;
            }
        }
        else
        {
            if (TryGetComponent(out _image))
            {
                _image.enabled = false;
            }
        }
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
        IsOpen = false;
    }

    public virtual void Open<T>(T param, int popUpLevel = 0)
    {
        Open(popUpLevel);
    }
}
