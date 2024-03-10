using System;
using TMPro;
using UnityEngine;

public abstract class FieldMaskValidate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _invalidDescriptionText;
    [SerializeField] private string _descriptionString = "���� �� ����� ���� ������";
    [SerializeField] private bool _showStroke = true;

    private TMP_InputField _inputField;
    private InputFieldValidateStroke _stroke;

    public string ExceptionMessage { get { return _descriptionString; } protected set { _descriptionString = value; } }

    public bool ShowStroke { get { return _showStroke; } set { _showStroke = value; } }
    public string Text { get { return _inputField.text; } }
    public Action<bool> OnValidate { get; set; }

    protected virtual void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();

        if (_inputField == null)
        {
            Debug.LogError("������ �� ������ ����� �����������");
            return;
        }

        if (_invalidDescriptionText != null)
        {
            _invalidDescriptionText?.gameObject.SetActive(false);
        }

        OnValidate += (bool value) => { if (!value) DisplayException(true); };

        _stroke = _inputField.GetComponent<InputFieldValidateStroke>();
        
        if (_stroke == null)
        {
            Debug.LogError("���������� \"InputFieldValidateStroke\" �� ���� �������");
            return;
        }
    }

    private void OnEnable()
    {
        _inputField.onDeselect.AddListener((string str) => { ValidateInput(str); });
        _inputField.onSelect.AddListener((string str) => { DisplayException(false); });

        _stroke?.DisplayStroke(false);

        if (_stroke != null)
        {
            _stroke.enabled = true;
        }
    }

    private void OnDisable()
    {
        _inputField.onSelect.RemoveAllListeners();
        _inputField.onDeselect.RemoveAllListeners();

        if (_invalidDescriptionText != null)
        {
            _invalidDescriptionText.gameObject.SetActive(false);
        }

        _stroke?.DisplayStroke(false);

        if (_stroke != null)
        {
            _stroke.enabled = false;
        }
    }

    public abstract bool ValidateInput(string str);

    public virtual bool ValidateInput()
    {
        if (!enabled)
        {
            return true;
        }

        return ValidateInput(_inputField.text);
    }

    public void DisplayException(bool value)
    {
        if (_invalidDescriptionText != null)
        {
            _invalidDescriptionText.text = _descriptionString;
            _invalidDescriptionText.gameObject.SetActive(value);
        }

        if (ShowStroke)
        {
            _stroke?.DisplayStroke(value);
        }
    }
}