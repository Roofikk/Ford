using Ford.WebApi.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OwnerPanel : Page
{
    [SerializeField] private SearchUserPage _searchPage;

    [Space(10)]
    [SerializeField] private TMP_InputField _ownerNameInput;
    [SerializeField] private TMP_InputField _ownerPhoneNumberInput;
    [SerializeField] private GameObject _accessRoleObject;
    [SerializeField] private TMP_Dropdown _accessDropdown;

    [Space(10)]
    [SerializeField] private Button _addYourselfButton;
    [SerializeField] private Button _searchUserButton;
    [SerializeField] private Button _removeOwnerButton;
    [SerializeField] private Button _addUserButton;

    [Space(10)]
    [SerializeField] private Transform _usersGroup;
    [SerializeField] private UserLayoutElement _userElementPrefab;

    private HorseUserDto _owner;
    private List<HorseUserDto> _users;

    public string OwnerName => _ownerNameInput.text;
    public string OwnerNumber => _ownerPhoneNumberInput.text;

    private void Start()
    {
        _removeOwnerButton.onClick.AddListener(RemoveOwner);
        _addYourselfButton.onClick.AddListener(() =>
        {
            SetRealOwner(new()
            {
                Id = Player.UserData.UserId,
                FirstName = Player.UserData.FirstName,
                LastName = Player.UserData.LastName,
                PhoneNumber = Player.UserData.PhoneNumber,
                AccessRole = UserRoleAccess.Creator.ToString(),
                IsOwner = true,
            });
        });
        _searchUserButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenPage(_searchPage, 2);
            _searchPage.OnAddButtonClicked += SetRealOwner;
        });
        _addUserButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenPage(_searchPage, 2);
            _searchPage.OnAddButtonClicked += AddUser;
        });
    }

    private void OnDestroy()
    {
        _addYourselfButton.onClick.RemoveAllListeners();
        _removeOwnerButton.onClick.RemoveListener(RemoveOwner);
        _searchUserButton.onClick.RemoveAllListeners();
        _addUserButton.onClick.RemoveAllListeners();
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        _addYourselfButton.gameObject.SetActive(Player.IsLoggedIn);
        _searchUserButton.gameObject.SetActive(Player.IsLoggedIn);
        _addUserButton.gameObject.SetActive(Player.IsLoggedIn);
        _removeOwnerButton.gameObject.SetActive(false);

        foreach (var t in _usersGroup.GetComponentsInChildren<UserLayoutElement>())
        {
            Destroy(t.gameObject);
        }
    }

    public override void Close()
    {
        base.Close();

        RemoveOwner();
        _accessDropdown.value = 0;

        foreach (var t in _usersGroup.GetComponentsInChildren<UserLayoutElement>())
        {
            Destroy(t.gameObject);
        }
    }

    public void RefreshData()
    {
        if (Player.IsLoggedIn)
        {
            _searchUserButton.gameObject.SetActive(true);
        }
        else
        {
            _searchUserButton.gameObject.SetActive(false);
        }
    }

    public void SetRealOwner(HorseUserDto user)
    {
        user.IsOwner = true;
        _owner = user;

        _ownerNameInput.text = $"{user.FirstName} {user.LastName}";
        _ownerNameInput.readOnly = true;

        _ownerPhoneNumberInput.text = $"{user.PhoneNumber}";
        _ownerPhoneNumberInput.readOnly = true;

        if (_owner.Id == Player.UserData.UserId)
        {
            DisplayAccessRole(false);
        }
        else
        {
            DisplayAccessRole(true);
        }

        _accessDropdown.value = (int)Enum.Parse<UserRoleAccess>(user.AccessRole);
        _removeOwnerButton.gameObject.SetActive(true);
    }

    public void RemoveOwner()
    {
        _ownerNameInput.readOnly = false;
        _ownerNameInput.text = string.Empty;

        _ownerPhoneNumberInput.readOnly = false;
        _ownerPhoneNumberInput.text = string.Empty;

        DisplayAccessRole(true);
        _removeOwnerButton.gameObject.SetActive(false);
        _owner = null;
    }

    private void AddUser(HorseUserDto user)
    {
        if (_users.Contains(user))
        {
            ToastMessage.Show("Пользователь уже добавлен");
        }

        user.IsOwner = false;
        Instantiate(_userElementPrefab, _usersGroup).Initiate(user, () => { OnUserRemoved(user); });
        _users.Add(user);
    }

    private void OnUserRemoved(HorseUserDto user)
    {
        if (_users.Contains(user))
        {
            _users.Remove(user);
        }
    }

    public HorseUserDto GetOwner()
    {
        return _owner;
    }

    private void DisplayAccessRole(bool enabled)
    {
        _accessRoleObject.SetActive(enabled);
        _accessDropdown.gameObject.SetActive(enabled);
    }
}
