using Ford.SaveSystem;
using UnityEngine;

public class TestSaveSystem : MonoBehaviour
{
    [SerializeField] private StartProjectObject _startProjectObject;

    void Start()
    {
        StartProjectObject.OnProjectStarted += OnStarted;
    }

    private void OnStarted()
    {
        StorageSystem storage = new();

        storage.GetHorses((result) =>
        {
            Debug.Log("Retrieve horses");
        });
    }
}
