public class EmptyInputValidate : FieldMaskValidate
{
    public override bool ValidateInput(string str)
    {
        OnValidate?.Invoke(!string.IsNullOrEmpty(str));
        return !string.IsNullOrEmpty(str);
    }
}
