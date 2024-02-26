using System;
using TMPro;
using UnityEngine;

public abstract class FieldMaskValidate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _invalidDescriptionText;
    [SerializeField] private string _descriptionString = "Поле не может быть пустым";
    [SerializeField] private bool _showStroke = true;

    private TMP_InputField _inputField;
    private InputFieldValidateStroke _stroke;

    public bool ShowStroke { get { return _showStroke; } set { _showStroke = value; } }
    public string Text { get { return _inputField.text; } }
    public Action<bool> OnValidate { get; set; }

    protected virtual void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();

        if (_inputField == null)
        {
            Debug.LogError("Ссылка на строку ввода отсутствует");
            return;
        }

        _invalidDescriptionText.gameObject.SetActive(false);
        OnValidate += (bool value) => { if (!value) DisplayException(true); };

        _stroke = _inputField.GetComponent<InputFieldValidateStroke>();
        
        if (_stroke == null)
        {
            Debug.LogError("Компонента \"InputFieldValidateStroke\" не была найдена");
            return;
        }

        //_stroke.Initiate(this);
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

        _stroke?.DisplayStroke(false);

        if (_stroke != null)
        {
            _stroke.enabled = false;
        }
    }

    public abstract bool ValidateInput(string str);

    public virtual bool ValidateInput()
    {
        return ValidateInput(_inputField.text);
    }

    public void DisplayException(bool value)
    {
        _invalidDescriptionText.text = _descriptionString;
        _invalidDescriptionText.gameObject.SetActive(value);

        if (ShowStroke)
        {
            _stroke?.DisplayStroke(value);
        }
    }

    public virtual bool IsTextEmpty()
    {
        return string.IsNullOrEmpty(_inputField.text);
    }
}