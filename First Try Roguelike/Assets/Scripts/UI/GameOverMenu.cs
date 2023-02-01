using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
   
   public GameObject _GameOverUI;

    public void TryAgain(){
        Time.timeScale = 1f;
        _GameOverUI.SetActive(false);
        //Goes back to the main menu to start again
        LevelLoader.Instance.LoadSceneByIndex(1);
        //This command stops the desired sound clip
        FindObjectOfType<AudioManager>().StopSound("GameOverTheme");
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound("MainMenuTheme");
    }

    public void QuitGame(){
        Debug.Log("Quitting Game...");
        // Saves user settings
        SettingsManager.PersistSettings();
        // Saves session data
        SessionManager.PersistSession();
        Application.Quit();
    }
}
