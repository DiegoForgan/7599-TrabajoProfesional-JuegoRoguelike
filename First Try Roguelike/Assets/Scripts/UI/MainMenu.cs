using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
     public void ContinueGame()
    {
        //TO DO: implement save and load system
        Debug.Log("Game should be loading but still not implemented");    
    }
    
    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().StopSound("MainMenuTheme");
        LevelLoader.Instance.LoadNextLevel();
    }

    public void ShowHowToPlay(){
        Debug.Log("Decide if this shows image on input or loads tutorial scene");
    }

    public void ShowHighScores(){
        Debug.Log("Showing HighScores...");
    }

   

    public void QuitGame()
    {
        Application.Quit();
    }
}
