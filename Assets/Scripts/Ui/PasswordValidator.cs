using System.Text.RegularExpressions;

public class PasswordValidator : FieldMaskValidate
{
    public override bool ValidateInput(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            ExceptionMessage = "Поле не может быть пустым";
            OnValidate?.Invoke(false);
            return false;
        }

        var length = str.Length;
        if (length < 8 || length > 24)
        {
            ExceptionMessage = "Пароль должен быть не менее 8-и и не более 24-x символов";
            OnValidate?.Invoke(false);
            return false;
        }

        if (!Regex.IsMatch(str, @"\w+"))
        {
            ExceptionMessage = "Пароль должен содержать хотя бы один алфавитный символ";
            OnValidate?.Invoke(false);
            return false;
        }

        if (!Regex.IsMatch(str, @"\W+"))
        {
            ExceptionMessage = "Пароль должен содержать хотя бы один спец. символ";
            OnValidate?.Invoke(false);
            return false;
        }

        if (Regex.IsMatch(str, @"\s+"))
        {
            ExceptionMessage = "Пароль не может содержать пробелы";
            OnValidate?.Invoke(false);
            return false;
        }

        OnValidate?.Invoke(true);
        return true;
    }
}
