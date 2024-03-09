using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Canvas))]
public class ToastMessage : MonoBehaviour
{
    [SerializeField] private float _time = 2f;
    [SerializeField] private float _timeVisibility = 0.5f;
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    private bool _isShowing;
    private bool _isHidhing;
    private CanvasGroup _canvasGroup;
    private float t = .0f;

    public event Action OnHidedMessage;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (_canvasGroup == null)
        {
            Debug.LogError("Canvas is null");
        }

        _canvasGroup.alpha = .0f;
        gameObject.SetActive(true);

        _isHidhing = false;
    }

    private void Update()
    {
        if (_isShowing)
        {
            _canvasGroup.alpha = Mathf.Lerp(0, 1, t);

            t += Time.deltaTime / _timeVisibility;

            if (t > 1.0f)
            {
                _isShowing = false;
                t = 0f;

                StartCoroutine(ShowMessage());
            }
        }

        if (_isHidhing)
        {
            _canvasGroup.alpha = Mathf.Lerp(1, 0, t);

            t += Time.deltaTime / _timeVisibility;

            if (t > 1.0f)
            {
                _isHidhing = false;
                t = 0f;

                gameObject.SetActive(false);
                OnHidedMessage?.Invoke();
            }
        }
    }

    public void Show(string message)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();

        _textMeshPro.text = message;

        _isHidhing = false;
        _isShowing = true;
    }

    public static void Show(string message, Transform parent = null)
    {
        GameObject toastPrefab = (GameObject)Resources.Load("Prefabs/UI/Toast", typeof(GameObject));
        GameObject toastObject = null;
        if (parent != null)
        {
            toastObject = Instantiate(toastPrefab, parent);
        }
        else
        {
            toastObject = Instantiate(toastPrefab, GameObject.Find("ToastCanvas").transform);
        }

        ToastMessage toast = toastObject.GetComponent<ToastMessage>();

        toast.Show(message);
        toast.OnHidedMessage += () => { Destroy(toastObject); };
    }

    private IEnumerator ShowMessage()
    {
        yield return new WaitForSeconds(_time);

        Hide();
    }

    private void Hide()
    {
        _isHidhing = true;
    }
}
