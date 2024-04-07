using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Settings/Create")]
public class Settings : ScriptableObject
{    
    [SerializeField] private bool _inverseMovementPlayer;
    [SerializeField] private bool _inverseRotationCamera;

    [Space(10)]
    [Header("Movement settings")]
    [SerializeField] private float _minValueSensetivityMovementPlayer = 0.5f;
    [SerializeField] private float _maxValueSensetivityMovementPlayer = 10f;
    [SerializeField] private float _sensetivityMovementPlayer = 4f;

    [Space(10)]
    [Header("Rotation settings")]
    [SerializeField] private float _minValueSensetivityRotationCamera = 0.1f;
    [SerializeField] private float _maxValueSensetivityRotationCamera = 5f;
    [SerializeField] private float _sensetivityRotationCamera = 2f;

    [Space(10)]
    [Header("Scroll settings")]
    [SerializeField] private float _minValueSensetivityScrollPlayer = 5f;
    [SerializeField] private float _maxValueSensetivityScrollPlayer = 100f;
    [SerializeField] private float _sensetivityScrollPlayer = 40f;

    private readonly string PATH_SAVE = "PATH_SAVE";
    private readonly string INVERSE_MOVEMENT = "INVERSE_MOVEMENT";
    private readonly string INVERSE_ROTATION = "INVERSE_ROTATION";
    private readonly string SENSETIVITY_MOVEMENT = "SENSETIVITY_MOVEMENT";
    private readonly string SENSETIVITY_ROTATION = "SENSETIVITY_ROTATION";
    private readonly string SENSETIVITY_SCROLL = "SENSETIVITY_SCROLL";

    public bool InverseMovementPlayer { get { return _inverseMovementPlayer; } 
        set
        {
            _inverseMovementPlayer = value;
        }
    }
    public bool InverseRotationCamera
    {
        get { return _inverseRotationCamera; }
        set
        {
            _inverseRotationCamera = value;
            OnInverseRotationToggleChanged?.Invoke(value);
        }
    }

    public float MinValueSensetivityMovementPlayer { get { return _minValueSensetivityMovementPlayer; } }
    public float MaxValueSensetivityMovementPlayer { get { return _maxValueSensetivityMovementPlayer; } }
    public float SensetivityMovementPlayer { get { return _sensetivityMovementPlayer; }
        set
        {
            if (value >= MinValueSensetivityMovementPlayer && value <= MaxValueSensetivityMovementPlayer)
                _sensetivityMovementPlayer = value;
            else if (value < MinValueSensetivityMovementPlayer)
                _sensetivityMovementPlayer = MinValueSensetivityMovementPlayer;
            else
                _sensetivityMovementPlayer = MaxValueSensetivityMovementPlayer;
        } 
    }

    public float MinValueSensetivityRotationCamera { get { return _minValueSensetivityRotationCamera; } }
    public float MaxValueSensetivityRotationCamera { get { return _maxValueSensetivityRotationCamera; } }
    public float SensetivityRotationCamera { get { return _sensetivityRotationCamera; } 
        set
        {
            if (value >= MinValueSensetivityRotationCamera && value <= MaxValueSensetivityRotationCamera)
                _sensetivityRotationCamera = value;
            else if (value < MinValueSensetivityRotationCamera)
                _sensetivityRotationCamera = MinValueSensetivityRotationCamera;
            else
                _sensetivityRotationCamera = MaxValueSensetivityRotationCamera;
        } 
    }

    public float MinValueSensetivityScrollPlayer { get { return _minValueSensetivityScrollPlayer; } }
    public float MaxValueSensetivityScrollPlayer { get { return _maxValueSensetivityScrollPlayer; } }
    public float SensetivityScrollPlayer { get { return _sensetivityScrollPlayer;} 
        set
        {
            if (value >= MinValueSensetivityScrollPlayer && value <= MaxValueSensetivityScrollPlayer)
                _sensetivityScrollPlayer = value;
            else if (value < MinValueSensetivityScrollPlayer)
                _sensetivityScrollPlayer = MinValueSensetivityScrollPlayer;
            else
                _sensetivityScrollPlayer = MaxValueSensetivityScrollPlayer;
        }
    }

    public event Action<bool> OnInverseRotationToggleChanged;

    internal void Initiate()
    {
        InverseMovementPlayer = Convert.ToBoolean(PlayerPrefs.GetString(INVERSE_MOVEMENT, InverseMovementPlayer.ToString()));
        InverseRotationCamera = Convert.ToBoolean(PlayerPrefs.GetString(INVERSE_ROTATION, InverseRotationCamera.ToString()));
        SensetivityMovementPlayer = PlayerPrefs.GetFloat(SENSETIVITY_MOVEMENT, SensetivityMovementPlayer);
        SensetivityRotationCamera = PlayerPrefs.GetFloat(SENSETIVITY_ROTATION, SensetivityRotationCamera);
        SensetivityScrollPlayer = PlayerPrefs.GetFloat(SENSETIVITY_SCROLL, SensetivityScrollPlayer);
    }

    internal void Save()
    {
        PlayerPrefs.SetString(INVERSE_MOVEMENT, InverseMovementPlayer.ToString());
        PlayerPrefs.SetString(INVERSE_ROTATION, InverseRotationCamera.ToString());
        PlayerPrefs.SetFloat(SENSETIVITY_MOVEMENT, SensetivityMovementPlayer);
        PlayerPrefs.SetFloat(SENSETIVITY_ROTATION, SensetivityRotationCamera);
        PlayerPrefs.SetFloat(SENSETIVITY_SCROLL, SensetivityScrollPlayer);
    }
}
