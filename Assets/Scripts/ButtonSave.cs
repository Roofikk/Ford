using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSave : MonoBehaviour
{
    private Button _button;
    [SerializeField] private Text text;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(OnClicked);
    }

    public void OnClicked()
    {

    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
}