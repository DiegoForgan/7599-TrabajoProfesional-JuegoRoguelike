using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : ToMainMenuUI
{
   
   public GameObject _GameOverUI;

   public static GameOverMenu instance;

    private void Awake() {
        // Singleton implementation
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        // Maintain this object through all the life of the game
        DontDestroyOnLoad(gameObject);
    }
    public void TryAgain(){
        Time.timeScale = 1f;
        DestroyAllPreservedInstances();
        _GameOverUI.SetActive(false);
        //Goes back to the main menu to start again
        SceneManager.LoadScene(0);
        //This command stops the desired sound clip
        FindObjectOfType<AudioManager>().StopSound("GameOverTheme");
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound("MainMenuTheme");
    }

    public void QuitGame(){
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
