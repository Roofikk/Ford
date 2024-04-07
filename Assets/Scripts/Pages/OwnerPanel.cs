using Ford.SaveSystem.Data;
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
    private HorseUserDto _self;
    private UserAccessRole SelfAccessRole => Enum.Parse<UserAccessRole>(_self.AccessRole);

    public string OwnerName => _owner == null ? _ownerNameInput.text : $"{_owner.FirstName} {_owner.LastName}".Trim();
    public string OwnerPhoneNumber => _owner == null ? _ownerPhoneNumberInput.text : _owner.PhoneNumber;
    public string OwnerAccessRole => ((UserAccessRole)_accessDropdown.value).ToString();
    public HorseUserDto Owner => _owner;
    public ICollection<HorseUserDto> Users => _users;

    public PageMode Mode { get; private set; }

    private void Start()
    {
        _removeOwnerButton.onClick.AddListener(RemoveOwner);
        _addYourselfButton.onClick.AddListener(() =>
        {
            SetRealOwner(new()
            {
                UserId = Player.UserData.UserId,
                FirstName = Player.UserData.FirstName,
                LastName = Player.UserData.LastName,
                PhoneNumber = Player.UserData.PhoneNumber,
                AccessRole = UserAccessRole.Creator.ToString(),
                IsOwner = true,
            });
        });
        _searchUserButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenPage(_searchPage, 4);
            _searchPage.OnAddButtonClicked += SetRealOwner;
        });
        _addUserButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenPage(_searchPage, 4);
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

        Mode = PageMode.Write;

        DisplayAccessRoleDropdown(false);

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
        HorseUserDto owner = null;
        _self = ownerParam.Self;

        if (ownerParam.Self.IsOwner)
        {
            owner = ownerParam.Self;
        }
        else
        {
            owner = ownerParam.Users.SingleOrDefault(u => u.IsOwner);
        }

        Mode = ownerParam.Mode;

        switch (Mode)
        {
            case PageMode.Read:
                OpenReadMode();
                if (owner != null)
                {
                    SetRealOwner(owner);
                }
                else
                {
                    SetCustomOwner(ownerParam.OwnerName, ownerParam.OwnerPhoneNumber);
                }

                foreach (var user in ownerParam.Users)
                {
                    AddUser(user, false, false);
                }
                break;
            case PageMode.Write:
                OpenWriteMode();
                if (owner != null)
                {
                    SetRealOwner(owner);
                }
                else
                {
                    SetCustomOwner(ownerParam.OwnerName, ownerParam.OwnerPhoneNumber);
                }

                foreach (var user in ownerParam.Users)
                {
                    AddUser(user, SelfAccessRole > UserAccessRole.Write, false);
                }
                break;
        }
    }

    public override void Close()
    {
        base.Close();

        RemoveOwner();
        _accessDropdown.value = 0;
        Mode = PageMode.Write;

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
        _addYourselfButton.gameObject.SetActive(SelfAccessRole > UserAccessRole.Write);
        _searchUserButton.gameObject.SetActive(SelfAccessRole > UserAccessRole.Write);
        _removeOwnerButton.gameObject.SetActive(SelfAccessRole > UserAccessRole.Write);
        _addUserButton.gameObject.SetActive(SelfAccessRole > UserAccessRole.Write);


        _accessDropdown.interactable = SelfAccessRole > UserAccessRole.Write;
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
        if (_users.Any(u => u.UserId == user.UserId))
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

        if (_owner.UserId == Player.UserData.UserId)
        {
            DisplayAccessRoleDropdown(false);
        }
        else
        {
            DisplayAccessRoleDropdown(true);
        }

        _accessDropdown.value = (int)Enum.Parse<UserAccessRole>(user.AccessRole);
        if (Mode != PageMode.Read)
        {
            _removeOwnerButton.gameObject.SetActive(SelfAccessRole > UserAccessRole.Write);
        }
    }

    public void SetCustomOwner(string ownerName, string ownerPhoneNumber)
    {
        _ownerNameInput.text = ownerName;
        _ownerPhoneNumberInput.text = ownerPhoneNumber;

        DisplayAccessRoleDropdown(false);
    }

    public void RemoveOwner()
    {
        _ownerNameInput.text = "";
        _ownerNameInput.SetInteractable(true);

        _ownerPhoneNumberInput.text = "";
        _ownerPhoneNumberInput.SetInteractable(true);

        DisplayAccessRoleDropdown(false);
        _removeOwnerButton.gameObject.SetActive(false);
        _owner = null;
    }

    private void AddUser(HorseUserDto user, bool displayRemove = true, bool displayMessage = true)
    {
        if (_users.Any(u => u.UserId == user.UserId))
        {
            if (displayMessage)
            {
                ToastMessage.Show("Пользователь уже добавлен");
            }
            return;
        }

        if (user.UserId == Player.UserData.UserId)
        {
            if (displayMessage)
            {
                ToastMessage.Show("Вы не можете добавить себя");
            }
            return;
        }

        if (_owner != null && _owner.UserId == user.UserId)
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

    private void DisplayAccessRoleDropdown(bool enabled)
    {
        _accessRoleObject.SetActive(enabled);
        _accessDropdown.gameObject.SetActive(enabled);
    }
}
