using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Animator loginFormAnimator;
    [SerializeField] private GameObject loggedPanel;
    [SerializeField] private GameObject highScoresButton;
    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject highScoresTableMessage;
    [SerializeField] private GameObject aboutVersionField;

    // Start is called before the first frame update
    void Start()
    {
        // Use this for initialisation
        Debug.Log("MainMenuManager Start!");
        Debug.Log("Application Version : " + Application.version);

        // Set the version in the "About" screen
        aboutVersionField.GetComponent<TextMeshProUGUI>().text = "v" + Application.version + " - PREVIEW ONLY";

        // Checking for saved session
        var sessionToken = PlayerPrefs.GetString("session_token");
        var sessionUser = PlayerPrefs.GetString("username");
        if (sessionToken != "") {

            Debug.Log("tiene token");
            // ToDo: Check if the token is still valid!
            highScoresButton.SetActive(true);
            loginButton.GetComponent<Button>().interactable = true;
            loggedPanel.SetActive(true);
            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            GameObject.Find("LoggedUsername").GetComponent<TextMeshProUGUI>().SetText(sessionUser);

        }
        else {
            Debug.Log("no tiene token");
            highScoresButton.SetActive(false);
            loginButton.GetComponent<Button>().interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearHighScoresTable() {
        Debug.Log("Clearing HighScoresTable");

        var highscoreResults = GameObject.FindGameObjectsWithTag("HighScoreEntry");
        foreach(var entry in highscoreResults) {
            Destroy(entry);
        }
        highScoresTableMessage.SetActive(true);
    }

    public void StartNewGame() {
        Debug.Log("Starting new game");
        LevelLoader.Instance.LoadNextLevel();
    }

    public void ShowOrHideLoginForm() {
        loginFormAnimator.SetTrigger("ShowOrHide");
    }

    public void ExitGame() { 
        Application.Quit();
    }
}
