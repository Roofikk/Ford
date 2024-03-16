using Ford.WebApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
    private List<HorseUserDto> _users = new();

    public string OwnerName => _owner == null ? _ownerNameInput.text : $"{_owner.FirstName} {_owner.LastName}".Trim();
    public string OwnerPhoneNumber => _owner == null ? _ownerPhoneNumberInput.text : _owner.PhoneNumber;
    public string AccessRole => _owner == null ? ((UserRoleAccess)_accessDropdown.value).ToString() : _owner.AccessRole;
    public HorseUserDto Owner => _owner;
    public ICollection<HorseUserDto> Users => _users;

    public OwnerPanelMode Mode { get; private set; }

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
            _searchPage.OnAddButtonClicked += (user) => AddUser(user);
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

        Mode = OwnerPanelMode.Write;

        DisplayAccessRole(false);

        foreach (var t in _usersGroup.GetComponentsInChildren<UserLayoutElement>())
        {
            Destroy(t.gameObject);
        }
    }

    public override void Open<T>(T param, int popUpLevel = 0)
    {
        base.Open(param, popUpLevel);

        if (param is not OwnerPanelParam ownerParam)
        {
            Debug.LogError($"Need {typeof(OwnerPanelParam)} param");
            return;
        }

        _users = new();
        HorseUserDto owner = ownerParam.Users.SingleOrDefault(u => u.IsOwner);
        Mode = ownerParam.Mode;

        switch (Mode)
        {
            case OwnerPanelMode.Read:
                OpenReadMode();
                SetRealOwner(owner);

                foreach (var user in ownerParam.Users)
                {
                    AddUser(user, false, false);
                }
                break;
            case OwnerPanelMode.Write:
                OpenWriteMode();
                SetRealOwner(owner);

                foreach (var user in ownerParam.Users)
                {
                    AddUser(user, true, false);
                }
                break;
        }
    }

    public override void Close()
    {
        base.Close();

        RemoveOwner();
        _accessDropdown.value = 0;
        Mode = OwnerPanelMode.Write;

        foreach (var t in _usersGroup.GetComponentsInChildren<UserLayoutElement>())
        {
            Destroy(t.gameObject);
        }
    }

    private void OpenReadMode()
    {
        _addYourselfButton.gameObject.SetActive(false);
        _searchUserButton.gameObject.SetActive(false);
        _removeOwnerButton.gameObject.SetActive(false);
        _addUserButton.gameObject.SetActive(false);

        _accessDropdown.interactable = false;
    }

    private void OpenWriteMode()
    {
        _addYourselfButton.gameObject.SetActive(true);
        _searchUserButton.gameObject.SetActive(true);
        _removeOwnerButton.gameObject.SetActive(true);
        _addUserButton.gameObject.SetActive(true);

        _accessDropdown.interactable = true;
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
        if (_users.Any(u => u.Id == user.Id))
        {
            ToastMessage.Show("Нельзя добавить, поскольку пользователь уже находится в списке");
            return;
        }

        user.IsOwner = true;
        _owner = user;

        _ownerNameInput.text = $"{user.FirstName} {user.LastName}";
        _ownerNameInput.SetInteractable(false);

        _ownerPhoneNumberInput.text = string.IsNullOrEmpty(user.PhoneNumber) ? "Неизвестно" : user.PhoneNumber;
        _ownerPhoneNumberInput.SetInteractable(false);

        if (_owner.Id == Player.UserData.UserId)
        {
            DisplayAccessRole(false);
        }
        else
        {
            DisplayAccessRole(true);
        }

        _accessDropdown.value = (int)Enum.Parse<UserRoleAccess>(user.AccessRole);
        if (Mode != OwnerPanelMode.Read)
        {
            _removeOwnerButton.gameObject.SetActive(true);
        }
    }

    public void RemoveOwner()
    {
        _ownerNameInput.text = "";
        _ownerNameInput.SetInteractable(true);

        _ownerPhoneNumberInput.text = "";
        _ownerPhoneNumberInput.SetInteractable(true);

        DisplayAccessRole(false);
        _removeOwnerButton.gameObject.SetActive(false);
        _owner = null;
    }

    private void AddUser(HorseUserDto user, bool displayRemove = true, bool displayMessage = true)
    {
        if (_users.Any(u => u.Id == user.Id))
        {
            if (displayMessage)
            {
                ToastMessage.Show("Пользователь уже добавлен");
            }
            return;
        }

        if (user.Id == Player.UserData.UserId)
        {
            if (displayMessage)
            {
                ToastMessage.Show("Вы не можете добавить себя");
            }
            return;
        }

        if (_owner != null && _owner.Id == user.Id)
        {
            if (displayMessage)
            {
                ToastMessage.Show("Пользователь уже находится на позиции хозяина");
            }
            return;
        }

        user.IsOwner = false;
        Instantiate(_userElementPrefab, _usersGroup)
            .Initiate(user, () => { OnUserRemoved(user); })
            .DisplayRemoveButton(displayRemove);
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
