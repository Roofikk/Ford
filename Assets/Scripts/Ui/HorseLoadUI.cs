using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class HorseLoadUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _horseNameText;
    [SerializeField] private TextMeshProUGUI _ownerNameText;
    [SerializeField] private TextMeshProUGUI _localityText;
    [SerializeField] private Button _removeButton;

    [Space(10)]
    [Header("Color selecting")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _selectingColor = new(1f, 0.75f, 0f, 1f);

    private Image _image;
    private Toggle _toggle;
    private HorseData _horseData;

    public event Action OnDestroyed;

    public void Initiate(HorseData horse, UnityAction<HorseData> onClick, ToggleGroup group)
    {
        _horseData = horse;

        _horseNameText.text = horse.Name;
        _ownerNameText.text = horse.OwnerName;
        _localityText.text = horse.Locality;

        if (_image == null)
        {
            _image = GetComponent<Image>();
        }

        if (_toggle == null)
        {
            _toggle = GetComponent<Toggle>();
            _toggle.group = group;

            _toggle.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    _image.color = _selectingColor;
                    onClick?.Invoke(_horseData);
                }
                else
                {
                    _image.color = _normalColor;
                }
            });
        }

        _removeButton.onClick.AddListener(OpenWarningDeletePage);
    }

    public void UpdateHorseInfo()
    {
        _horseNameText.text = _horseData.Name;
        _ownerNameText.text = _horseData.OwnerName;
        _localityText.text = _horseData.Locality;
    }

    private void OpenWarningDeletePage()
    {
        PageManager.Instance.OpenWarningPage(new WarningData("”даление", "¬ы действительно хотите безвозвратно удалить данную лошадь и все ее сохранени€?", RemoveHorse, null, null), 1);
    }

    private void RemoveHorse()
    {
        Storage storage = new(GameManager.Instance.Settings.PathSave);
        storage.DeleteHorse(_horseData);

        Destroy(gameObject);

        PageManager.Instance.CloseWarningPage();
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}
