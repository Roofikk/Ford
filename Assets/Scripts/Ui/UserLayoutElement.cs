using Ford.WebApi.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserLayoutElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fullName;
    [SerializeField] private Button _removeButton;

    public event Action OnRemoved;

    private void Start()
    {
        _removeButton.onClick.AddListener(() =>
        {
            OnRemoved?.Invoke();
            Destroy(gameObject);
        });
    }

    public UserLayoutElement Initiate(HorseUserDto user, Action onRemoved)
    {
        _fullName.text = $"{user.FirstName} {user.LastName}".Trim();
        OnRemoved += onRemoved;
        return this;
    }

    public void DisplayRemoveButton(bool enabled)
    {
        _removeButton.gameObject.SetActive(enabled);
    }
}
