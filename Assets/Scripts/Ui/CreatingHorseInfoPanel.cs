using Ford.SaveSystem.Data;
using Ford.WebApi.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatingHorseInfoPanel : MonoBehaviour
{
    [SerializeField] private Page _userInfoPage;

    [Space(10)]
    [SerializeField] private TextMeshProUGUI _createDateText;
    [SerializeField] private TextMeshProUGUI _creatorNameText;
    [SerializeField] private Button _creatorMoreInfoButton;

    [Space(10)]
    [SerializeField] private TextMeshProUGUI _lastModifiedDateText;
    [SerializeField] private TextMeshProUGUI _lastModifiedUserNameText;
    [SerializeField] private Button _lastModifiedByUserMoreInfoButton;

    private HorseUserDto _creator = null;
    private HorseUserDto _lastUserModified = null;

    private void Start()
    {
        _creatorMoreInfoButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenPage(_userInfoPage, new UserIdentity(_creator.UserId), 4);
        });

        _lastModifiedByUserMoreInfoButton.onClick.AddListener(() =>
        {
            PageManager.Instance.OpenPage(_userInfoPage, new UserIdentity(_lastUserModified.UserId), 4);
        });
    }

    public void SetInfo(UserDate createdData, UserDate lastModifiedData)
    {
        _creator = createdData.User;
        _lastUserModified = lastModifiedData.User;

        _createDateText.text = createdData.Date.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
        _creatorNameText.text = $"{createdData.User.FirstName} {createdData.User.LastName}";

        _lastModifiedDateText.text = lastModifiedData.Date.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
        _lastModifiedUserNameText.text = $"{lastModifiedData.User.FirstName} {lastModifiedData.User.LastName}";
    }
}
