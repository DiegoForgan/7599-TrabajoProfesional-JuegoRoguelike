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

        // Loading settings from saved values
        // Assigns defaults if not present
        SettingsManager.InitializeSettings();

        // Loading session from saved values
        // Assigns defaults if not present
        SessionManager.InitializeSession();
        // Checking for saved session
        if (SessionManager.IsUserLoggedIn()) {

            Debug.Log("No session token found");
            // ToDo: Check if the token is still valid!
            highScoresButton.SetActive(true);
            loginButton.SetActive(false);
            loggedPanel.SetActive(true);
            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            GameObject.Find("LoggedUsername").GetComponent<TextMeshProUGUI>().SetText(SessionManager.GetSessionUsername());

        }
        else {
            Debug.Log("Session token found");
            highScoresButton.SetActive(false);
            loginButton.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Resets the highscores table
    // Clears results, and writes initial loading message
    public void ClearHighScoresTable() {

        // Hide the highscores entry template from the table
        highScoresEntryTemplate.SetActive(false);
        // Reset initial table message
        highScoresTableMessage.GetComponent<TextMeshProUGUI>().text = "Loading Highscores, please wait...";
        // Delete old entries
        var highscoreResults = GameObject.FindGameObjectsWithTag("HighScoreEntry");
        foreach(var entry in highscoreResults) {
            Destroy(entry);
        }
        // Enable table
        highScoresTableMessage.SetActive(true);
    }

    public void StartNewGame() {
        Debug.Log("Starting new game");
        LevelLoader.Instance.LoadNextLevel();
    }

    public void ShowOrHideLoginForm() {
        loginFormAnimator.SetTrigger("ShowOrHide");
    }

    // Settings
    // Applies current settings to UI elements
    public void LoadBasicSettings(GameObject settingsMenu)
    {
        Slider volumeSlider = settingsMenu.gameObject.transform.Find("SoundVolume/SoundVolumeSlider").GetComponent<Slider>();
        volumeSlider.value = SettingsManager.GetSoundVolume();
        
        Slider difficultySlider = settingsMenu.gameObject.transform.Find("StartingDifficulty/StartingDifficultySlider").GetComponent<Slider>();
        difficultySlider.value = SettingsManager.GetStartingDifficulty();

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
    }
    public void UpdateStartingDifficultySlider(GameObject sliderContainer)
    {
        Slider sliderControl = sliderContainer.gameObject.transform.Find("StartingDifficultySlider").GetComponent<Slider>();
        SettingsManager.SetSoundVolume((int)sliderControl.value);

        TextMeshProUGUI sliderValue = sliderContainer.gameObject.transform.Find("StartingDifficultySliderValue").GetComponent<TextMeshProUGUI>();
        sliderValue.text = SettingsManager.GetSoundVolume().ToString();
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

    // Saves user data and quits the game
    public void ExitGame() { 
        // Saves user settings befor exiting the application
        SettingsManager.PersistSettings();
        // Saves session data before exiting the application
        SessionManager.PersistSession();
        Application.Quit();
    }
}
