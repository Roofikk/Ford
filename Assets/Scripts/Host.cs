using UnityEngine;

[CreateAssetMenu(fileName = "NewHost", menuName = "Host", order = 1)]
public class Host : ScriptableObject
{
    [SerializeField] private string _hostConnection;
    public string HostConnection => _hostConnection;
}
