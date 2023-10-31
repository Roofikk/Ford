using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private GameObject _prefabGroupBones;
    [SerializeField] private BoneUI _boneUiPrefab;
    [SerializeField] private BoneUI _devBoneUiPrefab;
    [SerializeField] private ScrollRect _bonesScrollView;
    [SerializeField] private Page _editBonePage;
    [SerializeField] private WarningPage _warningDevPage;
    public Page EditBonePage { get { return _editBonePage; } }
    public BoneUI BoneUiPrefab
    {
        get
        {
            return GameManager.Instance.DevMode ? _devBoneUiPrefab : _boneUiPrefab;
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    internal GroupBonesUI CreateGroupBones()
    {
        GroupBonesUI groupBonesUi = Instantiate(_prefabGroupBones, _bonesScrollView.content.transform).GetComponent<GroupBonesUI>();
        return groupBonesUi;
    }

    internal BoneUI CreateBoneUI(Transform parent)
    {
        return Instantiate(BoneUiPrefab.gameObject, parent).GetComponent<BoneUI>();
    }

    public void ShowDevAlertPage()
    {
        var warningData = new WarningData(
            "Внимание",
            "\tВы перешли в режим разработчика. Здесь вы можете изменить расположения костей по умолчанию. Также можете изменить наименование костей." +
            "\r\n\tБудьте внимательны, обратно вернуть значения уже будет невозможно. " +
            "Поэтому, если вы не планировали менять значения на системном уровне, то просто выйдите без сохранения." +
            "\r\n\tТакже можете ознакомиться с информацией о режиме разработчика в руководстве в главном меню.",
            () => { PageManager.Instance.ClosePage(_warningDevPage); },
            null,
            GameManager.Instance.ExitProject);

        PageManager.Instance.OpenPage(_warningDevPage, warningData, 1);
    }
}
