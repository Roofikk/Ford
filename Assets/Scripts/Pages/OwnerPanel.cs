using Ford.WebApi.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OwnerPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _ownerNameInput;
    [SerializeField] private TMP_InputField _ownerPhoneNumberInput;
    [SerializeField] private Dropdown _accessDropdown;

    [Space(10)]
    [SerializeField] private Button _searchUserButton;
    [SerializeField] private Button _removeOwnerButton;

    private HorseUserDto _owner;

    private void Start()
    {
        _removeOwnerButton.onClick.AddListener(RemoveOwner);
    }

    private void OnDestroy()
    {
        _removeOwnerButton.onClick.RemoveListener(RemoveOwner);
    }

    private void OnEnable()
    {
        _accessDropdown.gameObject.SetActive(false);
        _removeOwnerButton.gameObject.SetActive(false);
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
        _owner = user;

        _ownerNameInput.text = $"{user.FirstName} {user.LastName}";
        _ownerNameInput.readOnly = true;

        _ownerPhoneNumberInput.text = $"{user.PhoneNumber}";
        _ownerPhoneNumberInput.readOnly = true;

        _accessDropdown.gameObject.SetActive(true);
        _removeOwnerButton.gameObject.SetActive(true);
    }

    public void RemoveOwner()
    {
        _ownerNameInput.readOnly = false;
        _ownerPhoneNumberInput.readOnly = false;
        _accessDropdown.gameObject.SetActive(false);
        _removeOwnerButton.gameObject.SetActive(false);
        _owner = null;
    }
}
