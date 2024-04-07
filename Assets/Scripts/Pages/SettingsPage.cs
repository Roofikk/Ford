using Ford.SaveSystem.Ver2;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPage : Page
{
    [SerializeField] private Settings _settings;

    [Space(10)]
    [SerializeField] private TMP_InputField _pathSaveText;
    [SerializeField] private Button _showInFolderButton;

    [SerializeField] private Toggle _inverseMovementPlayerToggle;
    [SerializeField] private Toggle _inverseRotationCameraToggle;

    [SerializeField] private Slider _sensetivityMovementPlayerSlider;
    [SerializeField] private TextMeshProUGUI _sensetivityMovementPlayerText;

    [SerializeField] private Slider _sensetivityRotationCameraSlider;
    [SerializeField] private TextMeshProUGUI _sensetivityRotationCameraText;

    [SerializeField] private Slider _sensetivityScrollPlayerSlider;
    [SerializeField] private TextMeshProUGUI _sensetivityScrollPlayerText;

    public void Start()
    {
        _showInFolderButton.onClick.AddListener(ShowSavePathFolder);
        _showInFolderButton.onClick.AddListener(_showInFolderButton.Select);
    }

    public override void Open(int popUpLevel = 0)
    {
        base.Open(popUpLevel);

        //Initialize value
        //Path save
        var storage = new Storage();
        _pathSaveText.text = Path.GetFullPath(storage.StoragePath);

        //Inverse
        _inverseMovementPlayerToggle.isOn = _settings.InverseMovementPlayer;
        _inverseRotationCameraToggle.isOn = _settings.InverseRotationCamera;

        //Sensetivity
        _sensetivityMovementPlayerSlider.value = _settings.SensetivityMovementPlayer;
        _sensetivityMovementPlayerText.text = _sensetivityMovementPlayerSlider.value.ToString();

        _sensetivityRotationCameraSlider.value = _settings.SensetivityRotationCamera;
        _sensetivityRotationCameraText.text = _sensetivityRotationCameraSlider.value.ToString();

        _sensetivityScrollPlayerSlider.value = _settings.SensetivityScrollPlayer;
        _sensetivityScrollPlayerText.text = _sensetivityScrollPlayerSlider.value.ToString();

        //Initialize UI elemets
        //Movement
        int countStep = Mathf.CeilToInt(_sensetivityMovementPlayerSlider.maxValue);
        float step = (_settings.MaxValueSensetivityMovementPlayer - _settings.MinValueSensetivityMovementPlayer) / countStep;
        _sensetivityMovementPlayerSlider.value = _settings.SensetivityMovementPlayer / step;

        //Rotation
        countStep = Mathf.CeilToInt(_sensetivityRotationCameraSlider.maxValue);
        step = (_settings.MaxValueSensetivityRotationCamera - _settings.MinValueSensetivityRotationCamera) / countStep;
        _sensetivityRotationCameraSlider.value = _settings.SensetivityRotationCamera / step;

        //Scroll
        countStep = Mathf.CeilToInt(_sensetivityScrollPlayerSlider.maxValue);
        step = (_settings.MaxValueSensetivityScrollPlayer - _settings.MinValueSensetivityScrollPlayer) / countStep;
        _sensetivityScrollPlayerSlider.value = _settings.SensetivityScrollPlayer / step;

        //Events
        _inverseMovementPlayerToggle.onValueChanged.AddListener(OnChangedInverseMovement);
        _inverseRotationCameraToggle.onValueChanged.AddListener(OnChangedInverseRotation);
        _sensetivityMovementPlayerSlider.onValueChanged.AddListener(OnChangedMovement);
        _sensetivityRotationCameraSlider.onValueChanged.AddListener(OnChangedRotation);
        _sensetivityScrollPlayerSlider.onValueChanged.AddListener(OnChangedScroll);
    }

    private void OnDestroy()
    {
        Close();

        _showInFolderButton.onClick.RemoveListener(ShowSavePathFolder);
    }

    public override void Close()
    {
        base.Close();

        _inverseMovementPlayerToggle.onValueChanged.RemoveListener(OnChangedInverseMovement);
        _inverseRotationCameraToggle.onValueChanged.RemoveListener(OnChangedInverseRotation);
        _sensetivityMovementPlayerSlider.onValueChanged.RemoveListener(OnChangedMovement);
        _sensetivityRotationCameraSlider.onValueChanged.RemoveListener(OnChangedRotation);
        _sensetivityScrollPlayerSlider.onValueChanged.RemoveListener(OnChangedScroll);
    }

    private void OnChangedInverseMovement(bool value)
    {
        _settings.InverseMovementPlayer = value;
    }

    private void OnChangedInverseRotation(bool value)
    {
        _settings.InverseRotationCamera = value;
    }

    private void OnChangedMovement(float sliderValue)
    {
        int countStep = Mathf.CeilToInt(_sensetivityMovementPlayerSlider.maxValue - _sensetivityMovementPlayerSlider.minValue);
        float stepLength = (_settings.MaxValueSensetivityMovementPlayer - _settings.MinValueSensetivityMovementPlayer) / countStep;
        float value = _settings.MinValueSensetivityMovementPlayer + stepLength * sliderValue;

        _sensetivityMovementPlayerText.text = string.Format("{0:0.##}", value);

        _settings.SensetivityMovementPlayer = value;
    }

    private void OnChangedRotation(float sliderValue)
    {
        int countStep = Mathf.CeilToInt(_sensetivityRotationCameraSlider.maxValue);
        float stepLength = (_settings.MaxValueSensetivityRotationCamera - _settings.MinValueSensetivityRotationCamera) / countStep;
        float value = _settings.MinValueSensetivityRotationCamera + stepLength * sliderValue;

        _sensetivityRotationCameraText.text = string.Format("{0:0.##}", value);

        _settings.SensetivityRotationCamera = value;
    }

    private void OnChangedScroll(float sliderValue)
    {
        int countStep = Mathf.CeilToInt(_sensetivityScrollPlayerSlider.maxValue);
        float stepLength = (_settings.MaxValueSensetivityScrollPlayer - _settings.MinValueSensetivityScrollPlayer) / countStep;
        float value = _settings.MinValueSensetivityScrollPlayer + stepLength * sliderValue;

        _sensetivityScrollPlayerText.text = string.Format("{0:0.##}", value);

        _settings.SensetivityScrollPlayer = value;
    }

    public void ShowSavePathFolder()
    {
        var storage = new Storage();
        string path = Path.GetFullPath(storage.StoragePath);
        Application.OpenURL(@"file://" + path);
    }

//    private void OpenSelectFolderWithExplorer()
//    {
//#if PLATFORM_STANDALONE_WIN
//        BrowserProperties bp = new BrowserProperties("Выберете папку");
//        bp.filter = "txt files (*.txt)|*.txt|All Files (*.*)|*.*";
//        bp.filterIndex = 0;

//        FileBrowser fileBrowser = new FileBrowser();
//        fileBrowser.OpenFolderBrowser(bp, path =>
//        {
//            if (path is not null)
//            {
//                PageManager.Instance.OpenPage(_warningPage, new WarningData(
//                    "Внимание",
//                    "Перенести сохранения с предыдущей папки?\r\nЕсли отмените, то сохранения с предыдущей папки удалятся",
//                    () => { TransferSaves(_settings.PathSave, path); },
//                    () => { CancelTransferSave(_settings.PathSave, path); },
//                    CancelChangePathSave), 1);
//            }
//            else
//            {
//                Debug.Log("Path has not choosen");
//            }
//        });
//#endif
//    }

    //private void TransferSaves(string pathFrom, string pathTarget)
    //{
    //    if (CheckRequiredSpace(pathFrom, pathTarget))
    //    {
    //        if (CheckEmptyFolder(pathTarget))
    //        {
    //            DirectoryInfo sourceDir = new(pathFrom);
    //            DirectoryInfo targetDir = new(pathTarget);

    //            CopyFilesRecursively(sourceDir, targetDir);
    //            DeleteSaves(sourceDir);
    //            SetSavePath(pathTarget);
    //        }
    //        else
    //        {
    //            // delete
    //        }
    //    }
    //    else
    //    {
    //        // delete
    //    }
    //}

    //private void CancelTransferSave(string sourcePath, string targetPath)
    //{
    //    DirectoryInfo directoryInfo = new DirectoryInfo(sourcePath);
    //    DeleteSaves(directoryInfo);
    //    SetSavePath(targetPath);
    //}

    //private void DeleteSaves(DirectoryInfo sourceDir)
    //{
    //    StartCoroutine(DeleteSavesCoroutine(sourceDir));
    //}

    //private IEnumerator DeleteSavesCoroutine(DirectoryInfo sourceDir)
    //{
    //    yield return new WaitForSecondsRealtime(2f);
    //    sourceDir.Delete(true);
    //}

    //private static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
    //{
    //    foreach (DirectoryInfo dir in source.GetDirectories())
    //        CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));

    //    foreach (System.IO.FileInfo file in source.GetFiles())
    //        file.CopyTo(Path.Combine(target.FullName, file.Name));
    //}

    //private static bool CheckRequiredSpace(string pathFrom, string pathTarget)
    //{
    //    DirectoryInfo dirOld = new DirectoryInfo(pathFrom);
    //    long sizeOldFolder = DirSize(dirOld);
    //    long sizeNewDisk = -1;

    //    string pathDisk = pathTarget.Split('/')[0];

    //    foreach (DriveInfo drive in DriveInfo.GetDrives())
    //    {
    //        if (drive.IsReady && drive.Name == pathDisk + "\\")
    //        {
    //            sizeNewDisk = drive.AvailableFreeSpace;
    //            break;
    //        }
    //    }

    //    if (sizeNewDisk < 0)
    //    {
    //        Debug.LogError("Диск не был найден");
    //        return false;
    //    }

    //    return sizeNewDisk > sizeOldFolder;
    //}

    //private static long DirSize(DirectoryInfo d)
    //{
    //    long size = 0;

    //    // Add file sizes.
    //    FileInfo[] fis = d.GetFiles();

    //    foreach (FileInfo fi in fis)
    //    {
    //        size += fi.Length;
    //    }

    //    // Add subdirectory sizes.
    //    DirectoryInfo[] dis = d.GetDirectories();

    //    foreach (DirectoryInfo di in dis)
    //    {
    //        size += DirSize(di);
    //    }

    //    return size;
    //}

    //private bool CheckEmptyFolder(string path)
    //{
    //    DirectoryInfo dir = new DirectoryInfo(path);
    //    return dir.GetFileSystemInfos().Length == 0 && dir.GetDirectories().Length == 0;
    //}

    //private void CancelChangePathSave()
    //{
    //    ToastMessage.Show("Путь сохранения не изменен");
    //}

    //private void SetSavePath(string path)
    //{
    //    _settings.PathSave = path;
    //    _pathSaveText.text = _settings.PathSave;

    //    ToastMessage.Show("Путь сохранения изменен");
    //}
}
