public class EmptyInputValidate : FieldMaskValidate
{
    public override bool ValidateInput(string str)
    {
        str = str.Trim();
        OnValidate?.Invoke(!string.IsNullOrEmpty(str));
        return !string.IsNullOrEmpty(str);
    }
}
