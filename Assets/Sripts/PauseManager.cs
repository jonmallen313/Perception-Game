using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        Debug.Log("PauseManager Awake in: " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("pauseMenuUI is: " + pauseMenuUI);
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f; 
    }

    public void Resume()
    {
        
        Debug.Log("Resume called on: " + gameObject.name);
        Debug.Log("pauseMenuUI is: " + pauseMenuUI);
        TogglePause();
    }

    public void RestartLevel()
    {
        TogglePause();
        Debug.Log("Restarting Level");
        pauseMenuUI.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void BackToMenu()
    {
        TogglePause();
        Debug.Log("Back to Main Menu");
        pauseMenuUI.SetActive(false);
        SceneManager.LoadScene("MainMenu");

    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
