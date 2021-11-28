using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().StopSound("MainMenuTheme");
        if(GameManager.Instance != null) GameManager.Instance.StartNewGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        //TO DO: implement save and load system
        Debug.Log("Game should be loading but still not implemented");    
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
