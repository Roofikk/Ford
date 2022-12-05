using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private void Start()
    {

    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadIdScene(int idScene)
    {
        SceneManager.LoadScene(idScene);
    }

    public void LoadIdSceneWithPath(string path)
    {
        SceneManager.LoadScene(1);
    }
}
