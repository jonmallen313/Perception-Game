using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame() => SceneManager.LoadScene("NewTest");

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
