using System;
using TMPro;
using UnityEngine;

public abstract class FieldMaskValidate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _invalidDescriptionText;
    [SerializeField] private string _descriptionString = "Поле не может быть пустым";
    [SerializeField] private bool _showStroke = true;

    private TMP_InputField _inputField;

    public bool ShowStroke { get { return _showStroke; } private set { _showStroke = value; } }
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

        _inputField.onDeselect.AddListener((string str) => { ValidateInput(str); });

        _invalidDescriptionText.gameObject.SetActive(false);

        OnValidate += (bool value) => { if (!value) DisplayDescription(true); };
        _inputField.onSelect.AddListener((string str) => { DisplayDescription(false); });

        var stroke = _inputField.GetComponent<InputFieldValidateStroke>();
        
        if (stroke == null)
        {
            Debug.LogError("Компонента \"InputFieldValidateStroke\" не была найдена");
            return;
        }

        stroke.Initiate(this);
    }

    public abstract bool ValidateInput(string str);

    public virtual bool ValidateInput()
    {
        return ValidateInput(_inputField.text);
    }

    public void DisplayDescription(bool value)
    {
        _invalidDescriptionText.text = _descriptionString;
        _invalidDescriptionText.gameObject.SetActive(value);
    }

    public virtual bool IsTextEmpty()
    {
        return string.IsNullOrEmpty(_inputField.text);
    }
}