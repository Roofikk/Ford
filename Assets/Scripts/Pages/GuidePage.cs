using UnityEngine;
using UnityEngine.UI;

public class GuidePage : Page
{
    [SerializeField] private ScrollRect _scrollView;

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        if (_scrollView == null)
            _scrollView = GetComponent<ScrollRect>();

        _scrollView.verticalScrollbar.value = 1f;
    }
}