using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    private static bool initDone = false;
    private bool loginPanelWasShowing = false;
    private AudioManager audioManager;
    [SerializeField] private APIRequestHandler apiRequestHandler;
    [SerializeField] private Animator loginFormAnimator;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject highScoresButton;
    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject loggedPanel;
    [SerializeField] private GameObject highScoresTableMessage;
    [SerializeField] private GameObject highScoresEntryTemplate;
    [SerializeField] private GameObject devSettingsPanel;
    [SerializeField] private GameObject aboutVersionField;

    // Start is called before the first frame update
    void Start()
    {
        // Use this for initialisation
        Debug.Log("MainMenuManager Start!");
        Debug.Log("Application Version : " + Application.version);

        // Set the version in the "About" screen
        aboutVersionField.GetComponent<TextMeshProUGUI>().text = "v" + Application.version + " - PREVIEW ONLY";

        // We need to initialize managers only the scene loads for the first time
        // This is done using a static property
        if (!initDone) {
            // Loading settings from saved values
            // Assigns defaults if not present
            SettingsManager.InitializeSettings();

            // Loading session from saved values
            // Assigns defaults if not present
            SessionManager.InitializeSession();

            // Loading gameprogress from saved values
            // Assigns defaults if not present
            GameProgressManager.InitializeGameProgress();

            // Mark game as initialized
            initDone = true;
        }

        // Add a reference to the AudioManager to MainMenuManager
        // I need to do this since we are destroying the previoud object when the scene loads
        audioManager = FindObjectOfType(typeof(AudioManager)) as AudioManager;
        // Set initial audio volume
        // Every time the scene loads, as the object is destroyed and then re-instanced!
        audioManager.UpdateVolume((float)SettingsManager.GetSoundVolume());

        // Checking for saved session
        if (SessionManager.IsUserLoggedIn()) {
            Debug.Log("Session token found");

            // Getting data from the UI
            Transform loggedInPanelContainer = loggedPanel.gameObject.transform.Find("LoggedInPanelContainer");
            Transform loggedInMessageContainer = loggedPanel.gameObject.transform.Find("LoggedInMessageContainer");
            Transform loggedInMessage = loggedInMessageContainer.Find("LoggedInMessage");
            Transform loggedInSpinner = loggedInMessageContainer.Find("LoggedInSpinner");
            Transform loggedInCloseButton = loggedInMessageContainer.Find("LoggedInCloseButton");

            // Setting up logged in panel
            loggedPanel.SetActive(true);
            loggedInPanelContainer.gameObject.SetActive(false);
            loggedInMessage.GetComponent<TMP_Text>().text = "Validating your session\nplease wait...";
            loggedInMessage.gameObject.SetActive(true);
            loggedInSpinner.gameObject.SetActive(true);
            loggedInCloseButton.gameObject.SetActive(false);
            loggedInMessageContainer.gameObject.SetActive(true);

            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");

            // Check the session
            apiRequestHandler.CheckValidSession();
        }
        else {
            Debug.Log("No session token found");
            highScoresButton.SetActive(false);
            loginButton.SetActive(true);
        }

        // Set interactability of "Continue" button
        continueButton.GetComponent<Button>().interactable = GameProgressManager.PlayerCanContinue();
    }

    // Resets the highscores table
    // Clears results, and writes initial loading message
    public void ClearHighScoresTable() {

        // Hide the highscores entry template from the table
        highScoresEntryTemplate.SetActive(false);
        // Reset initial table message
        Transform highScoresTableMessageText = highScoresTableMessage.gameObject.transform.Find("HighscoresTableMessageText");
        Transform highScoresTableMessageSpinner = highScoresTableMessage.gameObject.transform.Find("HighscoresTableSpinner");
        highScoresTableMessageText.GetComponent<TMP_Text>().text = "Loading Highscores, please wait...";
        // Delete old entries
        var highscoreResults = GameObject.FindGameObjectsWithTag("HighScoreEntry");
        foreach(var entry in highscoreResults) {
            Destroy(entry);
        }
        // Enable table
        highScoresTableMessage.SetActive(true);
        highScoresTableMessageText.gameObject.SetActive(true);
        highScoresTableMessageSpinner.gameObject.SetActive(true);
    }

    // Starts a new game!
    // Loads first cinematic scene
    public void StartNewGame() {
        Debug.Log("Starting new game");
        LevelLoader.Instance.LoadNextScene();
    }

    // Toggles the log in panel in the main menu screen
    public void ShowOrHideLoginForm() {
       loginFormAnimator.SetTrigger("ShowOrHide");
    }

    // Settings
    // Applies current settings to UI elements
    public void LoadBasicSettings(GameObject settingsMenu)
    {
        Slider volumeSlider = settingsMenu.gameObject.transform.Find("SoundVolume/SoundVolumeSlider").GetComponent<Slider>();
        volumeSlider.value = SettingsManager.GetSoundVolume();
        TextMeshProUGUI volumeSliderValue = settingsMenu.gameObject.transform.Find("SoundVolume/SoundVolumeSliderValue").GetComponent<TextMeshProUGUI>();
        volumeSliderValue.text = SettingsManager.GetSoundVolume().ToString();

        Slider difficultySlider = settingsMenu.gameObject.transform.Find("StartingDifficulty/StartingDifficultySlider").GetComponent<Slider>();
        difficultySlider.value = SettingsManager.GetStartingDifficulty();
        TextMeshProUGUI difficultySliderValue = settingsMenu.gameObject.transform.Find("StartingDifficulty/StartingDifficultySliderValue").GetComponent<TextMeshProUGUI>();
        difficultySliderValue.text = SettingsManager.GetStartingDifficulty().ToString();

        Toggle devModeToggle = settingsMenu.gameObject.transform.Find("DeveloperModeOn/DeveloperModeOnToggle").GetComponent<Toggle>();
        devModeToggle.isOn = SettingsManager.GetDeveloperModeOn();
        ShowDeveloperModeSettings(devModeToggle);
        if (devModeToggle.isOn)
        {
            LoadDeveloperModeSettings(settingsMenu);
        }
    }
    public void LoadDeveloperModeSettings(GameObject settingsMenu)
    {
        // Find root GameObject for the developer settings
        Transform rootObject = settingsMenu.gameObject.transform.Find("DeveloperSettings/ScrollView/Viewport/Content/Panel");
        
        // Apply values to each toggle
        Toggle qaToggle = rootObject.Find("UseQaServersOn/UseQaServersOnToggle").GetComponent<Toggle>();
        qaToggle.isOn = SettingsManager.GetUseQaServersOn();
        Toggle nextLevelToggle = rootObject.Find("LoadNextLevelOn/LoadNextLevelOnToggle").GetComponent<Toggle>();
        nextLevelToggle.isOn = SettingsManager.GetLoadNextLevelOn();
        Toggle regenerateDungeonToggle = rootObject.Find("RegenerateDungeonOn/RegenerateDungeonOnToggle").GetComponent<Toggle>();
        regenerateDungeonToggle.isOn = SettingsManager.GetRegenerateDungeonOn();
        Toggle killEnemiesToggle = rootObject.Find("KillEnemiesOn/KillEnemiesOnToggle").GetComponent<Toggle>();
        killEnemiesToggle.isOn = SettingsManager.GetKillEnemiesOn();
        Toggle regenerateHealthToggle = rootObject.Find("RegenerateHealthOn/RegenerateHealthOnToggle").GetComponent<Toggle>();
        regenerateHealthToggle.isOn = SettingsManager.GetRegenerateHealthOn();
        Toggle regenerateManaToggle = rootObject.Find("RegenerateManaOn/RegenerateManaOnToggle").GetComponent<Toggle>();
        regenerateManaToggle.isOn = SettingsManager.GetRegenerateManaOn();
        Toggle levelDumpToggle = rootObject.Find("LevelDumpOn/LevelDumpOnToggle").GetComponent<Toggle>();
        levelDumpToggle.isOn = SettingsManager.GetLevelDumpOn();
        Toggle showInfoToggle = rootObject.Find("ShowInfoOn/ShowInfoOnToggle").GetComponent<Toggle>();
        showInfoToggle.isOn = SettingsManager.GetShowInfoOn();
    }
    // Persists all settings
    public void UpdateSettings() { SettingsManager.PersistSettings(); }
    // Updates SettingsManager based on UI selection
    public void UpdateSoundVolumeSlider(GameObject sliderContainer)
    { 
        Slider sliderControl = sliderContainer.gameObject.transform.Find("SoundVolumeSlider").GetComponent<Slider>();
        SettingsManager.SetSoundVolume((int)sliderControl.value);

        TextMeshProUGUI sliderValue = sliderContainer.gameObject.transform.Find("SoundVolumeSliderValue").GetComponent<TextMeshProUGUI>();
        sliderValue.text = SettingsManager.GetSoundVolume().ToString();

        audioManager.UpdateVolume(sliderControl.value);
    }
    public void UpdateStartingDifficultySlider(GameObject sliderContainer)
    {
        Slider sliderControl = sliderContainer.gameObject.transform.Find("StartingDifficultySlider").GetComponent<Slider>();
        SettingsManager.SetStartingDifficulty((int)sliderControl.value);

        TextMeshProUGUI sliderValue = sliderContainer.gameObject.transform.Find("StartingDifficultySliderValue").GetComponent<TextMeshProUGUI>();
        sliderValue.text = SettingsManager.GetStartingDifficulty().ToString();
    }
    public void UpdateDeveloperModeToggle(GameObject settingsMenu)
    {
        Toggle devModeToggle = settingsMenu.gameObject.transform.Find("DeveloperModeOn/DeveloperModeOnToggle").GetComponent<Toggle>();
        SettingsManager.SetDeveloperModeOn(devModeToggle.isOn);
        if (devModeToggle.isOn)
        {
            LoadDeveloperModeSettings(settingsMenu);
        }
    }
    public void UpdateUseQAServersToggle(Toggle toggleControl) { SettingsManager.SetUseQaServersOn(toggleControl.isOn); }
    public void UpdateLoadNextLevelToggle(Toggle toggleControl) { SettingsManager.SetLoadNextLevelOn(toggleControl.isOn); }
    public void UpdateRegenerateDungeonToggle(Toggle toggleControl) { SettingsManager.SetRegenerateDungeonOn(toggleControl.isOn); }
    public void UpdateKillEnemiesToggle(Toggle toggleControl) { SettingsManager.SetKillEnemiesOn(toggleControl.isOn); }
    public void UpdateRegenerateHealthToggle(Toggle toggleControl) { SettingsManager.SetRegenerateHealthOn(toggleControl.isOn); }
    public void UpdateRegenerateManaToggle(Toggle toggleControl) { SettingsManager.SetRegenerateManaOn(toggleControl.isOn); }
    public void UpdateLevelDumpToggle(Toggle toggleControl) { SettingsManager.SetLevelDumpOn(toggleControl.isOn); }
    public void UpdateShowInfoToggle(Toggle toggleControl) { SettingsManager.SetShowInfoOn(toggleControl.isOn); }
    // Shows or hides developer settings from UI
    public void ShowDeveloperModeSettings(Toggle toggleControl)
    {
        if (toggleControl.isOn)
        {
            devSettingsPanel.gameObject.SetActive(true);
        }
        else
        {
            devSettingsPanel.gameObject.SetActive(false);
        }
    }

    // Hides all session related main menu UI elements
    public void HideSessionControls()
    {
        if (SessionManager.IsUserLoggedIn())
        {
            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");

        }
        else
        {
            loginButton.SetActive(false);
            // Using screen width and panel position to determine weather it is showing or not
            if (loginPanel.transform.position.x < Screen.width)
            {
                loginPanelWasShowing = true;
                ShowOrHideLoginForm();
            }
        }
    }

    // Shows all session related main menu UI elements
    public void ShowSessionControls()
    {
        if (SessionManager.IsUserLoggedIn())
        {
            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
        }
        else
        {
            loginButton.SetActive(true);
            if (loginPanelWasShowing) {
                loginPanelWasShowing = false;
                ShowOrHideLoginForm();
            }
        }
    }

    // Closes ERROR message in login panel
    public void CloseLoginError() {
        // Getting data from the UI
        Transform loginFormContainer = loginPanel.gameObject.transform.Find("LoginFormContainer");
        Transform loginMessageContainer = loginPanel.gameObject.transform.Find("LoginMessageContainer");
        Transform loginMessage = loginMessageContainer.Find("LoginMessage");
        Transform loginSpinner = loginMessageContainer.Find("LoginSpinner");
        Transform loginCloseButton = loginMessageContainer.Find("LoginCloseButton");

        // Show login form again
        loginFormContainer.gameObject.SetActive(true);
        loginMessageContainer.gameObject.SetActive(false);

        // Reset login message panel
        loginMessage.GetComponent<TMP_Text>().text = "Logging into your account\nplease wait...";
        loginSpinner.gameObject.SetActive(true);
        loginCloseButton.gameObject.SetActive(false);
    }

    // Closes ERROR message in logged in pane
    public void CloseLoggedInError() {
        // Getting data from the UI
        Transform loggedInPanelContainer = loggedPanel.gameObject.transform.Find("LoggedInPanelContainer");
        Transform loggedInUsername = loggedInPanelContainer.Find("LoggedUsername");
        Transform loggedInMessageContainer = loggedPanel.gameObject.transform.Find("LoggedInMessageContainer");
        Transform loggedInMessage = loggedInMessageContainer.Find("LoggedInMessage");
        Transform loggedInSpinner = loggedInMessageContainer.Find("LoggedInSpinner");
        Transform loggedInCloseButton = loggedInMessageContainer.Find("LoggedInCloseButton");

        // Resetting logged in panel
        loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
        loggedInMessageContainer.gameObject.SetActive(false);
        loggedInMessage.GetComponent<TMP_Text>().text = "Logging out of your account\nplease wait...";
        loggedInMessage.gameObject.SetActive(true);
        loggedInSpinner.gameObject.SetActive(true);
        loggedInCloseButton.gameObject.SetActive(false);        
        loggedInUsername.GetComponent<TMP_Text>().text = "";
        loggedInPanelContainer.gameObject.SetActive(true);

        // Clear the session details
        SessionManager.ClearSession();
        
        // Change UI element status
        loginButton.SetActive(true);
    }

    // Saves all user data to disk
    private void PersistAll() {
        // Saves user settings
        SettingsManager.PersistSettings();
        // Saves session data
        SessionManager.PersistSession();
        // Saves gameprogress data
        GameProgressManager.PersistGameProgress();
    }

    // Saves user data and quits the game
    public void ExitGame() {  
        Debug.Log("Quitting Game...");

        PersistAll();
        Application.Quit();
    }
}
