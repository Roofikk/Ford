using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LockCustomScroll : MonoBehaviour
{
    [SerializeField] private LoadIcon _loadIcon;
    [SerializeField] private Color _defaultColor;
    public bool IsLocked { get; private set; } = false;

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private GraphicRaycaster _graphicRaycaster;
    private Image _image;
    private Tween _tween;

    private void Start()
    {
        if (_canvas == null)
        {
            Initiate();
        }
    }

    public void Initiate()
    {
        gameObject.SetActive(false);
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _graphicRaycaster = GetComponent<GraphicRaycaster>();
    }

    public void Lock()
    {
        if (_canvas == null)
        {
            Initiate();
        }

        gameObject.SetActive(true);
        IsLocked = true;
        _canvasGroup.alpha = 0f;
        StartCoroutine(ShowSmoothly(.5f));
        _loadIcon.EnableFreeRotate(true);
    }

    public void Unlock()
    {
        IsLocked = false;
        StartCoroutine(HideSmoothly(.5f));
    }

    private IEnumerator ShowSmoothly(float duration)
    {
        var elapsed = 0f;
        var speed = Time.deltaTime / duration;
        while (elapsed < 1)
        {
            elapsed = Mathf.Lerp(0f, 1f, elapsed + speed);
            _canvasGroup.alpha = elapsed;
            yield return new WaitForEndOfFrame();
        }

        _canvasGroup.alpha = 1f;
    }

    private IEnumerator HideSmoothly(float duration)
    {
        float elapsed = 1f;
        var speed = Time.deltaTime / duration;
        while (elapsed > 0)
        {
            elapsed = Mathf.Lerp(1f, 0f, elapsed + speed);
            _canvasGroup.alpha = elapsed;
            yield return new WaitForEndOfFrame();
        }

        _canvasGroup.alpha = 0f;
        StopAll();
    }

    private void StopAll()
    {
        gameObject.SetActive(false);
        _loadIcon.EnableFreeRotate(false);
    }
}
