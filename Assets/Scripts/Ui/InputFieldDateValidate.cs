using System;
using System.Globalization;
using UnityEngine;

public class InputFieldDateValidate : FieldMaskValidate
{
    [SerializeField] private bool _validateEmpty = false;
    private DateTime? _date;

    public DateTime? Date { get { return _date; } }

    public override bool ValidateInput(string str)
    {
        if (!_validateEmpty)
            if (string.IsNullOrEmpty(str))
                return true;

        bool result;
        if (result = DateTime.TryParseExact(str, "d.M.yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date))
        {
            _date = date;
        }
        else
        {
            _date = null;
        }
        
        OnValidate?.Invoke(result);
        return result;
    }
}
