using System;
using UnityEngine;

public class InputFieldDateValidate : FieldMaskValidate
{
    [SerializeField] private bool _validateEmpty = false;
    private DateTime _date;

    public DateTime Date { get { return _date; } }

    public override bool ValidateInput(string str)
    {
        if (!_validateEmpty)
            if (string.IsNullOrEmpty(str))
                return true;

        int count = str.Split(new char[] { '.', '/', ',' }).Length;
        bool value = DateTime.TryParse(str, out _date);

        bool result = value && count == 3;
        if (result)
            _date = default;
        
        OnValidate?.Invoke(result);
        return result;
    }
}
