using Ford.SaveSystem;
using Ford.WebApi.Data;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class HorseLoadElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _horseNameText;
    [SerializeField] private TextMeshProUGUI _locationText;
    [SerializeField] private TextMeshProUGUI _ownerFullNameText;
    [SerializeField] private Button _moreButton;

    [Space(10)]
    [Header("Color selecting")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _selectingColor = new(1f, 0.75f, 0f, 1f);

    private Image _image;
    private Toggle _toggle;
    private HorseBase _horseData;

    public HorseBase HorseData => _horseData;

    public HorseLoadElement Initiate(HorseBase horse, Action onClick, Action onMoreButtonClicked, ToggleGroup group)
    {
        _horseData = horse;

        UpdateHorseInfo(horse);

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
                    onClick?.Invoke();
                }
                else
                {
                    _image.color = _normalColor;
                }
            });
        }

        _moreButton.onClick.AddListener(() => { onMoreButtonClicked?.Invoke(); });
        return this;
    }

    public void UpdateHorseInfo(HorseBase horse)
    {
        _horseData = horse;
        _horseNameText.text = horse.Name;

        HorseUserDto owner = null;
        if (horse.Self.IsOwner)
        {
            owner = horse.Self;
        }
        else
        {
            owner = horse.Users.SingleOrDefault(u => u.IsOwner);
        }

        if (owner != null)
        {
            _ownerFullNameText.text = $"{owner.FirstName} {owner.LastName}".Trim();
        }
        else if (!string.IsNullOrEmpty(horse.OwnerName.Trim()))
        {
            _ownerFullNameText.text = horse.OwnerName.Trim();
        }
        else
        {
            _ownerFullNameText.text = "Безхозный";
        }

        if (!string.IsNullOrEmpty(horse.City.Trim()) || !string.IsNullOrEmpty(horse.Region))
        {
            string location = "";

            if (!string.IsNullOrEmpty(horse.City))
            {
                location = horse.City.Trim();
            }
            else if (!string.IsNullOrEmpty(horse.Region))
            {
                location += $", {horse.Region.Trim()}";
            }
            else
            {
                location = horse.Region.Trim();
            }

            _locationText.text = location;
        }
        else
        {
            _locationText.text = "Неизвестно";
        }
    }
}
