using Ford.SaveSystem;
using Ford.WebApi;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class StartProjectObject : MonoBehaviour
{
    [SerializeField] private Page _loadingPage;
    [SerializeField] private Host _host;

    public static event Action OnProjectStarted;
    public static bool ProjectStarted { get; private set; } = false;

    private bool _playerAuthorizeFinished = false;

    void Start()
    {
        if (!ProjectStarted)
        {
            _loadingPage.Open(5);

            FordApiClient.SetHost(_host.HostConnection);
            StorageSystem.Initiate(StorageSystemStateEnum.Offline);

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
