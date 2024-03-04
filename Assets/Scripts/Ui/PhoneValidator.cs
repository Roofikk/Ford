using System.Text.RegularExpressions;

public class PhoneValidator : FieldMaskValidate
{
    public override bool ValidateInput(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            if (!Regex.IsMatch(str, @"^([\+]?[0-9]{0,3}[-\s.]?[(\s]?[0-9]{3}[)\s]?[-\s.]?([0-9]{2,3}[-\s]?){2}[0-9]{2})$"))
            {
                OnValidate?.Invoke(false);
                return false;
            }
        }

        OnValidate?.Invoke(true);
        return true;
    }
}
