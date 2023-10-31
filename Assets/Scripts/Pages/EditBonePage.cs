using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditBonePage : Page
{
    [SerializeField] private TextMeshProUGUI _headerPage;
    [SerializeField] private TextMeshProUGUI _idText;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Button _applyButton;

    [Space(10)]
    [SerializeField] private ToastMessage _toastMessage;

    private BoneObject _bone;

    private void Start()
    {
        _applyButton.onClick.AddListener(OnApplyClicked);
    }

    private void OnDestroy()
    {
        _applyButton.onClick.RemoveAllListeners();
    }

    public override void Open<T>(T param, int popUpLevel)
    {
        base.Open(param, popUpLevel);

        if (param.GetType() != typeof(BoneObject))
        {
            Debug.LogError("Неверный тип параметра");
            return;
        }

        _bone = param as BoneObject;

        _headerPage.text = $"Изменение кости - {_bone.BoneData.Name}";
        _idText.text = _bone.BoneData.Id;
        _nameInputField.text = _bone.BoneData.Name;
    }

    private void OnApplyClicked()
    {
        if (_nameInputField.TryGetComponent(out EmptyInputValidate inputValidate))
        {
            if (!inputValidate.ValidateInput())
                return;
        }

        _bone.EditBoneName(_nameInputField.text);
        _toastMessage.Show("Изменения применены");
        Close();
    }
}
