using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoadingPage : Page
{
    [SerializeField] private Animator _animator;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [Range(0.2f, 1f)]
    [SerializeField] private float _speedChangingColor = .75f;

    private Tween _tween;

    private void Start()
    {
        Open();
    }

    private void Update()
    {
        int i = 0;
    }

    public override void Open(int popUpLevel = 5)
    {
        base.Open(popUpLevel);

        _animator.Play("HorseLoadingAnimation");
        StartLoopChangeColor();
    }

    public override void Close()
    {
        base.Close();

        _animator.StopPlayback();
        _tween.Kill();
    }

    private void StartLoopChangeColor()
    {
        _tween = _loadingText.DOColor(new Color(.75f, .75f, .75f, 1f), _speedChangingColor)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
