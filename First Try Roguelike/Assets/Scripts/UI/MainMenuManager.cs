using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    private static bool initDone = false;
    private bool loginPanelWasShowing = false;
    // Use this to mark if the user beat level 10, at any difficulty level
    // This is used so that the final message can be shown!
    private static bool finishedGame = false;
    private AudioManager audioManager;
    [SerializeField] private APIRequestHandler apiRequestHandler;
    [SerializeField] private Animator loginFormAnimator;
    [SerializeField] private GameObject gameProgessBadge;
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
    [SerializeField] private GameObject registerMenu;
    [SerializeField] private GameObject resetPasswordMenu;
    [SerializeField] private GameObject profileMenu;

    // Setter for finishedGame: always sets to *true*
    // Getter is not required, as it is handled internally
    // Shoud be re-setted to false after excecuting post game actions!
    void SetFinishedGame() {
        finishedGame = true;
    }

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

        // Set the game progress badge
        UpdateGameProgressBadge();
    }

    // Updates the Game Progress level and difficulty shown on the main menu
    private void UpdateGameProgressBadge() {
        Transform gameProgressLevel = gameProgessBadge.gameObject.transform.Find("LevelFlag/CurrentLevelText");
        Transform gameProgressDifficulty = gameProgessBadge.gameObject.transform.Find("DifficultyFlag/CurrentDifficultyLevelText");
        gameProgressLevel.GetComponent<TMP_Text>().text = "LEVEL - " + GameProgressManager.GetNextLevel().ToString();
        gameProgressDifficulty.GetComponent<TMP_Text>().text = GameProgressManager.getDifficultyLevel().ToString();
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

    // Verifies if the user wants to start a new game
    public void StartNewGameConfirm() {

        if (GameProgressManager.PlayerCanContinue())
        {
            // Ask the user via a confirmation dialog if they want to lose their game progress
            QuestionDialogUI.Instance.ShowConfirm(
                "WARNING",
                "All your saved progress will be lost. Are you sure?",
                "Yes!",
                "Cancel",
                () => {
                    // User clicked yes, we start a new game
                    StartNewGame();
                },
                // If the user cancels, we just close the dialog
                () => {}
            );
        }
        else {
            StartNewGame();
        }
    }

    // Starts a new game!
    // Loads first cinematic scene
    private void StartNewGame() {
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

        // If the player can't continue we have to update the starting difficulty in the game progress class
        if (!GameProgressManager.PlayerCanContinue())
        {
            GameProgressManager.ResetGameProgress();
            UpdateGameProgressBadge();
        }
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

    // Resets the "Reset Password" main menu screen
    public void ResetPasswordMenuForm() {
        // Getting data from the UI      
        Transform usernameInputField = resetPasswordMenu.gameObject.transform.Find("UsernameInputField");

        Transform statusContainer = resetPasswordMenu.gameObject.transform.Find("StatusContainer");
        Transform statusSpinner = statusContainer.Find("StatusSpinner");
        TMP_InputField usernameInputFieldComponent = usernameInputField.GetComponent<TMP_InputField>();
        Transform validationText = usernameInputField.gameObject.transform.Find("UsernameValidationText");
        TMP_Text statusText = statusContainer.Find("StatusText").GetComponent<TMP_Text>();
        Button sendButton = resetPasswordMenu.gameObject.transform.Find("SendButton").GetComponent<Button>();

        usernameInputField.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
        validationText.gameObject.SetActive(false);
        usernameInputFieldComponent.interactable = true;
        sendButton.interactable = true;
        statusContainer.gameObject.SetActive(false);
        statusSpinner.gameObject.SetActive(true);
        statusText.text = "Sending your request\nplease wait...";
    }

    // Resets the "Register" main menu screen
    public void ResetRegisterMenuForm() {
        // Getting data from the UI
        // Form container
        Transform formContainer = registerMenu.gameObject.transform.Find("RegisterFormContainer");
        // Submit button
        Transform submitButton = registerMenu.gameObject.transform.Find("RegisterFormContainer/SubmitButton");
        // Username
        Transform usernameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/UsernameInputField");
        Transform usernameValidationText = usernameInput.Find("UsernameValidationText");
        Transform usernameOK = usernameInput.Find("UsernameOK");
        Transform usernameSpinner = usernameInput.Find("UsernameSpinner");
        // Password
        Transform passwordInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordInputField");
        Transform passwordHintText = passwordInput.Find("PasswordHintText");
        Transform passwordValidationText = passwordInput.Find("PasswordValidationText");
        Transform passwordOK = passwordInput.Find("PasswordOK");
        // Password validation
        Transform passwordValidationInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordValidationInputField");
        Transform passwordValidationValidationText = passwordValidationInput.Find("PasswordValidationValidationText");
        Transform passwordValidationOK = passwordValidationInput.Find("PasswordValidationOK");
        // First Name
        Transform firstNameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/FirstNameInputField");
        Transform firstNameValidationText = firstNameInput.Find("FirstNameValidationText");
        Transform firstNameOK = firstNameInput.Find("FirstNameValidationOK");
        // Last Name
        Transform lastNameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/LastNameInputField");
        Transform lastNameValidationText = lastNameInput.Find("LastNameValidationText");
        Transform lastNameOK = lastNameInput.Find("LastNameValidationOK");
        // Email
        Transform emailInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/EmailInputField");
        Transform emailValidationText = emailInput.Find("EmailValidationText");
        Transform emailOK = emailInput.Find("EmailValidationOK");
        Transform emailSpinner = emailInput.Find("EmailSpinner");
        // Status
        Transform statusContainer = registerMenu.gameObject.transform.Find("StatusContainer");
        Transform statusSpinner = statusContainer.Find("StatusSpinner");
        Transform statusSuccess = statusContainer.Find("StatusSuccess");
        Transform statusError = statusContainer.Find("StatusError");
        Transform statusText = statusContainer.Find("StatusText");

        // Register form
        // Resetting submit button
        submitButton.gameObject.GetComponent<Button>().interactable = true;
        // Resetting form fields
        // Reset username field
        usernameInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
        usernameValidationText.gameObject.SetActive(false);
        usernameOK.gameObject.SetActive(false);
        usernameSpinner.gameObject.SetActive(false);
        // Reset password field
        passwordInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
        passwordHintText.gameObject.SetActive(true);
        passwordValidationText.gameObject.SetActive(false);
        passwordOK.gameObject.SetActive(false);
        // Reset password validation field
        passwordValidationInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
        passwordValidationValidationText.gameObject.SetActive(false);
        passwordValidationOK.gameObject.SetActive(false);
        // Reset first name field
        firstNameInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
        firstNameValidationText.gameObject.SetActive(false);
        firstNameOK.gameObject.SetActive(false);
        // Reset last name field
        lastNameInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
        lastNameValidationText.gameObject.SetActive(false);
        lastNameOK.gameObject.SetActive(false);
        // Reset email field
        emailInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
        emailValidationText.gameObject.SetActive(false);
        emailOK.gameObject.SetActive(false);
        emailSpinner.gameObject.SetActive(false);
        // Resetting form container
        formContainer.gameObject.SetActive(true);
        // Status
        // Resetting status container
        statusContainer.gameObject.SetActive(false);
        // Resetting status elements
        statusSpinner.gameObject.SetActive(true);
        statusSuccess.gameObject.SetActive(false);
        statusError.gameObject.SetActive(false);
        statusText.gameObject.GetComponent<TMP_Text>().text = "Sending your request\nplease wait...";
    }

    // Clear methods for register form
    // Username
    public void RegisterClearUsername() {
        // Getting data from the UI
        // Username
        Transform usernameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/UsernameInputField");
        Transform usernameValidationText = usernameInput.Find("UsernameValidationText");
        Transform usernameOK = usernameInput.Find("UsernameOK");
        Transform usernameSpinner = usernameInput.Find("UsernameSpinner");

        // If there is an error, we clear the field
        if (!FormDataValidation.IsValidUsername(usernameInput.gameObject.GetComponent<TMP_InputField>().text))
        {
            usernameInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
            usernameSpinner.gameObject.SetActive(false);
            usernameValidationText.gameObject.SetActive(false);
            usernameOK.gameObject.SetActive(false);
        }
    }
    // Password
    public void RegisterClearPassword() {

        // Getting data from the UI
        // Password
        Transform passwordInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordInputField");
        Transform passwordHintText = passwordInput.Find("PasswordHintText");
        Transform passwordValidationText = passwordInput.Find("PasswordValidationText");
        Transform passwordOK = passwordInput.Find("PasswordOK");
        // Password validation
        Transform passwordValidationInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordValidationInputField");
        Transform passwordValidationValidationText = passwordValidationInput.Find("PasswordValidationValidationText");
        Transform passwordValidationOK = passwordValidationInput.Find("PasswordValidationOK");

        string password = passwordInput.gameObject.GetComponent<TMP_InputField>().text;

        // If there is an error, we clear the field
        if (!FormDataValidation.IsValidPassword(password))
        {
            passwordInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
            passwordHintText.gameObject.SetActive(true);
            passwordValidationText.gameObject.SetActive(false);
            passwordOK.gameObject.SetActive(false);

            passwordValidationInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
            passwordValidationValidationText.gameObject.SetActive(false);
            passwordValidationOK.gameObject.SetActive(false);
        }
    }
    // Email
    public void RegisterClearEmail() {
        // Getting data from the UI
        // Email
        Transform emailInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/EmailInputField");
        Transform emailValidationText = emailInput.Find("EmailValidationText");
        Transform emailOK = emailInput.Find("EmailValidationOK");
        Transform emailSpinner = emailInput.Find("EmailSpinner");

        // If there is an error, we clear the field
        if (!FormDataValidation.IsValidEmail(emailInput.gameObject.GetComponent<TMP_InputField>().text))
        {
            emailInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
            emailSpinner.gameObject.SetActive(false);
            emailValidationText.gameObject.SetActive(false);
            emailOK.gameObject.SetActive(false);
        }
    }
    // First Name
    public void RegisterClearFirstName() {
        // Getting data from the UI
        // First Name
        Transform firstNameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/FirstNameInputField");
        Transform firstNameValidationText = firstNameInput.Find("FirstNameValidationText");
        Transform firstNameOK = firstNameInput.Find("FirstNameValidationOK");

        // If there is an error, we clear the field
        if (!FormDataValidation.IsValidName(firstNameInput.gameObject.GetComponent<TMP_InputField>().text))
        {
            firstNameInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
            firstNameValidationText.gameObject.SetActive(false);
            firstNameOK.gameObject.SetActive(false);
        }
    }
    // Last Name
    public void RegisterClearLastName() {
        // Getting data from the UI
        // Last Name
        Transform lastNameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/LastNameInputField");
        Transform lastNameValidationText = lastNameInput.Find("LastNameValidationText");
        Transform lastNameOK = lastNameInput.Find("LastNameValidationOK");

        // If there is an error, we clear the field
        if (!FormDataValidation.IsValidName(lastNameInput.gameObject.GetComponent<TMP_InputField>().text))
        {
            lastNameInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
            lastNameValidationText.gameObject.SetActive(false);
            lastNameOK.gameObject.SetActive(false);
        }
    }

    // Validate methods for register form
    // Username
    public void RegisterValidateUsername() {
        // Getting data from the UI
        // Username
        Transform usernameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/UsernameInputField");
        Transform usernameValidationText = usernameInput.Find("UsernameValidationText");
        Transform usernameOK = usernameInput.Find("UsernameOK");
        Transform usernameSpinner = usernameInput.Find("UsernameSpinner");

        // Validating the username field
        if (FormDataValidation.IsValidUsername(usernameInput.gameObject.GetComponent<TMP_InputField>().text))
        {
            usernameValidationText.gameObject.SetActive(false);
            usernameSpinner.gameObject.SetActive(false);
            usernameOK.gameObject.SetActive(true);
        }
        else
        {
            usernameValidationText.gameObject.SetActive(true);
            usernameSpinner.gameObject.SetActive(false);
            usernameOK.gameObject.SetActive(false);
        }    
    }
    // Password
    public void RegisterValidatePassword() {

        // Getting data from the UI
        // Password
        Transform passwordInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordInputField");
        Transform passwordHintText = passwordInput.Find("PasswordHintText");
        Transform passwordValidationText = passwordInput.Find("PasswordValidationText");
        Transform passwordOK = passwordInput.Find("PasswordOK");
        // Password validation
        Transform passwordValidationInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordValidationInputField");
        Transform passwordValidationValidationText = passwordValidationInput.Find("PasswordValidationValidationText");
        Transform passwordValidationOK = passwordValidationInput.Find("PasswordValidationOK");

        string password = passwordInput.gameObject.GetComponent<TMP_InputField>().text;
        string passwordValidation = passwordValidationInput.gameObject.GetComponent<TMP_InputField>().text;

        // Validating the password field, and the confirmation
        if (FormDataValidation.IsValidPassword(password))
        {
            passwordHintText.gameObject.SetActive(true);
            passwordValidationText.gameObject.SetActive(false);
            passwordOK.gameObject.SetActive(true);

            if (password == passwordValidation)
            {
                passwordValidationValidationText.gameObject.SetActive(false);
                passwordValidationOK.gameObject.SetActive(true);
            }
            else
            {
                passwordValidationValidationText.gameObject.SetActive(true);
                passwordValidationOK.gameObject.SetActive(false);
            }
        }
        else
        {
            passwordHintText.gameObject.SetActive(false);
            passwordValidationText.gameObject.SetActive(true);
            passwordOK.gameObject.SetActive(false);

            passwordValidationInput.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
            passwordValidationValidationText.gameObject.SetActive(false);
            passwordValidationOK.gameObject.SetActive(false);
        }
    }
    public void RegisterValidatePasswordValidation() {

        // Getting data from the UI
        // Password
        Transform passwordInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordInputField");
        Transform passwordHintText = passwordInput.Find("PasswordHintText");
        Transform passwordValidationText = passwordInput.Find("PasswordValidationText");
        Transform passwordOK = passwordInput.Find("PasswordOK");
        // Password validation
        Transform passwordValidationInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordValidationInputField");
        Transform passwordValidationValidationText = passwordValidationInput.Find("PasswordValidationValidationText");
        Transform passwordValidationOK = passwordValidationInput.Find("PasswordValidationOK");

        string password = passwordInput.gameObject.GetComponent<TMP_InputField>().text;
        string passwordValidation = passwordValidationInput.gameObject.GetComponent<TMP_InputField>().text;

        // Validating the password field, and the confirmation
        if (FormDataValidation.IsValidPassword(password))
        {
            passwordHintText.gameObject.SetActive(true);
            passwordValidationText.gameObject.SetActive(false);
            passwordOK.gameObject.SetActive(true);

            if (password == passwordValidation)
            {
                passwordValidationValidationText.gameObject.SetActive(false);
                passwordValidationOK.gameObject.SetActive(true);
            }
            else
            {
                passwordValidationValidationText.gameObject.SetActive(true);
                passwordValidationOK.gameObject.SetActive(false);
            }
        }
    }    
    // Email
    public void RegisterValidateEmail() {
        // Getting data from the UI
        // Email
        Transform emailInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/EmailInputField");
        Transform emailValidationText = emailInput.Find("EmailValidationText");
        Transform emailOK = emailInput.Find("EmailValidationOK");
        Transform emailSpinner = emailInput.Find("EmailSpinner");

        // Validating the email field
        if (FormDataValidation.IsValidEmail(emailInput.gameObject.GetComponent<TMP_InputField>().text))
        {
            emailValidationText.gameObject.SetActive(false);
            emailSpinner.gameObject.SetActive(false);
            emailOK.gameObject.SetActive(true);
        }
        else
        {
            emailValidationText.gameObject.SetActive(true);
            emailSpinner.gameObject.SetActive(false);
            emailOK.gameObject.SetActive(false);
        }   
    }
    // First Name
    public void RegisterValidateFirstName() {
        // Getting data from the UI
        // First Name
        Transform firstNameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/FirstNameInputField");
        Transform firstNameValidationText = firstNameInput.Find("FirstNameValidationText");
        Transform firstNameOK = firstNameInput.Find("FirstNameValidationOK");

        // Validating the first name field
        if (FormDataValidation.IsValidName(firstNameInput.gameObject.GetComponent<TMP_InputField>().text))
        {
            firstNameValidationText.gameObject.SetActive(false);
            firstNameOK.gameObject.SetActive(true);
        }
        else
        {
            firstNameValidationText.gameObject.SetActive(true);
            firstNameOK.gameObject.SetActive(false);
        }    
    }
    // Last Name
    public void RegisterValidateLastName() {
        // Getting data from the UI
        // Last Name
        Transform lastNameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/LastNameInputField");
        Transform lastNameValidationText = lastNameInput.Find("LastNameValidationText");
        Transform lastNameOK = lastNameInput.Find("LastNameValidationOK");

        // Validating the last name field
        if (FormDataValidation.IsValidName(lastNameInput.gameObject.GetComponent<TMP_InputField>().text))
        {
            lastNameValidationText.gameObject.SetActive(false);
            lastNameOK.gameObject.SetActive(true);
        }
        else
        {
            lastNameValidationText.gameObject.SetActive(true);
            lastNameOK.gameObject.SetActive(false);
        }    
    }

    // Saves user data and quits the game
    public void ExitGame() {

        var exitMessages = new List<string> {
            "Your quest is not finished!",
            "The people of Nodnol need you!",
            "Nilbud is still at large!",
            "Ordanel is cold in his cell!",
            "Don't be such a wuss!"
        };

        // Ask the user via a confirmation dialog if they want to quit
        QuestionDialogUI.Instance.ShowConfirm(
            "QUIT?",
            "How dare you quit. " + exitMessages[Random.Range(0,(exitMessages.Count-1))],
            "I'm scared",
            "Fight on!",
            () => {
                // User clicked yes, we quit the game
                Debug.Log("Quitting Game...");
                PersistAll();
                Application.Quit();
            },
            // If the user cancels, we just close the dialog
            () => {}
        );
    }
}
