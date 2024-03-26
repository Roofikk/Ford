using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private Vector2 _endPosition;
    [SerializeField] private Canvas _parentCanvas;

    [Space(10)]
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _cancelButton;

    private Tween _showTween;
    private Tween _hideTween;

    private static Stack<Notification> _stack = new();

    public Vector2 StartPosition => _startPosition; 
    public Vector2 EndPosition => _endPosition;
    public Canvas ParentCanvas => _parentCanvas;

    public static Notification ShowNotification(string message, float duration, Action onApplied = null)
    {
        var notificationPrefab = (Resources.Load("Prefabs/UI/Notification", typeof(GameObject)) as GameObject).GetComponent<Notification>();
        var notification = Instantiate(notificationPrefab, notificationPrefab.ParentCanvas.transform);
        notification.transform.position = notification.StartPosition;

        notification.Initiate(message, duration, onApplied);
        _stack.Push(notification);

        if (_stack.Count < 2)
        {
            notification.Show();
        }

        return notification;
    }

    private void Initiate(string message, float duration, Action onApplied)
    {
        _messageText.text = message;
        _showTween = transform.DOMove(_endPosition, 2f, false);
        _showTween.TogglePause();

        if (onApplied == null)
        {
            _applyButton.gameObject.SetActive(false);
        }
        else
        {
            _applyButton.onClick.AddListener(() =>
            {
                onApplied?.Invoke();
            });
        }

        _showTween.onComplete += () => StartCoroutine(WaitBeforeHide(duration));
    }

    private void Show()
    {
        _showTween.TogglePause();
    }

    private System.Collections.IEnumerator WaitBeforeHide(float duration)
    {
        yield return new WaitForSeconds(duration);
        Hide();
    }

    public void Hide()
    {
        _hideTween = transform.DOMove(_startPosition, 2f, false);
        _hideTween.onComplete += () =>
        {
            Destroy(gameObject);
            _stack.Pop().Show();
        };
    }
}
