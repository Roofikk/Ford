using Ford.WebApi;
using System;
using UnityEngine;

public class StartProjectObject : MonoBehaviour
{
    [SerializeField] private Page _loadingPage;
    [SerializeField] private ProjectSettings _projectSettings;

    public static event Action OnProjectStarted;
    public static bool ProjectStarted { get; private set; } = false;

    private bool _playerAuthorizeFinished = false;

    void Start()
    {
        // Show loading
        _loadingPage.Open(5);
        FordApiClient.SetHost(_projectSettings.HostWebApi);

        if (!ProjectStarted)
        {
            Player.Authorize(onAuthorizeFinished: () =>
            {
                _playerAuthorizeFinished = true;
                CheckStartProject();
            });
        }
    }

    private void CheckStartProject()
    {
        if (_playerAuthorizeFinished)
        {
            ProjectStarted = true;
            _loadingPage.Close();
            OnProjectStarted?.Invoke();
        }
    }
}
