using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : ToMainMenuUI
{
    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;
    }

    public void LoadMainMenu(){
        // Restore normal flow of time
        Time.timeScale = 1f;
        GameIsPaused = false;
        // When returning to main menu, all preserved instances must be destroyed
        DestroyAllPreservedInstances();
        PauseMenuUI.SetActive(false);
        //Scene one is our main menu scene
        SceneManager.LoadScene(1);
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound("MainMenuTheme");
    }

    public void QuitGame(){
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
