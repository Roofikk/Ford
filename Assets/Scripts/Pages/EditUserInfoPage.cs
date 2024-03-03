using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditUserInfoPage : Page
{
    [SerializeField] private TMP_InputField _firstNameInput;
    [SerializeField] private TMP_InputField _lastNameInput;
    [SerializeField] private TMP_InputField _birthDateInput;
    [SerializeField] private TMP_InputField _cityInput;
    [SerializeField] private TMP_InputField _regionInput;
    [SerializeField] private TMP_InputField _countryInput;

    [Space(10)]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _applyButton;

    public override void Open<T>(T param, int popUpLevel = 0)
    {
        base.Open(param, popUpLevel);
    }

    public override void Close()
    {
        base.Close();
    }
}
