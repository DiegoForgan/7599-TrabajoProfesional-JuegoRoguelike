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
    [SerializeField] private GameObject highScoresEntryTemplate;
    [SerializeField] private GameObject aboutVersionField;

    // Start is called before the first frame update
    void Start()
    {
        // Use this for initialisation
        Debug.Log("MainMenuManager Start!");
        Debug.Log("Application Version : " + Application.version);

        // Set the version in the "About" screen
        aboutVersionField.GetComponent<TextMeshProUGUI>().text = "v" + Application.version + " - PREVIEW ONLY";

        // Loading settings from saved values
        // Assigns defaults if not present
        SettingsManager.InitializeSettings();

        // Loading session from saved values
        // Assigns defaults if not present
        SessionManager.InitializeSession();
        // Checking for saved session
        if (SessionManager.IsUserLoggedIn()) {

            Debug.Log("tiene token");
            // ToDo: Check if the token is still valid!
            highScoresButton.SetActive(true);
            loginButton.SetActive(false);
            loggedPanel.SetActive(true);
            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            GameObject.Find("LoggedUsername").GetComponent<TextMeshProUGUI>().SetText(SessionManager.GetSessionUsername());

        }
        else {
            Debug.Log("no tiene token");
            highScoresButton.SetActive(false);
            loginButton.SetActive(true);
        }

        // Hide the highscores entry template from the table
        highScoresEntryTemplate.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearHighScoresTable() {
        Debug.Log("Clearing HighScoresTable");

        highScoresTableMessage.GetComponent<TextMeshProUGUI>().text = "Loading Highscores, please wait...";
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
        // Save session data before exiting the application
        SessionManager.PersistSession();
        Application.Quit();
    }
}
