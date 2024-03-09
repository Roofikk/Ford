using Ford.SaveSystem;
using Ford.SaveSystem.Ver2;
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
    [SerializeField] private TextMeshProUGUI _cityText;
    [SerializeField] private Button _removeButton;

    [Space(10)]
    [Header("Color selecting")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _selectingColor = new(1f, 0.75f, 0f, 1f);

    private Image _image;
    private Toggle _toggle;
    private HorseBase _horseData;

    public event Action OnDestroyed;

    public void Initiate(HorseBase horse, UnityAction<HorseBase> onClick, ToggleGroup group)
    {
        _horseData = horse;
        _horseNameText.text = horse.Name;

        if (string.IsNullOrEmpty(horse.OwnerName))
        {
            _ownerNameText.text = horse.OwnerName;
        }
        else
        {
            _ownerNameText.text = "Безхозный";
        }

        _cityText.text = horse.City;

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
        _cityText.text = _horseData.City;
    }

    private void OpenWarningDeletePage()
    {
        PageManager.Instance.OpenWarningPage(new WarningData("Удаление", "Вы действительно хотите безвозвратно удалить данную лошадь и все ее сохранения?", RemoveHorse, null, null), 1);
    }

    private void RemoveHorse()
    {
        Storage storage = new(GameManager.Instance.Settings.PathSave);
        storage.DeleteHorse(_horseData.HorseId);

        Destroy(gameObject);

        PageManager.Instance.CloseWarningPage();
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}
