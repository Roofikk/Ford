using UnityEngine;
using UnityEngine.UI;

public class LoadScenePage : Page
{
    [SerializeField] private Slider _sliderLoadProgress;
    public AsyncOperation LoadingOperation { get; set; }

    public void Open(AsyncOperation loadingOperation, int popUpLevel = 1)
    {
        LoadingOperation = loadingOperation;

        Open(popUpLevel);
    }

    public override void Close()
    {
        base.Close();

        LoadingOperation = null;
    }

    private void Update()
    {
        if (LoadingOperation != null)
            _sliderLoadProgress.value = Mathf.Clamp01(LoadingOperation.progress / 0.9f);
    }
}
