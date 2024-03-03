using System.Text.RegularExpressions;

public class UsernameValidator : FieldMaskValidate
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
        if (length < 3 || length > 24)
        {
            ExceptionMessage = "Логин должен быть не менее 3-x и не более 24-x символов";
            OnValidate?.Invoke(false);
            return false;
        }

        if (str[0] >= '0' && str[0] <= '9')
        {
            ExceptionMessage = "Логин не может начинаться с цифры";
            OnValidate?.Invoke(false);
            return false;
        }

        if (!Regex.IsMatch(str, "^[a-zA-Z0-9-._@+]+$"))
        {
            ExceptionMessage = "Недопустимые символы";
            OnValidate?.Invoke(false);
            return false;
        }

        if (!Regex.IsMatch(str, "(.*[a-zA-Z]){3,}"))
        {
            ExceptionMessage = "Логин должен содержать хотя бы 3 латинских символа";
            OnValidate?.Invoke(false);
            return false;
        }

        OnValidate?.Invoke(true);
        return true;
    }
}
