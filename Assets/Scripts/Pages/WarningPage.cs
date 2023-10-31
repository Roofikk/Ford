using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WarningPage : Page
{
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _warningText;
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _declineButton;
    [SerializeField] private Button _cancelButton;
    
    public override void Open<T>(T param, int popUpLevel)
    {
        if (param == null)
        {
            Debug.LogError("Открыть данную страницу нельзя без параметров");
            return;
        }

        if (param is not WarningData warningData)
        {
            Debug.LogError("Параметр для данной страницы не подходит");
            return;
        }
        else
        {
            base.Open(param, popUpLevel);

            _headerText.text = warningData.Header;
            _warningText.text = warningData.Message;

            _applyButton.onClick.AddListener(warningData.OnApply);
            _applyButton.onClick.AddListener(() => { PageManager.Instance.ClosePage(this); });

            if (warningData.OnDecline != null)
            {
                _declineButton.gameObject.SetActive(true);
                _declineButton.onClick.AddListener(warningData.OnDecline);
                _declineButton.onClick.AddListener(() => { PageManager.Instance.ClosePage(this); });
            }
            else
            {
                _declineButton.gameObject.SetActive(false);
            }

            _cancelButton.gameObject.SetActive(true);
            _cancelButton.onClick.AddListener(() => { PageManager.Instance.ClosePage(this); });

            if (warningData.OnCancel != null)
            {
                _cancelButton.onClick.AddListener(warningData.OnCancel);
            }
        }
    }

    public override void Close()
    {
        base.Close();

        _applyButton.onClick.RemoveAllListeners();
        _declineButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();
    }
}

public class WarningData
{
    public string Header { get; }
    public string Message { get; }
    public UnityAction OnApply;
    public UnityAction OnDecline;
    public UnityAction OnCancel;

    public WarningData(string header, string message, UnityAction onApply, UnityAction onDecline, UnityAction onCancel)
    {
        Header = header;
        Message = message;
        OnApply = onApply;
        OnDecline = onDecline;
        OnCancel = onCancel;
    }
}