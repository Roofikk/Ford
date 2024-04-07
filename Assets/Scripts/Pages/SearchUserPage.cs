using Ford.SaveSystem.Data;
using Ford.WebApi;
using Ford.WebApi.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchUserPage : Page
{
    [Header("Navigating elements")]
    [SerializeField] private TMP_InputField _searchInput;
    [SerializeField] private Button _searchButton;
    [SerializeField] private Button _closeButton;

    [Space(10)]
    [Header("Person info")]
    [SerializeField] private GameObject _personPanel;
    [SerializeField] private TextMeshProUGUI _fullNameText;
    [SerializeField] private TextMeshProUGUI _locationText;
    [SerializeField] private TextMeshProUGUI _phoneNumberText;
    [SerializeField] private TMP_Dropdown _accessRoleDropdown;

    [Space(10)]
    [SerializeField] private Button _addButton;
    public event Action<HorseUserDto> OnAddButtonClicked;

    private User _user;

    private void Start()
    {
        _closeButton.onClick.AddListener(() => { PageManager.Instance.ClosePage(this); });
        _searchButton.onClick.AddListener(FindUser);
        _addButton.onClick.AddListener(AddUser);
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);
        Initiate();
    }

    public override void Close()
    {
        base.Close();
        Initiate();
    }

    private void Initiate()
    {
        _personPanel.SetActive(false);
        _searchInput.text = "";
        OnAddButtonClicked = null;
        _user = null;
    }

    private void FindUser()
    {
        // ready for search
        PageManager.Instance.DisplayLoadingPage(true, 6);

        //search
        FordApiClient client = new();
        client.FindUser(_searchInput.text.Trim()).RunOnMainThread((result) =>
        {
            PageManager.Instance.DisplayLoadingPage(false);

            if (result.Content != null)
            {
                OnUserFound(result.Content);
            }
            else
            {
                ToastMessage.Show("Пользователь с таким ником не найден");
            }
        });
    }

    private void OnUserFound(User user)
    {
        _user = user;
        _personPanel.SetActive(true);
        _fullNameText.text = $"{user.FirstName} {user.LastName}";

        if (string.IsNullOrEmpty(user.City) && string.IsNullOrEmpty(user.Region))
        {
            _locationText.text = "Неизвестно";
        }
        else
        {
            _locationText.text = $"{user.City}, {user.Region}".Trim();
        }

        _phoneNumberText.text = string.IsNullOrEmpty(user.PhoneNumber) ? "Неизвестно" : user.PhoneNumber;
    }

    private void AddUser()
    {
        HorseUserDto user = new()
        {
            UserId = _user.UserId,
            FirstName = _user.FirstName,
            LastName = _user.LastName,
            PhoneNumber = _user.PhoneNumber,
            AccessRole = ((UserAccessRole)_accessRoleDropdown.value).ToString(),
            IsOwner = false
        };

        OnAddButtonClicked?.Invoke(user);
        PageManager.Instance.ClosePage(this);
    }
}
