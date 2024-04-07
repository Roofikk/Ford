using Ford.SaveSystem;
using Ford.WebApi;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class StartProjectObject : MonoBehaviour
{
    [SerializeField] private Page _loadingPage;
    [SerializeField] private string _hostJsonFilePath;

    public static event Action OnProjectStarted;
    public static bool ProjectStarted { get; private set; } = false;

    private bool _playerAuthorizeFinished = false;

    void Start()
    {
        if (!ProjectStarted)
        {
            _loadingPage.Open(5);

            StreamReader sr = new StreamReader(_hostJsonFilePath);
            string json = sr.ReadToEnd();
            var host = JsonConvert.DeserializeObject<Host>(json);

            FordApiClient.SetHost(host.HostConnection);
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
