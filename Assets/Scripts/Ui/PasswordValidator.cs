using System.Text.RegularExpressions;

public class PasswordValidator : FieldMaskValidate
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
        if (length < 8 || length > 24)
        {
            ExceptionMessage = "������ ������ ���� �� ����� 8-� � �� ����� 24-x ��������";
            OnValidate?.Invoke(false);
            return false;
        }

        if (!Regex.IsMatch(str, @"\w+"))
        {
            ExceptionMessage = "������ ������ ��������� ���� �� ���� ���������� ������";
            OnValidate?.Invoke(false);
            return false;
        }

        if (!Regex.IsMatch(str, @"\W+"))
        {
            ExceptionMessage = "������ ������ ��������� ���� �� ���� ����. ������";
            OnValidate?.Invoke(false);
            return false;
        }

        if (Regex.IsMatch(str, @"\s+"))
        {
            ExceptionMessage = "������ �� ����� ��������� �������";
            OnValidate?.Invoke(false);
            return false;
        }

        OnValidate?.Invoke(true);
        return true;
    }
}
