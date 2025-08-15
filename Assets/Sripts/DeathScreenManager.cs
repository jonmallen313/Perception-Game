using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    // call "FindObjectOfType<DeathScreenManager>().ShowDeathScreen();" to show the death screen

    public GameObject deathScreenUI;
    public TMPro.TextMeshProUGUI quoteText1;
    public TMPro.TextMeshProUGUI quoteText2;
    public string[] deathQuotes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowDeathScreen()
    {
        deathScreenUI.SetActive(true);
        Time.timeScale = 0f;
        quoteText1.text = deathQuotes[Random.Range(0, deathQuotes.Length)];
        quoteText2.text = quoteText1.text;

    }



    public void RestartLevel()
    {
        Time.timeScale = 1f;
        deathScreenUI.SetActive(false);
        Debug.Log("Restarting Level");
        deathScreenUI.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        deathScreenUI.SetActive(false);

        Debug.Log("Back to Main Menu");
        deathScreenUI.SetActive(false);
        SceneManager.LoadScene("MainMenu");

    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
