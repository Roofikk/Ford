using System;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;
    private bool _isArrowsShow = true;
    private bool _isTorusShow = false;
    //private bool _contextIsShow;

    //[SerializeField] private ScrollView _groupBonesScrollView;
    //[SerializeField] private Text _nameHorseText;
    //[SerializeField] private Text _nameHorseInputFieldText;
    //[SerializeField] private Text _cityText;
    //[SerializeField] private Text _cityInputFieldText;
    //[SerializeField] private Text _dateBeginText;
    //[SerializeField] private Text _dateBeginInputFieldText;
    //[SerializeField] private Text _exception;
    //[SerializeField] private Color _colorException;
    //[SerializeField] private Color _colorNormal;
    //[SerializeField] private GameObject _actionMetric;
    //[SerializeField] private Text _actionMetricText;
    //[SerializeField] private GameObject _contextBonePanel;
    //[SerializeField] private GameObject _contextFreePanel;

    [SerializeField] private GameObject _prefabGroupBones;
    [SerializeField] private GameObject _prefabBone;
    [SerializeField] public ScrollRect _bonesScrollView;

    public bool IsArrowsShow { get { return _isArrowsShow; } }
    public bool IsTorusShow { get { return _isTorusShow; } }
    //public bool ContextIsShow { get { return _contextIsShow; } }

    public InfoBone infoBone;
    public event Action onQuit;
    public event Action onStartGame;
    public event Action<bool> OnDisplayArrows;
    public event Action<bool> OnDisplayTorus;
    public event Action<Player> OnStartNewProject;
    public event Action OnChangedMode;

    public enum Mode
    {
        Date,
        Name,
        City
    }

    public enum ModeContext
    {
        Bone,
        Free,
        All
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        //if (SceneManager.GetActiveScene().buildIndex == 0)
        //{
        //    _nameHorseText.color = _colorNormal;
        //    _nameHorseInputFieldText.color = _colorNormal;
        //    _cityText.color = _colorNormal;
        //    _cityInputFieldText.color = _colorNormal;
        //    _dateBeginText.color = _colorNormal;
        //    _dateBeginInputFieldText.color = _colorNormal;
        //}

        //if (SceneManager.GetActiveScene().buildIndex == 1)
        //{
        //    _actionMetricText.text = "0";
        //    _actionMetric.SetActive(false);
        //}
    }

    public void InitiateGroupBoneUI()
    {

    }

    public void QuitToStartMenu()
    {
        if (onQuit != null)
            onQuit();
    }

    public void StartNewProjectButtonClicked()
    {
        //string[] dateParse = _dateBeginInputFieldText.text.Split('.');

        //DateTime dt;
        //if (DateTime.TryParse(_dateBeginInputFieldText.text, out dt))
        //{
        //    Player player = new Player(_nameHorseInputFieldText.text, _cityInputFieldText.text, new DateTime(int.Parse(dateParse[2]), int.Parse(dateParse[1]), int.Parse(dateParse[0])));
        //    OnStartNewProject?.Invoke(player);
        //}
        //else
        //{

        //}
    }

    public void ShowArrowsButtonClick()
    {
        if (OnDisplayTorus != null)
            OnDisplayTorus(false);

        if (OnDisplayArrows != null)
            OnDisplayArrows(true);

        _isArrowsShow = true;
        _isTorusShow = false;
    }

    public void ShowTorusButtonClick()
    {
        if (OnDisplayTorus != null)
            OnDisplayTorus(true);

        if (OnDisplayArrows != null)
            OnDisplayArrows(false);

        _isArrowsShow = false;
        _isTorusShow = true;
    }

    public void ShowException(Mode mode, string message)
    {
        //if (mode == Mode.Date)
        //{
        //    _dateBeginInputFieldText.color = new Color(1f, 0f, 0f);
        //    _dateBeginText.color = new Color(1f, 0f, 0f);
        //}

        //_exception.gameObject.SetActive(true);
        //_exception.text = message;
    }

    public void UnshowException()
    {
        //_exception.gameObject.SetActive(false);
    }

    public void SetNormalColor(Mode mode)
    {
        //if (mode == Mode.Date)
        //{
        //    _dateBeginText.color = _colorNormal;
        //    _dateBeginInputFieldText.color = _colorNormal;
        //}
    }

    public void ShowActionMetric(bool isShow, Vector2 v)
    {
        //_actionMetric.SetActive(isShow);
        //var rect = _actionMetric.GetComponent<RectTransform>();
        //Vector2 anchorPos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(_actionMetric.GetComponentInParent<Canvas>().GetComponent<RectTransform>(), v, 
        //                                                        null, out anchorPos);
        //rect.anchoredPosition = anchorPos;
    }

    public void ShowActionMetric(bool isShow)
    {
        //_actionMetric.SetActive(isShow);
    }

    public void UpdateActionMetric(int value)
    {
        //_actionMetricText.text = value.ToString();
    }

    public void DisplayContextPanel(bool isShow, ModeContext mode)
    { 
        //if (mode == ModeContext.Bone)
        //{
        //    _contextBonePanel.SetActive(isShow);
        //    if (isShow)
        //    {
        //        var rect = _contextBonePanel.GetComponent<RectTransform>();
        //        Vector2 anchorPos;
        //        RectTransformUtility.ScreenPointToLocalPointInRectangle(_contextBonePanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>(), Input.mousePosition,
        //                                                                null, out anchorPos);
        //        rect.anchoredPosition = anchorPos;
        //    }
        //}

        //if (mode == ModeContext.Free)
        //{
        //    _contextFreePanel.SetActive(isShow);
        //    if (isShow)
        //    {
        //        Vector2 anchorPos;
        //        RectTransformUtility.ScreenPointToLocalPointInRectangle(_contextFreePanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>(), Input.mousePosition,
        //                                                                null, out anchorPos);
        //        _contextFreePanel.GetComponent<RectTransform>().anchoredPosition = anchorPos;
        //    }
        //}

        //if (mode == ModeContext.All)
        //{
        //    //_contextFreePanel?.SetActive(isShow);
        //    //_contextBonePanel?.SetActive(isShow);
        //}
    }

    internal GroupBonesUI CreateGroupBones()
    {
        GroupBonesUI groupBonesUi = Instantiate(_prefabGroupBones, _bonesScrollView.content.transform).GetComponent<GroupBonesUI>();
        return groupBonesUi;
    }

    internal BoneUI CreateBoneUI(Transform parent)
    {
        BoneUI boneUi = Instantiate(_prefabBone, parent).GetComponent<BoneUI>();
        return boneUi;
    }
}
