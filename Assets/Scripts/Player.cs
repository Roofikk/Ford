using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Settings _settings;

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
