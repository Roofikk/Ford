using System.Text.RegularExpressions;

public class UsernameValidator : FieldMaskValidate
{
    public override bool ValidateInput(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            ExceptionMessage = "���� �� ����� ���� ������";
            OnValidate?.Invoke(false);
            return false;
        }

        var length = str.Length;
        if (length < 3 || length > 24)
        {
            ExceptionMessage = "����� ������ ���� �� ����� 3-x � �� ����� 24-x ��������";
            OnValidate?.Invoke(false);
            return false;
        }

        if (str[0] >= '0' && str[0] <= '9')
        {
            ExceptionMessage = "����� �� ����� ���������� � �����";
            OnValidate?.Invoke(false);
            return false;
        }

        if (!Regex.IsMatch(str, "^[a-zA-Z0-9-._@+]+$"))
        {
            ExceptionMessage = "������������ �������";
            OnValidate?.Invoke(false);
            return false;
        }

        if (!Regex.IsMatch(str, "(.*[a-zA-Z]){3,}"))
        {
            ExceptionMessage = "����� ������ ��������� ���� �� 3 ��������� �������";
            OnValidate?.Invoke(false);
            return false;
        }

        OnValidate?.Invoke(true);
        return true;
    }
}
