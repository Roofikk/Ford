using Ford.SaveSystem.Data;
using Ford.WebApi.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserLayoutElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _usernameText;
    [SerializeField] private Button _removeButton;
    [SerializeField] private Button _button;

    private long _userId;

    public event Action OnRemoved;

    private void Start()
    {
        _button = _button == null ? GetComponent<Button>() : _button;

        _button.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenPage(PageManager.Instance.UserInfoPage, new UserIdentity(_userId), 4);
        });

        _removeButton.onClick.AddListener(() =>
        {
            OnRemoved?.Invoke();
            Destroy(gameObject);
        });
    }

    public UserLayoutElement Initiate(HorseUserDto user, Action onRemoved)
    {
        _userId = user.UserId;
        _usernameText.text = $"{user.FirstName} {user.LastName}".Trim();
        OnRemoved += onRemoved;
        return this;
    }

    public void DisplayRemoveButton(bool enabled)
    {
        _removeButton.gameObject.SetActive(enabled);
    }
}
