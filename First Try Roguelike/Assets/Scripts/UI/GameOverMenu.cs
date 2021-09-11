using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
   public static bool IsPlayerDead = false;

   public GameObject _GameOverUI;


    // Update is called once per frame
    void Update()
    {
       if (IsPlayerDead)
        {
            _GameOverUI.SetActive(true);
            Time.timeScale = 0f;
        }  
    }

    public void TryAgain(){
        Time.timeScale = 1f;
        IsPlayerDead = false;
        _GameOverUI.SetActive(false);
        //Goes back to the main menu to start again
        SceneManager.LoadScene(0);
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().StopSound("GameOverTheme");
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound("MainMenuTheme");
    }

    public void QuitGame(){
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
