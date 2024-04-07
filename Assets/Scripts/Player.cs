using Ford.SaveSystem;
using Ford.WebApi;
using Ford.WebApi.Data;
using System;
using System.Net;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Settings _settings;
    private static AccountDto _userData = null;
    public static AccountDto UserData => _userData;
    public static bool IsLoggedIn { get; private set; }
    public static event Action OnChangedAuthState;

    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse2)) && !UIHandler.IsMouseOnUI)
        {
            GameManager.Instance.LockTouchObjects();
            Cursor.lockState = CursorLockMode.Locked;
        }

        if ((Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Mouse2)) && !UIHandler.IsMouseOnUI)
        {
            GameManager.Instance.UnlockTouchObjects();
            Cursor.lockState = CursorLockMode.None;
        }

        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0f && !UIHandler.IsMouseOnUI)
        {
            MoveForward(Input.GetAxis("Mouse ScrollWheel"));
        }

        if (Input.GetKey(KeyCode.Mouse1) && !UIHandler.IsMouseOnUI)
        {
            MoveOnPlane();
        }
    }

    public static void Authorize(TokenDto tokens, Action onAuthorizeFinished = null)
    {
        using var storage = new TokenStorage();
        storage.SetNewAccessToken(tokens.Token);
        storage.SetNewRefreshToken(tokens.RefreshToken);

        Authorize(onAuthorizeFinished);
    }

    public static void Authorize(Action onAuthorizeFinished = null)
    {
        FordApiClient client = new();
        StorageSystem storage = new();

        storage.OnReadyStateChanged += (state) =>
        {
            OnChangedStorageState(state);
            onAuthorizeFinished?.Invoke();
        };

        using var tokenStorage = new TokenStorage();

        if (!string.IsNullOrEmpty(tokenStorage.GetAccessToken()))
        {
            client.GetAccountInfoAsync(tokenStorage.GetAccessToken()).RunOnMainThread(result =>
            {
                using var tokenStorage = new TokenStorage();
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        IsLoggedIn = true;
                        _userData = result.Content;

                        storage.ChangeState(StorageSystemStateEnum.Authorized);
                        break;
                    case HttpStatusCode.Unauthorized:
                        client.RefreshTokenAndReply(tokenStorage.GetAccessToken(), client.GetAccountInfoAsync)
                        .RunOnMainThread(result =>
                        {
                            switch (result.StatusCode)
                            {
                                case HttpStatusCode.OK:
                                    _userData = result.Content;
                                    IsLoggedIn = true;

                                    storage.ChangeState(StorageSystemStateEnum.Authorized);
                                    break;
                                default:
                                    _userData = null;
                                    IsLoggedIn = false;
                                    onAuthorizeFinished?.Invoke();
                                    break;
                            }
                        });
                        break;
                    default:
                        _userData = null;
                        IsLoggedIn = false;
                        onAuthorizeFinished?.Invoke();
                        break;
                }
            });
        }
        else
        {
            IsLoggedIn = false;
            onAuthorizeFinished?.Invoke();
        }
    }

    public static void Logout()
    {
        _userData = null;
        IsLoggedIn = false;

        using var tokenStorage = new TokenStorage();
        tokenStorage.SetNewAccessToken("");
        tokenStorage.SetNewRefreshToken("");

        var storage = new StorageSystem();
        storage.OnReadyStateChanged += (state) => { OnChangedStorageState(state, OnChangedAuthState); };
        storage.ChangeState(StorageSystemStateEnum.Offline);

        //OnChangedAuthState?.Invoke();
    }

    private static void OnChangedStorageState(StorageSystemStateEnum state, Action onStateChanged = null)
    {
        if (state != StorageSystemStateEnum.Authorized)
        {
            onStateChanged?.Invoke();
            return;
        }

        StorageSystem storage = new();

        if (storage.History.History.Count == 0)
        {
            storage.ApplyTransition().RunOnMainThread(result =>
            {
                onStateChanged?.Invoke();
            });
            return;
        }

        PageManager.Instance.OpenWarningPage(new WarningData(
            "Предупреждение",
            "У вас имеются некоторые изменения, пока вы находились вне сети.\n" +
            "Желаете посмотреть и применить их к уже имеющимся?\n" +
            "ОТМЕНА приведет к их уничтожению",
            () =>
            {
                PageManager.Instance.OpenPage(PageManager.Instance.HistoryPage, new HistoryPageParam(storage.History), 2);
            },
            onCancel: () =>
            {
                storage.RawApplyTransition().RunOnMainThread(result =>
                {
                    onStateChanged?.Invoke();
                });
            }
        ), 2);

        //onStateChanged?.Invoke();
    }

    public static void UpdateUserInfo(AccountDto data)
    {
        _userData = data;
    }

    private void MoveOnPlane()
    {
        float mouseVx = Input.GetAxis("Mouse X");
        float mouseVy = Input.GetAxis("Mouse Y");
        Vector3 direction = new(mouseVx, mouseVy);

        if (_settings.InverseMovementPlayer)
            direction = -direction;

        //To check wall
        Vector3 v = (transform.right * mouseVx + transform.up * mouseVy) * _settings.SensetivityMovementPlayer;

        //To movement
        Vector3 dt = _settings.SensetivityMovementPlayer * direction;
        if (!CheckWall(v, v.magnitude * 3f))
            transform.Translate(dt);
    }

    private void MoveForward(float direction)
    {
        Vector3 ds = _settings.SensetivityScrollPlayer * direction * transform.forward;
        if (!CheckWall(ds, ds.magnitude * 3f))
            transform.Translate(ds, Space.World);
    }

    private bool CheckWall(Vector3 direction, float maxDistanceRaycast)
    {        
        Ray ray = new (transform.position, direction);
        int mask = 1 << LayerMask.NameToLayer("Wall");

        return Physics.Raycast(ray, out RaycastHit hit, maxDistanceRaycast, mask);
    }
}
