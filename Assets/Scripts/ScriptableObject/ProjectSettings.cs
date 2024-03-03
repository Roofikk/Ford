using UnityEngine;

[CreateAssetMenu(fileName = "ProjectSettings", menuName = "Settings/New Project Settings", order = 1)]
public class ProjectSettings : ScriptableObject
{
    [SerializeField] private string _hostWebApi;

    public string HostWebApi => _hostWebApi;
}
