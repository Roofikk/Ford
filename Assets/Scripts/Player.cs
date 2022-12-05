using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Skeleton _ford;
    [SerializeField] private bool _inverseMovement = true;

    [Range(100f, 400f)]
    [SerializeField] private float _sensetivityMovement = 200f;

    [Range(1000f, 4000f)]
    [SerializeField] private float _sensetivityScroll = 2000f;

    private string _name;
    private string _city;
    private DateTime _dateBegin;
    private DateTime _dateNow;

    public static Player Instance { get; private set; }
    public string Name { get { return _name; } }
    public string City { get { return _city; } }
    public DateTime DateBegin { get { return _dateBegin; } }
    public DateTime DateNow { get { return _dateNow; } }

    public Player(string name, string city, DateTime dateBegin)
    {
        _name = name;
        _city = city;
        _dateBegin = dateBegin;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        //Player player = SceneManagerWithParams.GetParam<Player>();

        //if (player != null)
        //{
        //    _name = player.Name;
        //    _city = player.City;
        //    _dateBegin = DateBegin;

        //    Debug.Log(player.Name);
        //    Debug.Log(player.City);
        //    Debug.Log(player.DateBegin);
        //}
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
    }

    private void FixedUpdate()
    {
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

        if (_inverseMovement)
            direction = -direction;

        transform.Translate(direction * _sensetivityMovement * Time.deltaTime);
    }

    private void MoveForward(float power)
    {
        transform.Translate(transform.forward * power * _sensetivityScroll * Time.deltaTime, Space.World);
    }
}
