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
        TokenStorage storage = new();
        storage.SetNewAccessToken(tokens.Token);
        storage.SetNewRefreshToken(tokens.RefreshToken);

        Authorize(tokens.Token, onAuthorizeFinished);
    }

    public static void Authorize(string accessToken = "", Action onAuthorizeFinished = null)
    {
        FordApiClient client = new();

        if (string.IsNullOrEmpty(accessToken))
        {
            TokenStorage storage = new();
            accessToken = storage.GetAccessToken().ToString();
        }

        if (!string.IsNullOrEmpty(accessToken))
        {
            client.GetUserInfoAsync(accessToken).RunOnMainThread(result =>
            {
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        IsLoggedIn = true;
                        _userData = result.Content;
                        onAuthorizeFinished?.Invoke();
                        OnChangedAuthState?.Invoke();
                        break;
                    case HttpStatusCode.Unauthorized:
                        client.RefreshTokenAndReply(accessToken, client.GetUserInfoAsync)
                        .RunOnMainThread(result =>
                        {
                            if (result.Content != null)
                            {
                                _userData = result.Content;
                                IsLoggedIn = true;
                            }
                            else
                            {
                                _userData = null;
                                IsLoggedIn = false;
                            }
                            onAuthorizeFinished?.Invoke();
                            OnChangedAuthState?.Invoke();
                        });
                        break;
                    default:
                        _userData = null;
                        IsLoggedIn = false;
                        onAuthorizeFinished?.Invoke();
                        OnChangedAuthState?.Invoke();
                        break;
                }
            });
        }
        else
        {
            IsLoggedIn = false;
            onAuthorizeFinished?.Invoke();
            OnChangedAuthState?.Invoke();
        }
    }

    public static void Logout()
    {
        _userData = null;
        IsLoggedIn = false;

        TokenStorage storage = new();
        storage.SetNewAccessToken("");
        storage.SetNewRefreshToken("");

        OnChangedAuthState?.Invoke();
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
