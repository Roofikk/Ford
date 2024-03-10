using TMPro;
using UnityEngine;

public static class InputFieldExtension
{
    public static void SetInteractable(this TMP_InputField input, bool enabled)
    {
        Color color = input.image.color;

        if (enabled)
        {
            input.image.color = new(color.r, color.g, color.b, 1f);
        }
        else
        {
            input.image.color = new(color.r, color.g, color.b, .25f);
        }

        if (input.TryGetComponent<FieldMaskValidate>(out var validator))
        {
            if (!enabled)
            {
                validator.DisplayException(enabled);
            }

            validator.enabled = enabled;
        }

        input.placeholder.gameObject.SetActive(enabled);
        input.readOnly = !enabled;
    }
}
