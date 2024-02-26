using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldValidateStroke : MonoBehaviour
{
    [SerializeField] private Sprite _strokeSprite;
    [SerializeField] private Color _strokeColor = new(1f, 0f, 0f, 1f);

    private TMP_InputField _inputField;
    private GameObject _strokeObject;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        if (_strokeSprite == null)
        {
            Debug.LogError("Нет ссылки на спрайт");
            return;
        }

        _strokeObject = new GameObject("StrokeObject");
        _strokeObject.transform.SetParent(_inputField.transform);

        RectTransform rectStroke = _strokeObject.AddComponent<RectTransform>();
        RectTransform rectInput = _inputField.GetComponent<RectTransform>();
        rectStroke.localScale = Vector3.one;

        rectStroke.anchorMax = Vector2.one;
        rectStroke.anchorMin = Vector2.zero;
        rectStroke.sizeDelta= Vector2.zero;
        rectStroke.anchoredPosition = Vector2.zero;

        Image strokeImage = _strokeObject.AddComponent<Image>();
        strokeImage.type = Image.Type.Sliced;
        strokeImage.sprite = _strokeSprite;
        strokeImage.color = _strokeColor;

        _strokeObject.SetActive(false);

        _inputField.onSelect.AddListener((string str) => { DisplayStroke(false); });
    }

    internal void Initiate(FieldMaskValidate fieldMaskValidate)
    {
        //fieldMaskValidate.OnValidate += (bool value) => 
        //{
        //    if (fieldMaskValidate.ShowStroke)
        //        DisplayStroke(!value); 
        //};
    }

    public void DisplayStroke(bool value)
    {
        _strokeObject?.SetActive(value);
    }
}
