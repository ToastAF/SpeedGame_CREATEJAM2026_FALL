using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagement : MonoBehaviour
{


    void Start()
    {

    }

    void Update()
    {
        
    }

    public void PlayScene()
    {
        SceneManager.LoadScene("The Scene");
    }

    public void Restart()
    {
        SceneManager.LoadScene("The Scene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
