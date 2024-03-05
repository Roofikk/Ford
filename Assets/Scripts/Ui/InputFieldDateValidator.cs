using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public class InputFieldDateValidator : FieldMaskValidate
{
    [SerializeField] private bool _validateEmpty = false;
    private DateTime? _date;

    public DateTime? Date { get { return _date; } }

    public override bool ValidateInput(string str)
    {
        ExceptionMessage = "Неверный формат даты";

        if (string.IsNullOrEmpty(str))
        {
            _date = null;
            if (_validateEmpty)
            {
                ExceptionMessage = "Поле не может быть пустым";
                OnValidate?.Invoke(false);
                return false;
            }
            OnValidate?.Invoke(true);
            return true;
        }

        var regex = new Regex(@"^(\d{1,2})[-\s.,\/\\]{1}(\d{1,2})[-\s.,\/\\](\d{4})");
        var match = regex.Match(str);
        
        if (!match.Success)
        {
            OnValidate?.Invoke(false);
            return false;
        }

        if (!int.TryParse(match.Groups[1].Value, out int day))
        {
            OnValidate?.Invoke(false);
            return false;
        }

        if (!int.TryParse(match.Groups[2].Value, out int month))
        {
            OnValidate?.Invoke(false);
            return false;
        }

        if (!int.TryParse(match.Groups[3].Value, out int year))
        {
            OnValidate?.Invoke(false);
            return false;
        }

        if (!DateTime.TryParseExact($"{day}.{month}.{year}", "d.M.yyyy", 
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            OnValidate?.Invoke(false);
            return false;
        }
        
        _date = date;

        OnValidate?.Invoke(true);
        return true;
    }
}
