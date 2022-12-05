using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CheckRightSaveText : MonoBehaviour
{
    private string text;
    [SerializeField] private Button buttonApply;
    [SerializeField] private Text exception;
    [SerializeField] private InputField inputField;

    private void Start()
    {
        text = inputField.text;

        if (text.Contains("\\") || text.Contains("/") || text.Contains(".") || text.Contains("?") || 
            text.Contains(":") || text.Contains("*") || text.Contains("\"") || text.Contains(":") || 
            text.Contains("<") || text.Contains(">") || text.Contains("|"))
        {
            buttonApply.enabled = false;
            exception.enabled = true;
        }
        else
        {
            buttonApply.enabled = true;
            exception.enabled = false;
        }
    }

    public void CheckWrongSymbol()
    {
        text = inputField.text;

        if (text.Contains("\\") || text.Contains("/") || text.Contains(".") || text.Contains("?") ||
            text.Contains(":") || text.Contains("*") || text.Contains("\"") || text.Contains(":") ||
            text.Contains("<") || text.Contains(">") || text.Contains("|"))
        {
            buttonApply.enabled = false;
            exception.enabled = true;
        }
        else
        {
            buttonApply.enabled = true;
            exception.enabled = false;
        }
    }

    public static void CheckEmpty()
    {

    }
}
