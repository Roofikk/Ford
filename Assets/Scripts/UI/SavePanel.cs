using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SavePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _localityInputField;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _applyButton;
    [SerializeField] private TextMeshProUGUI _exceptionText;

    private void Start()
    {
        _applyButton.onClick.AddListener(Save);
    }

    private void OnDestroy()
    {
        _applyButton.onClick.RemoveAllListeners();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        _localityInputField.text = "";
    }

    public void Save()
    {

    }
}