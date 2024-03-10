using System.Text.RegularExpressions;
using UnityEngine;

public class PhoneValidator : FieldMaskValidate
{
    [SerializeField] private bool _checkEmpty = false;

    public override bool ValidateInput(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            if (_checkEmpty)
            {
                ExceptionMessage = "Поле не может быть пустым";
                OnValidate?.Invoke(false);
                return false;
            }

            OnValidate?.Invoke(true);
            return true;
        }

        if (!Regex.IsMatch(str, @"^([\+]?[0-9]{0,3}[-\s.]?[(\s]?[0-9]{3}[)\s]?[-\s.]?([0-9]{2,3}[-\s]?){2}[0-9]{2})$"))
        {
            ExceptionMessage = "Неверный формат. (+X XXX XXX-XX-XX)";
            OnValidate?.Invoke(false);
            return false;
        }

        OnValidate?.Invoke(true);
        return true;
    }
}
