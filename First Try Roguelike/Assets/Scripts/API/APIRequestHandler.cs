using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;

// This class handles the communication with the FIUBA CloudSync API
// Temporarily, it also controls the Main Menu UI after an async call is resolved
// TODO: separate Request and UI handling responsibilities 
// ---
// API technical documuentation: https://github.com/juanmg0511/7599-TrabajoProfesional-CloudSync-AppServer/wiki/Dise%C3%B1o:-Servidor-Flask-(API)
// OpenAPI 3.0 specification: https://app-qa.7599-fiuba-cs.net/api/v1/swagger-ui/
public class APIRequestHandler : MonoBehaviour
{
    // Resources and Routes 
    // The PR environment of FIUBA CloudSync is not online for the develepment process
    // Use the "Use QA Servers" in the Developer mode settings
    private const string PR_URL = "https://app.7599-fiuba-cs.net/api/v1/";
    private const string QA_URL = "https://app-qa.7599-fiuba-cs.net/api/v1/";
    private const string DEFAULT_AVATAR_URL = "https://ui-avatars.com/api/?background=321FDB&color=FFFFFF&size=512&name=";
    private const string HIGHSCORES_ROUTE = "highscores?start=0&limit=50&sort_column=difficulty_level,achieved_level,gold_collected,time_elapsed&sort_order=-1,-1,-1,1";
    private const string GAMEPROGRESS_ROUTE = "gameprogress";
    private const string SESSIONS_ROUTE = "sessions";
    private const string RECOVERY_ROUTE = "recovery";
    private const string USERS_ROUTE = "users";
    private const string EMAIL_MASK_REGEX = @".(?=.*..@)";

    // UI objects references
    [SerializeField] private GameObject loggedPanel;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject highScoresButton;
    [SerializeField] private GameObject howToPlayButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject aboutButton;
    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject highScoresTableMessage;
    [SerializeField] private GameObject highScoresEntryContainer;
    [SerializeField] private GameObject highScoresEntryTemplate;
    [SerializeField] private GameObject lowerButtonRow;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject registerMenu;
    [SerializeField] private GameObject resetPasswordMenu;
    [SerializeField] private GameObject profileMenu;
    [SerializeField] private GameObject gameProgessBadge;

    // Returns the base URL to use, depending on the configured setting
    // Use the "Use QA Servers" in the Developer mode settings to use QA environment
    private string GetServerBaseURL()
    {
        if (SettingsManager.GetUseQaServersOn())
        {
            return QA_URL;
        }
        else
        {
            return PR_URL;
        }
    } 


    // Operations to the API
    // Verifies that a username or email is already registered
    public void CheckUsernameAlreadyTaken(){
        StartCoroutine(CheckUsernameRequest());
    }
    // Checks if a session is still valid
    // If it is, it extends it by one time delta (configurable in server)
    public void CheckValidSession(){
        StartCoroutine(CheckValidSessionRequest());
    }
    // Sends a request to open a new FIUBA CloudSync account
    // Performs field validations, but they should also be done client side
    public void RegisterNewUser(){

        // Getting data from the UI
        // Username
        Transform usernameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/UsernameInputField");
        Transform usernameOK = usernameInput.Find("UsernameOK");
        // Password
        Transform passwordInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordInputField");
        Transform passwordOK = passwordInput.Find("PasswordOK");
        // Password validation
        Transform passwordValidationInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordValidationInputField");
        Transform passwordValidationOK = passwordValidationInput.Find("PasswordValidationOK");
        // First Name
        Transform firstNameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/FirstNameInputField");
        Transform firstNameOK = firstNameInput.Find("FirstNameValidationOK");
        // Last Name
        Transform lastNameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/LastNameInputField");
        Transform lastNameOK = lastNameInput.Find("LastNameValidationOK");
        // Email
        Transform emailInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/EmailInputField");
        Transform emailOK = emailInput.Find("EmailValidationOK");

        // We can only make the request if there are no errors in the form
        if (usernameOK.gameObject.activeSelf &&
            passwordOK.gameObject.activeSelf &&
            passwordValidationOK.gameObject.activeSelf &&
            firstNameOK.gameObject.activeSelf &&
            lastNameOK.gameObject.activeSelf &&
            emailOK.gameObject.activeSelf)
        {
            StartCoroutine(RegisterNewUserRequest());
        }
    }
    // Sends a password reset request
    // Performs username field validation, but it should also be done client side
    public void ForgotPassword(){

        // Getting data from the UI
        Transform usernameInputField = resetPasswordMenu.gameObject.transform.Find("UsernameInputField");

        string username = usernameInputField.gameObject.GetComponent<TMP_InputField>().text;
        Transform validationText = usernameInputField.gameObject.transform.Find("UsernameValidationText");

        // Only make request if user has entered a valid email address
        if (!string.IsNullOrWhiteSpace(username) && FormDataValidation.IsValidUsername(username)) {
            validationText.gameObject.SetActive(false);
            StartCoroutine(ForgotPasswordRequest());
        }
        else {
            validationText.gameObject.SetActive(true);
        }        
    }
    // Logs the user into the system
    // A user can concurrently log in from multiple devices
    // Returns a session object, with token
    public void UserLogin(){

        // Getting data from the UI
        Transform loginFormContainer = loginPanel.gameObject.transform.Find("LoginFormContainer");
        Transform inputUsername = loginFormContainer.Find("UsernameInputField");
        Transform inputPassword = loginFormContainer.Find("PasswordInputField");

        string username = inputUsername.GetComponent<TMP_InputField>().text;
        string password = inputPassword.GetComponent<TMP_InputField>().text;

        // Only make request if user has entered BOTH username and pwd
        if (!string.IsNullOrWhiteSpace(username) && FormDataValidation.IsValidUsername(username) && !string.IsNullOrWhiteSpace(password)) {
            if (GameProgressManager.PlayerCanContinue())
            {
                // Ask the user via a confirmation dialog if they want to quit
                QuestionDialogUI.Instance.ShowConfirm(
                    "WARNING!",
                    "All your offline progress will be lost. Are you sure?",
                    "Yes",
                    "Cancel",
                    () => {
                        // User clicked yes, we login
                        StartCoroutine(UserLoginRequest());
                },
                    // If the user cancels, we just close the dialog
                    () => {}
                );
            }
            else
            {
                // The user has no offline progress, log in directly
                StartCoroutine(UserLoginRequest());
            }
        }
        else {
            inputUsername.GetComponent<TMP_InputField>().text = string.Empty;
            inputPassword.GetComponent<TMP_InputField>().text = string.Empty;
        }
    }
    // Logs the user out of the system
    // If the request fails with "unauthorized", the session should be cleared in the client
    public void UserLogOut(){
        if (SessionManager.IsUserLoggedIn()) StartCoroutine(UserLogOutRequest());
        else Debug.LogWarning("Cant logout user because is already logged out!");
    }
    // Fetches the highscores table from the servers
    public void GetHighScores(){
        StartCoroutine(GetHighScoresRequest());
    }
    // Get the user's profile information
    public void GetUserProfile() {
        // Getting data from the UI
        // Form modes
        Transform displayMode = profileMenu.gameObject.transform.Find("DisplayMode");
        Transform editMode = profileMenu.gameObject.transform.Find("EditMode");
        Transform changeAvatar = profileMenu.gameObject.transform.Find("ChangeAvatar");
        Transform changePassword = profileMenu.gameObject.transform.Find("ChangePassword");
        Transform operationStatus = profileMenu.gameObject.transform.Find("OperationStatus");
        // Action Buttons
        Transform editModeButton = displayMode.gameObject.transform.Find("ActionPanel/EditProfileButton");
        Transform changeAvatarButton = displayMode.gameObject.transform.Find("ActionPanel/ChangeAvatarButton");
        Transform changePasswordButton = displayMode.gameObject.transform.Find("ActionPanel/ChangePasswordButton");
        Transform closeAccountButton = displayMode.gameObject.transform.Find("ActionPanel/CloseAccountButton");

        // Setting default mode
        displayMode.gameObject.SetActive(true);
        changeAvatar.gameObject.SetActive(false);
        changePassword.gameObject.SetActive(false);
        editMode.gameObject.SetActive(false);
        operationStatus.gameObject.SetActive(false);

        editModeButton.GetComponent<Button>().interactable = false;
        changeAvatarButton.GetComponent<Button>().interactable = false;
        changePasswordButton.GetComponent<Button>().interactable = false;
        closeAccountButton.GetComponent<Button>().interactable = false;

        // Fetch the data
        StartCoroutine(GetUserProfileRequest());
        StartCoroutine(GetUserGameProgressProfileRequest());
        StartCoroutine(GetUserHighScoresProfileRequest());        
    }
    // Close the user's account
    public void CloseUserAccount() {
        // Getting data from the UI
        // Form modes
        Transform displayMode = profileMenu.gameObject.transform.Find("DisplayMode");
        Transform editMode = profileMenu.gameObject.transform.Find("EditMode");
        Transform changeAvatar = profileMenu.gameObject.transform.Find("ChangeAvatar");
        Transform changePassword = profileMenu.gameObject.transform.Find("ChangePassword");
        Transform operationStatus = profileMenu.gameObject.transform.Find("OperationStatus");

        // Setting default mode
        displayMode.gameObject.SetActive(true);
        changeAvatar.gameObject.SetActive(false);
        changePassword.gameObject.SetActive(false);
        editMode.gameObject.SetActive(false);
        operationStatus.gameObject.SetActive(false);

        // Ask the user via a confirmation dialog if they want to close the account
        QuestionDialogUI.Instance.ShowConfirm(
            "GOODBYE?",
            "You are about to close your account. This action cannot be undone!",
            "OK",
            "Cancel",
            () => {
                // User clicked yes, we close their account
                StartCoroutine(CloseUserAccountRequest());
            },
            // If the user cancels, we just close the dialog
            () => {}
        );
    }
    // Change the user's avatar
    public void ChangeUserAvatar() {

        StartCoroutine(ChangeUserAvatarRequest());
    }
    // Change the user's password
    public void ChangeUserPassword() {
        // Getting data from the UI
        // Form modes
        Transform changePassword = profileMenu.gameObject.transform.Find("ChangePassword");
        // Password
        Transform passwordInput = changePassword.gameObject.transform.Find("ChangePasswordFormContainer/PasswordInputField");
        Transform passwordOK = passwordInput.Find("PasswordOK");
        // Password validation
        Transform passwordValidationInput = changePassword.gameObject.transform.Find("ChangePasswordFormContainer/PasswordValidationInputField");
        Transform passwordValidationOK = passwordValidationInput.Find("PasswordValidationOK");

        // We can only make the request if there are no errors in the form
        if (passwordOK.gameObject.activeSelf &&
            passwordValidationOK.gameObject.activeSelf)
        {
            StartCoroutine(ChangeUserPasswordRequest());
        }
    }
    // Update the user's details
    public void UpdateUser() {
        // Getting data from the UI
        // Email
        Transform emailInput = profileMenu.gameObject.transform.Find("EditMode/EditProfileFormContainer/EmailInputField");
        Transform emailOK = emailInput.Find("EmailValidationOK");
        // First Name
        Transform firstNameInput = profileMenu.gameObject.transform.Find("EditMode/EditProfileFormContainer/FirstNameInputField");
        Transform firstNameOK = firstNameInput.Find("FirstNameValidationOK");
        // Last Name
        Transform lastNameInput = profileMenu.gameObject.transform.Find("EditMode/EditProfileFormContainer/LastNameInputField");
        Transform lastNameOK = lastNameInput.Find("LastNameValidationOK");

        // We can only make the request if there are no errors in the form
        if (emailOK.gameObject.activeSelf &&
            firstNameOK.gameObject.activeSelf &&
            lastNameOK.gameObject.activeSelf)
        {
            StartCoroutine(UpdateUserRequest());
        }
    }
    // Post a new Highscore record
    public void PostNewHighScore(string postHighsocreJson) {
        StartCoroutine(PostNewHighScoreRequest(postHighsocreJson));
    }
    // Gets the player's game progress
    public void GetGameProgress() {
        StartCoroutine(GetGameProgressRequest());
    }
    // Creates the player's game progress
    public void CreateGameProgress(string updateGameProgressJson) {
        StartCoroutine(CreateGameProgressRequest(updateGameProgressJson));
    }
    // Saves the player's game progress
    public void UpdateGameProgress(string updateGameProgressJson) {
        StartCoroutine(UpdateGameProgressRequest(updateGameProgressJson));
    }

    // Async request and result handling implementation
    private IEnumerator CheckValidSessionRequest()
    {
        // Getting data from the UI
        Transform loggedInPanelContainer = loggedPanel.gameObject.transform.Find("LoggedInPanelContainer");
        Transform loggedInUsername = loggedInPanelContainer.Find("LoggedUsername");
        Transform loggedInMessageContainer = loggedPanel.gameObject.transform.Find("LoggedInMessageContainer");
        Transform loggedInMessage = loggedInMessageContainer.Find("LoggedInMessage");
        Transform loggedInSpinner = loggedInMessageContainer.Find("LoggedInSpinner");
        Transform loggedInCloseButton = loggedInMessageContainer.Find("LoggedInCloseButton");

        // Disabling "High Scores" button
        highScoresButton.GetComponent<Button>().interactable = false;
        // Disabling menu navigation
        SessionDisableMainControls();

        UnityWebRequest request = UnityWebRequest.Get(GetServerBaseURL()+"/sessions/"+SessionManager.GetSessionToken());
        
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);

        if (responseDTO.getResult() == UnityWebRequest.Result.Success)
        {
            // Session is still valid, and now is renwed
            Debug.Log("Valid session");

            // Save the game progress
            loggedInMessage.GetComponent<TMP_Text>().text = "Syncing your game progress\nplease wait...";
            StartCoroutine(UpdateGameProgressRequest(GameProgressManager.GetJsonStringUpdateGameProgress(SessionManager.GetSessionUsername(), false)));
        }
        else
        {
            // Session is no longer valid, show error message
            Debug.Log("Invalid session!");
            SetloggedInPanelError("ERROR\nyour session has expired");
            // Enabling menu navigation
            SessionEnableMainControls();
        }
        // Session check process done
        Debug.Log("Done!");
    }

    // Configures the logged in panel to show a session expired message
    // Should be called from all API requests that error our due to expired session
    private void SetloggedInPanelError(string message)
    {
        // Getting data from the UI
        Transform loggedInPanelContainer = loggedPanel.gameObject.transform.Find("LoggedInPanelContainer");
        Transform loggedInMessageContainer = loggedPanel.gameObject.transform.Find("LoggedInMessageContainer");
        Transform loggedInMessage = loggedInMessageContainer.Find("LoggedInMessage");
        Transform loggedInSpinner = loggedInMessageContainer.Find("LoggedInSpinner");
        Transform loggedInCloseButton = loggedInMessageContainer.Find("LoggedInCloseButton");

        // Setting up logged in panel
        loggedInPanelContainer.gameObject.SetActive(false);
        loggedInMessageContainer.gameObject.SetActive(true);
        loggedInMessage.GetComponent<TMP_Text>().text = message;
        loggedInMessage.gameObject.SetActive(true);
        loggedInSpinner.gameObject.SetActive(false);
        loggedInCloseButton.gameObject.SetActive(true);        
    }

    // Disables navigation during session operations in the main screen (login/logout)
    private void SessionDisableMainControls() {

        newGameButton.GetComponent<Button>().interactable = false;
        continueButton.GetComponent<Button>().interactable = false;
        howToPlayButton.GetComponent<Button>().interactable = false;
        settingsButton.GetComponent<Button>().interactable = false;
        aboutButton.GetComponent<Button>().interactable = false;
        loginButton.GetComponent<Button>().interactable = false;
    }

    // Enables navigation during session operations in the main screen (login/logout)
    private void SessionEnableMainControls() {

        newGameButton.GetComponent<Button>().interactable = true;
        continueButton.GetComponent<Button>().interactable = GameProgressManager.PlayerCanContinue();
        howToPlayButton.GetComponent<Button>().interactable = true;
        settingsButton.GetComponent<Button>().interactable = true;
        aboutButton.GetComponent<Button>().interactable = true;
        loginButton.GetComponent<Button>().interactable = true;
    }

    private IEnumerator GetGameProgressRequest()
    {
        // Getting data from the UI
        Transform loginFormContainer = loginPanel.gameObject.transform.Find("LoginFormContainer");
        Transform loginMessageContainer = loginPanel.gameObject.transform.Find("LoginMessageContainer");
        Transform loginMessage = loginMessageContainer.Find("LoginMessage");
        Transform loginSpinner = loginMessageContainer.Find("LoginSpinner");
        Transform loginCloseButton = loginMessageContainer.Find("LoginCloseButton");

        UnityWebRequest request = UnityWebRequest.Get(GetServerBaseURL()+USERS_ROUTE+"/"+SessionManager.GetSessionUsername()+"/"+GAMEPROGRESS_ROUTE);
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        
        loginMessage.GetComponent<TMP_Text>().text = "Syncing your game progress\nplease wait...";

        yield return request.SendWebRequest();
        
        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);

        if (responseDTO.getResult() == UnityWebRequest.Result.Success) {
            
            GameProgressResponseDTO gameProgressResponse = JsonConvert.DeserializeObject<GameProgressResponseDTO>(responseDTO.getBody());

            // Load gameprogress from cloud
            GameProgressManager.SetNexLevel(gameProgressResponse.next_level);
            GameProgressManager.SetDifficultyLevel(gameProgressResponse.difficulty_level);
            GameProgressManager.SetGoldCollected(gameProgressResponse.gold_collected);
            GameProgressManager.SetTimeElapsed(gameProgressResponse.time_elapsed);

            // Set UI status
            UpdateGameProgressBadge();
            loginButton.SetActive(false);
            highScoresButton.SetActive(true);
            loginPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            loggedPanel.SetActive(true);
            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            // Resetting the form for next use
            loginFormContainer.gameObject.SetActive(true);
            loginMessageContainer.gameObject.SetActive(false);
			
	        // Enbaling "New Game" button
	        newGameButton.GetComponent<Button>().interactable = true;
			// Enabling "Continue" button
            if (GameProgressManager.PlayerCanContinue()) {
                continueButton.GetComponent<Button>().interactable = true;
            }
			
	        // Reset login message panel
	        loginMessage.GetComponent<TMP_Text>().text = "Logging into your account\nplease wait...";
	        loginSpinner.gameObject.SetActive(true);
	        loginCloseButton.gameObject.SetActive(false);
        }
        else {
            APIErrorResponseDTO errorResponse = JsonConvert.DeserializeObject<APIErrorResponseDTO>(responseDTO.getBody());
            if (request.responseCode == (long)HttpStatusCode.NotFound && errorResponse.getCode() == -1){
                StartCoroutine(CreateGameProgressRequest(GameProgressManager.GetJsonStringUpdateGameProgress(SessionManager.GetSessionUsername(), true)));
            }
            else {
                loginMessage.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
                loginSpinner.gameObject.SetActive(false);
	            loginCloseButton.gameObject.SetActive(true);
            
                // Enabling menu navigation
	            SessionEnableMainControls();
            }
        }
    }

    private IEnumerator GetHighScoresRequest(){

        Transform highScoresTableMessageText = highScoresTableMessage.gameObject.transform.Find("HighscoresTableMessageText");
        Transform highScoresTableMessageSpinner = highScoresTableMessage.gameObject.transform.Find("HighscoresTableSpinner");
        Transform highScoresTableMessageSucess = highScoresTableMessage.gameObject.transform.Find("HighscoresTableSuccess");
        Transform highScoresTableMessageFailure = highScoresTableMessage.gameObject.transform.Find("HighscoresTableError");
        
        UnityWebRequest request = UnityWebRequest.Get(GetServerBaseURL()+HIGHSCORES_ROUTE);
        
        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);
        
        if (responseDTO.getResult() == UnityWebRequest.Result.Success){

            PaginatedHighscoreResponseDTO paginatedHighscoreResponse = JsonConvert.DeserializeObject<PaginatedHighscoreResponseDTO>(responseDTO.getBody());
            List<HighScoreResultsDTO> highscoreResults = paginatedHighscoreResponse.getResults();

            if (highscoreResults.Count == 0) {
                highScoresTableMessageText.GetComponent<TextMeshProUGUI>().text = "No Highscores found!";
                highScoresTableMessageSpinner.gameObject.SetActive(false);
                highScoresTableMessageSucess.gameObject.SetActive(true);
            }
            else {
                highScoresTableMessage.SetActive(false);

                int i = 0;
                float templateHeight = 30f;
                foreach (var entry in highscoreResults)
                {
                    Transform entryTransform = Instantiate(highScoresEntryTemplate.transform, highScoresEntryContainer.transform);
                    entryTransform.gameObject.tag = "HighScoreEntry";
                    RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
                    entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
                    entryTransform.Find("Background").gameObject.SetActive(i % 2 == 1);
                    entryTransform.gameObject.SetActive(true);

                    int rank = i + 1;
                    string rankString = "";
                    switch(rank % 100)
                    {
                        case 11:
                        case 12:
                        case 13:
                            rankString = rank.ToString() + "th";
                            break;
                        default:
                            switch (rank % 10) {
                                case 1:
                                    rankString = rank.ToString() + "st";
                                    break;
                                case 2:
                                    rankString = rank.ToString() + "nd";
                                    break;
                                case 3:
                                    rankString = rank.ToString() + "rd";
                                    break;
                                default:
                                    rankString = rank.ToString() + "th";
                                    break;
                            }
                            break;
                    }

                    entryTransform.Find("TextPos").GetComponent<TextMeshProUGUI>().text = rankString;
                    entryTransform.Find("TextUsername").GetComponent<TextMeshProUGUI>().text = entry.getUsername();
                    entryTransform.Find("TextLevel").GetComponent<TextMeshProUGUI>().text = entry.getAchievedLevel().ToString();
                    entryTransform.Find("TextDifficulty").GetComponent<TextMeshProUGUI>().text = entry.getDifficultyLevel().ToString();
                    entryTransform.Find("TextGold").GetComponent<TextMeshProUGUI>().text = entry.getGoldCollected().ToString();
                    entryTransform.Find("TextTime").GetComponent<TextMeshProUGUI>().text = entry.getTimeElapsed();

                    i++;
                }
            }
        }
        else{
            ErrorAPIResponse errorResponse = JsonUtility.FromJson<ErrorAPIResponse>(request.downloadHandler.text);
            highScoresTableMessageText.GetComponent<TextMeshProUGUI>().text = "Error fetching data!";
            highScoresTableMessageSpinner.gameObject.SetActive(false);
            highScoresTableMessageFailure.gameObject.SetActive(true);

            // If the result was "Unauthorized", we show the session expired message in the logged in panel
            if (request.responseCode == (long)HttpStatusCode.Unauthorized)
            {
                SetloggedInPanelError("ERROR\nyour session has expired");
            }
            else {
                SetloggedInPanelError("ERROR\nPlease try again later");
            }
        }
    }

    private IEnumerator UserLoginRequest(){

        // Getting data from the UI
        Transform loginFormContainer = loginPanel.gameObject.transform.Find("LoginFormContainer");
        Transform loginMessageContainer = loginPanel.gameObject.transform.Find("LoginMessageContainer");
        Transform loginMessage = loginMessageContainer.Find("LoginMessage");
        Transform loginSpinner = loginMessageContainer.Find("LoginSpinner");
        Transform loginCloseButton = loginMessageContainer.Find("LoginCloseButton");

        string username = loginFormContainer.Find("UsernameInputField").GetComponent<TMP_InputField>().text;
        string password = loginFormContainer.Find("PasswordInputField").GetComponent<TMP_InputField>().text;

        loginFormContainer.gameObject.SetActive(false);
        loginMessageContainer.gameObject.SetActive(true);
        // Disabling menu navigation
        SessionDisableMainControls();

        // Formatting JSON string
        LoginRequestDTO loginRequest = new(username, password);
        string loginRequestJSON = JsonConvert.SerializeObject(loginRequest);
     
        // The UnityWebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        UnityWebRequest request = UnityWebRequest.Put(GetServerBaseURL()+SESSIONS_ROUTE, loginRequestJSON);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        Debug.Log("Login Status: Logging into your account, please wait...");
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);

        // Resseting username and password fields
        loginFormContainer.Find("UsernameInputField").GetComponent<TMP_InputField>().text = "";
        loginFormContainer.Find("PasswordInputField").GetComponent<TMP_InputField>().text = "";

        if (responseDTO.getResult() == UnityWebRequest.Result.Success){
            Debug.Log("Success");
            // Formatting data to JSON.
            LoginResponseDTO loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseDTO.getBody());
            // Storing user session data to use it on other API endpoints
            SessionManager.SetSession(loginResponse.getSessionToken(), loginResponse.getUsername());
            Debug.Log("Logged In!");
            setLoggedPanel(loginResponse);
            // Start fecth game progress
            StartCoroutine(GetGameProgressRequest());
        }
        else{
            Debug.Log("Error");
            // Formatting data to JSON.
            try {
                APIErrorResponseDTO errorResponse = JsonConvert.DeserializeObject<APIErrorResponseDTO>(responseDTO.getBody());
                // Show data to the user to reflect the result of the request
                Debug.Log(errorResponse.getCode());
                Debug.Log(errorResponse.getMessage());
                Debug.Log(errorResponse.getData());
            }
            catch
            {
                Debug.LogWarning("This server is not sending the correct messages!");
            }
            // Show error message in panel
            if (request.responseCode == (long)HttpStatusCode.BadRequest || request.responseCode == (long)HttpStatusCode.Unauthorized)
            {
                loginMessage.GetComponent<TMP_Text>().text = "ERROR\nWrong user or password";
            }
            else
            {
                loginMessage.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
            }
            loginSpinner.gameObject.SetActive(false);
            loginCloseButton.gameObject.SetActive(true);
            // Enabling menu navigation
            SessionEnableMainControls();
        }
    }

    private void setLoggedPanel(LoginResponseDTO loginResponse)
    {
        loggedPanel.transform.Find("LoggedInPanelContainer/LoggedUsername").GetComponent<TextMeshProUGUI>().SetText(loginResponse.getUsername());
    }

    private IEnumerator UserLogOutRequest(){
        // The UnityWebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        if(SessionManager.IsUserLoggedIn()){

            // Getting data from the UI
            Transform loggedInPanelContainer = loggedPanel.gameObject.transform.Find("LoggedInPanelContainer");
            Transform loggedInMessageContainer = loggedPanel.gameObject.transform.Find("LoggedInMessageContainer");
            Transform loggedInMessage = loggedInMessageContainer.Find("LoggedInMessage");
            Transform loggedInSpinner = loggedInMessageContainer.Find("LoggedInSpinner");
            Transform loggedInCloseButton = loggedInMessageContainer.Find("LoggedInCloseButton");

            // Disabling "High Scores" button
            highScoresButton.GetComponent<Button>().interactable = false;
            // Disabling menu navigation
            SessionDisableMainControls();

            loggedInPanelContainer.gameObject.SetActive(false);
            loggedInMessage.GetComponent<TMP_Text>().text = "Logging out of your account\nplease wait...";
            loggedInMessage.gameObject.SetActive(true);
            loggedInSpinner.gameObject.SetActive(true);
            loggedInCloseButton.gameObject.SetActive(false);
            loggedInMessageContainer.gameObject.SetActive(true);

            UnityWebRequest request = UnityWebRequest.Get(GetServerBaseURL()+SESSIONS_ROUTE+"/"+SessionManager.GetSessionToken());
            request.method = UnityWebRequest.kHttpVerbDELETE;
        
            request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
            request.SetRequestHeader("Accept", "application/json");
        
            yield return request.SendWebRequest();
            UnityWebRequestResponseDTO responseDTO = new(request);
            showResponseData(responseDTO);

            APIErrorResponseDTO serverResponse = JsonConvert.DeserializeObject<APIErrorResponseDTO>(responseDTO.getBody());
            // Show data to the user to reflect the result of the request
            Debug.Log(serverResponse.code);
            Debug.Log(serverResponse.message);

            // Enabling "High Scores" button
            highScoresButton.GetComponent<Button>().interactable = true;
            // Logout process done
            // Enabling menu navigation
            SessionEnableMainControls();

            if (request.result == UnityWebRequest.Result.Success){
                Debug.Log("Success");
            }
            else{
                Debug.Log("Error");
            }

            // Performing after request actions
            // Reset game progress
            GameProgressManager.ResetGameProgress();
            UpdateGameProgressBadge();
            // Set interactability of "Continue" button
            continueButton.GetComponent<Button>().interactable = GameProgressManager.PlayerCanContinue();
            // Clear sesion data
            SessionManager.ClearSession();
            loginButton.GetComponent<Button>().interactable = true;
            loginButton.SetActive(true);
            highScoresButton.SetActive(false);
            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            loggedPanel.SetActive(false);
            loginPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            // Resetting loggedin panel
            loggedInPanelContainer.gameObject.SetActive(true);
            loggedInMessageContainer.gameObject.SetActive(false);
            // Resseting username and password fields
            GameObject.Find("UsernameInputField").GetComponent<TMP_InputField>().text = "";
            GameObject.Find("PasswordInputField").GetComponent<TMP_InputField>().text = "";
        }
    }

    private IEnumerator ForgotPasswordRequest(){
        // Getting data from the UI
        Transform usernameInputField = resetPasswordMenu.gameObject.transform.Find("UsernameInputField");

        string username = usernameInputField.gameObject.GetComponent<TMP_InputField>().text;
        Transform validationText = usernameInputField.gameObject.transform.Find("UsernameValidationText");
        Transform statusContainer = resetPasswordMenu.gameObject.transform.Find("StatusContainer");
        Transform statusSpinner = statusContainer.Find("StatusSpinner");
        TMP_InputField usernameInputFieldComponent = usernameInputField.GetComponent<TMP_InputField>();
        TMP_Text statusText = statusContainer.Find("StatusText").GetComponent<TMP_Text>();
        Button sendButton = resetPasswordMenu.gameObject.transform.Find("SendButton").GetComponent<Button>();

        // Setting initial status for form
        statusText.text = "Sending your request\nplease wait...";
        statusSpinner.gameObject.SetActive(true);
        statusContainer.gameObject.SetActive(true);
        usernameInputFieldComponent.interactable = false;
        sendButton.interactable = false;
        backButton.gameObject.GetComponent<Button>().interactable = false;

        PasswordRecoveryRequestDTO passwordRecoveryDTO = new(username);
        string passwordRecoveryBody = JsonConvert.SerializeObject(passwordRecoveryDTO);
        
        UnityWebRequest request = UnityWebRequest.Put(GetServerBaseURL()+RECOVERY_ROUTE, passwordRecoveryBody);
        request.method = UnityWebRequest.kHttpVerbPOST;
        
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);

        statusSpinner.gameObject.SetActive(false);
        backButton.gameObject.GetComponent<Button>().interactable = true;
        if (responseDTO.getResult() == UnityWebRequest.Result.Success){
            Debug.Log("Success");
            // Formatting data to JSON.
            PasswordRecoveryResponseDTO passwordRecoveryResponse = JsonConvert.DeserializeObject<PasswordRecoveryResponseDTO>(responseDTO.getBody());
            // Masks the current user recovery email address address@hosting.com => *****ss@hosting.com
            string input = passwordRecoveryResponse.getEmail();
            string maskedEmail = Regex.Replace(input, EMAIL_MASK_REGEX, m => new string('*', m.Length));
            // Shows the user the correct UI
            statusText.text = "Recovery mail sent to: " + maskedEmail + "\n Check your account to complete the process!";
        }
        else{
            usernameInputField.gameObject.GetComponent<TMP_InputField>().text = string.Empty;
            usernameInputFieldComponent.interactable = true;
            sendButton.interactable = true;
            try {
                APIErrorResponseDTO errorResponse = JsonConvert.DeserializeObject<APIErrorResponseDTO>(responseDTO.getBody());
                // Show data to the user to reflect the result of the request
                Debug.Log(errorResponse.getCode());
                Debug.Log(errorResponse.getMessage());
                Debug.Log(errorResponse.getData());
                statusText.text = errorResponse.getMessage();
            }
            catch
            {
                Debug.LogWarning("This server is not sending the correct messages!");
                statusText.text = "ERROR\nPlease try again later";
            }
        }
    }

    private IEnumerator CheckUsernameRequest(){
        // Getting data from the UI
        TMP_InputField field = GameObject.Find("UserNameInputField").GetComponent<TMP_InputField>();
        string userToCheck = field.text;
        // If input field is blank you don´t need to make a request for that
        TextMeshProUGUI usernameTakenMsg = GameObject.Find("UsernameTakenMsg").GetComponent<TextMeshProUGUI>();
        if (userToCheck == ""){ 
            usernameTakenMsg.SetText("");
            yield break;
        }
        Debug.Log("Checking if username is already taken...");
        // Preparing the GET request
        UnityWebRequest request = UnityWebRequest.Get(GetServerBaseURL()+USERS_ROUTE+"/"+userToCheck+"/exists");
        yield return request.SendWebRequest();
        UnityWebRequestResponseDTO responseDTO = new(request.result, request.responseCode, request.downloadHandler.text);
        showResponseData(responseDTO);

        // Processing the response
        // BEWARE OF THIS BUTTON INTERACTABLE ATTRIBUTE, MIGHT LEAD TO BUGS IF NOT MANAGED CORRECTLY
        Button createButton = GameObject.Find("CreateButton").GetComponent<Button>();
        // Username already taken
        if(request.result == UnityWebRequest.Result.Success){
            createButton.interactable = false;
            usernameTakenMsg.SetText("Username Already Taken !!!");
        }
        // Username not Taken -> VALID username
        else if (request.result == UnityWebRequest.Result.ProtocolError){
            createButton.interactable = true;
            usernameTakenMsg.SetText("Valid Username!");
        }
        // Error
        else if (request.result == UnityWebRequest.Result.ConnectionError){
            Debug.Log("The request resulted in a connection error");
        }
        else Debug.Log("Request resulted in unknown error");
    }

    // Generates a json object from the register form
    // Ready to be posted to the server
    private string GetJsonStringRegisterData(){
        // Getting all register data from input fields (maybe should optimize this!)
        Transform usernameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/UsernameInputField");
        Transform passwordInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/PasswordInputField");
        Transform firstNameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/FirstNameInputField");
        Transform lastNameInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/LastNameInputField");
        Transform emailInput = registerMenu.gameObject.transform.Find("RegisterFormContainer/EmailInputField");

        string newUsername = usernameInput.gameObject.GetComponent<TMP_InputField>().text;
        string newPassword = passwordInput.gameObject.GetComponent<TMP_InputField>().text;
        string newFirstName = firstNameInput.gameObject.GetComponent<TMP_InputField>().text;
        string newLastName = lastNameInput.gameObject.GetComponent<TMP_InputField>().text;
        string newEmail = emailInput.gameObject.GetComponent<TMP_InputField>().text;

        // We are not using the phone property, but the server validates in
        // A placeholder is sent
        string newPhone = "555 5555";

        // When registering a new user, the default avatar is set
        bool isUrl = true;
        string avatarUrl = DEFAULT_AVATAR_URL + newFirstName.Replace(" ", string.Empty) + "+" + newLastName.Replace(" ", string.Empty);
 
        // Custom DTO class for registration
        RegisterNewUserRequestDTO registerNewUserRequestDTO = new(newUsername,newPassword,newFirstName,newLastName,new(newEmail,newPhone),new(isUrl,avatarUrl));

        // Login service is not used, so it is not included in the message
        // As we include a username and password, the server assumes *false* 
        return JsonConvert.SerializeObject(registerNewUserRequestDTO);
    }

    // Implements the POST request to register a new user
    private IEnumerator RegisterNewUserRequest(){
        Debug.Log("Registering new user to the game...");
        // Getting data from the UI
        // Form container
        Transform formContainer = registerMenu.gameObject.transform.Find("RegisterFormContainer");
        // Status
        Transform statusContainer = registerMenu.gameObject.transform.Find("StatusContainer");
        Transform statusSpinner = statusContainer.Find("StatusSpinner");
        Transform statusSuccess = statusContainer.Find("StatusSuccess");
        Transform statusError = statusContainer.Find("StatusError");
        Transform statusText = statusContainer.Find("StatusText");

        backButton.gameObject.GetComponent<Button>().interactable = false;
        // Setting form container
        formContainer.gameObject.SetActive(false);
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(true);
        statusSuccess.gameObject.SetActive(false);
        statusError.gameObject.SetActive(false);
        statusText.gameObject.GetComponent<TMP_Text>().text = "Sending your request\nplease wait...";
        // Setting status container
        statusContainer.gameObject.SetActive(true);

        // The UnityWebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        string registerDataJson = GetJsonStringRegisterData();
        UnityWebRequest request = UnityWebRequest.Put(GetServerBaseURL()+USERS_ROUTE, registerDataJson);
        request.method = UnityWebRequest.kHttpVerbPOST;
        
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        // Show data to the user to reflect the result of the request
        showResponseData(responseDTO);

        //Response handling
        if (request.responseCode == (long)HttpStatusCode.Created){
            Debug.Log("Success");
            statusText.gameObject.GetComponent<TMP_Text>().text = "SUCCESS! Welcome to FIUBA CloudSync\nyou may login now...";
            statusSuccess.gameObject.SetActive(true);
        }
        else{
            Debug.Log("Error");
            statusError.gameObject.SetActive(true);
            // Formatting data to JSON.
            try {
                APIErrorResponseDTO errorResponse = JsonConvert.DeserializeObject<APIErrorResponseDTO>(responseDTO.getBody());

                // Show error message in panel
                if (request.responseCode == (long)HttpStatusCode.BadRequest)
                {
                    switch(errorResponse.getCode()) 
                    {
                    case -5:
                        statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\n" + errorResponse.getMessage();
                        break;
                    case -6:
                        statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\n" + errorResponse.getMessage();
                        break;
                    default:
                        statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\n Bad request. This should not happen, please contact support!";
                        break;
                    }
                }
                else
                {
                    statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
                }
            }
            catch
            {
                Debug.LogWarning("This server is not sending the correct messages!");
                statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
            }
        }
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(false);
        // Registration process done
        // Enabling menu navigation
        backButton.gameObject.GetComponent<Button>().interactable = true;
    }

    private IEnumerator GetUserProfileRequest(){
        Debug.Log("Getting your profile information...");
        // Getting data from the UI
        // Profile sections
        Transform profileSection = profileMenu.gameObject.transform.Find("DisplayMode/ScrollView/Viewport/Content/ProfileSection");
        // Fields to update
        Transform dataContainer = profileMenu.gameObject.transform.Find("DisplayMode/ScrollView/Viewport/Content/ProfileSection/Data");
        Transform usernameField = profileSection.Find("Data/UsernameData");
        Transform firstNameField = profileSection.Find("Data/FirstNameData");
        Transform lastNameField = profileSection.Find("Data/LastNameData");
        Transform emailField = profileSection.Find("Data/EmailData");
        // Status
        Transform statusContainer = profileMenu.gameObject.transform.Find("DisplayMode/ScrollView/Viewport/Content/ProfileSection/StatusContainer");
        Transform statusSpinner = statusContainer.Find("StatusSpinner");
        Transform statusSuccess = statusContainer.Find("StatusSuccess");
        Transform statusError = statusContainer.Find("StatusError");
        Transform statusText = statusContainer.Find("StatusText");
        // Action Buttons
        Transform editModeButton = profileMenu.gameObject.transform.Find("DisplayMode/ActionPanel/EditProfileButton");
        Transform changeAvatarButton = profileMenu.gameObject.transform.Find("DisplayMode/ActionPanel/ChangeAvatarButton");
        Transform changePasswordButton = profileMenu.gameObject.transform.Find("DisplayMode/ActionPanel/ChangePasswordButton");
        Transform closeAccountButton = profileMenu.gameObject.transform.Find("DisplayMode/ActionPanel/CloseAccountButton");

        // Form
        dataContainer.gameObject.SetActive(false);
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(true);
        statusSuccess.gameObject.SetActive(false);
        statusError.gameObject.SetActive(false);
        statusText.gameObject.GetComponent<TMP_Text>().text = "Loading your details\nplease wait...";
        // Setting status container
        statusContainer.gameObject.SetActive(true);
    
        UnityWebRequest request = UnityWebRequest.Get(GetServerBaseURL()+USERS_ROUTE+'/'+SessionManager.GetSessionUsername());
        
        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        Debug.Log("Done!");
        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);

       // Response handling
        if (request.responseCode == (long)HttpStatusCode.OK){
            Debug.Log("Success");
            RegisterNewUserRequestDTO profileResponse = JsonConvert.DeserializeObject<RegisterNewUserRequestDTO>(responseDTO.getBody());

            usernameField.gameObject.GetComponent<TMP_Text>().text = profileResponse.username;
            firstNameField.gameObject.GetComponent<TMP_Text>().text = profileResponse.first_name;
            lastNameField.gameObject.GetComponent<TMP_Text>().text = profileResponse.last_name;
            emailField.gameObject.GetComponent<TMP_Text>().text = profileResponse.contact.email;

            editModeButton.GetComponent<Button>().interactable = true;
            // Re-enable after implementing feature
            // changeAvatarButton.GetComponent<Button>().interactable = true;
            changePasswordButton.GetComponent<Button>().interactable = true;
            closeAccountButton.GetComponent<Button>().interactable = true;

            dataContainer.gameObject.SetActive(true);
            statusContainer.gameObject.SetActive(false);          
        }
        else{
            Debug.Log("Error");
            statusError.gameObject.SetActive(true);
            statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
            SetloggedInPanelError("ERROR\nPlease try again later");
        }
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(false);
    }

    private IEnumerator GetUserGameProgressProfileRequest()
    {
        // Getting data from the UI
        // Profile sections
        Transform profileSection = profileMenu.gameObject.transform.Find("DisplayMode/ScrollView/Viewport/Content/GameProgressSection");
        // Fields to update
        Transform dataContainer = profileMenu.gameObject.transform.Find("DisplayMode/ScrollView/Viewport/Content/GameProgressSection/Data");
        Transform nextLevelField = profileSection.Find("Data/NextLevelData");
        Transform difficultyLevelField = profileSection.Find("Data/DifficultyLevelData");
        Transform goldCollectedField = profileSection.Find("Data/GoldCollectedData");
        Transform timeElapsedField = profileSection.Find("Data/TimeElapsedData");        
        // Status
        Transform statusContainer = profileMenu.gameObject.transform.Find("DisplayMode/ScrollView/Viewport/Content/GameProgressSection/StatusContainer");
        Transform statusSpinner = statusContainer.Find("StatusSpinner");
        Transform statusSuccess = statusContainer.Find("StatusSuccess");
        Transform statusError = statusContainer.Find("StatusError");
        Transform statusText = statusContainer.Find("StatusText");

        // Form
        dataContainer.gameObject.SetActive(false);
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(true);
        statusSuccess.gameObject.SetActive(false);
        statusError.gameObject.SetActive(false);
        statusText.gameObject.GetComponent<TMP_Text>().text = "Loading your details\nplease wait...";
        // Setting status container
        statusContainer.gameObject.SetActive(true);

        UnityWebRequest request = UnityWebRequest.Get(GetServerBaseURL()+USERS_ROUTE+"/"+SessionManager.GetSessionUsername()+"/"+GAMEPROGRESS_ROUTE);
        
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        
        yield return request.SendWebRequest();
        
        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);

       //Response handling
        if (request.responseCode == (long)HttpStatusCode.OK){
            Debug.Log("Success");

            GameProgressResponseDTO gameProgress = JsonConvert.DeserializeObject<GameProgressResponseDTO>(responseDTO.getBody());
        
            nextLevelField.gameObject.GetComponent<TMP_Text>().text = gameProgress.getNextLevel().ToString();
            difficultyLevelField.gameObject.GetComponent<TMP_Text>().text = gameProgress.getDifficultyLevel().ToString();
            goldCollectedField.gameObject.GetComponent<TMP_Text>().text = gameProgress.getGoldCollected().ToString();
            timeElapsedField.gameObject.GetComponent<TMP_Text>().text = gameProgress.getTimeElapsed();      

            statusContainer.gameObject.SetActive(false);
            dataContainer.gameObject.SetActive(true);
        }
        else{
            Debug.Log("Error");
            statusError.gameObject.SetActive(true);
            if (request.responseCode == (long)HttpStatusCode.NotFound){
                statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\nNo game progress record found";
            }
            else {
                statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
                SetloggedInPanelError("ERROR\nPlease try again later");
            }
        }
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(false);
    }

    private IEnumerator GetUserHighScoresProfileRequest(){
        // Getting data from the UI
        // Profile sections
        Transform profileSection = profileMenu.gameObject.transform.Find("DisplayMode/ScrollView/Viewport/Content/HighscoresSection");
        // Fields to update
        Transform dataContainer = profileMenu.gameObject.transform.Find("DisplayMode/ScrollView/Viewport/Content/HighscoresSection/Data");
        Transform levelField = profileSection.Find("Data/NextLevelData");
        Transform difficultyLevelField = profileSection.Find("Data/DifficultyLevelData");
        Transform goldCollectedField = profileSection.Find("Data/GoldCollectedData");
        Transform timeElapsedField = profileSection.Find("Data/TimeElapsedData");
        // Status
        Transform statusContainer = profileMenu.gameObject.transform.Find("DisplayMode/ScrollView/Viewport/Content/HighscoresSection/StatusContainer");
        Transform statusSpinner = statusContainer.Find("StatusSpinner");
        Transform statusSuccess = statusContainer.Find("StatusSuccess");
        Transform statusError = statusContainer.Find("StatusError");
        Transform statusText = statusContainer.Find("StatusText");

        // Form
        dataContainer.gameObject.SetActive(false);
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(true);
        statusSuccess.gameObject.SetActive(false);
        statusError.gameObject.SetActive(false);
        statusText.gameObject.GetComponent<TMP_Text>().text = "Loading your details\nplease wait...";
        // Setting status container
        statusContainer.gameObject.SetActive(true);        

        UnityWebRequest request = UnityWebRequest.Get(GetServerBaseURL()+USERS_ROUTE+'/'+SessionManager.GetSessionUsername()+'/'+HIGHSCORES_ROUTE);
        
        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);

       //Response handling
        if (request.responseCode == (long)HttpStatusCode.OK){
            Debug.Log("Success");

            PaginatedHighscoreResponseDTO paginatedHighscoreResponse = JsonConvert.DeserializeObject<PaginatedHighscoreResponseDTO>(responseDTO.getBody());
            List<HighScoreResultsDTO> highscoreResults = paginatedHighscoreResponse.getResults();

            if (highscoreResults.Count > 0)
            {
                Debug.Log(highscoreResults[0]);
                levelField.gameObject.GetComponent<TMP_Text>().text = highscoreResults[0].getAchievedLevel().ToString();
                difficultyLevelField.gameObject.GetComponent<TMP_Text>().text = highscoreResults[0].getDifficultyLevel().ToString();
                goldCollectedField.gameObject.GetComponent<TMP_Text>().text = highscoreResults[0].getGoldCollected().ToString();
                timeElapsedField.gameObject.GetComponent<TMP_Text>().text = highscoreResults[0].getTimeElapsed();      
            
                statusContainer.gameObject.SetActive(false);
                dataContainer.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("El usurio" + SessionManager.GetSessionUsername() + "no tiene highscores");
                statusSuccess.gameObject.SetActive(true);
                statusText.gameObject.GetComponent<TMP_Text>().text = "No highscores found!\nGo fight Nilbud...";
            }
        }
        else{
            Debug.Log("Error");
            statusError.gameObject.SetActive(true);
            statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
            SetloggedInPanelError("ERROR\nPlease try again later");
        }
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(false);
    }

    private IEnumerator CloseUserAccountRequest() {
        // The UnityWebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/

        // Getting data from the UI
        // Form modes
        Transform displayMode = profileMenu.gameObject.transform.Find("DisplayMode");
        Transform editMode = profileMenu.gameObject.transform.Find("EditMode");
        Transform changeAvatar = profileMenu.gameObject.transform.Find("ChangeAvatar");
        Transform changePassword = profileMenu.gameObject.transform.Find("ChangePassword");
        Transform operationStatus = profileMenu.gameObject.transform.Find("OperationStatus");
        // Status
        Transform statusContainer = profileMenu.gameObject.transform.Find("OperationStatus/StatusContainer");
        Transform statusSpinner = statusContainer.Find("StatusSpinner");
        Transform statusSuccess = statusContainer.Find("StatusSuccess");
        Transform statusError = statusContainer.Find("StatusError");
        Transform statusText = statusContainer.Find("StatusText");
        // Logged in panel
        Transform loggedInPanelContainer = loggedPanel.gameObject.transform.Find("LoggedInPanelContainer");
        Transform loggedInMessageContainer = loggedPanel.gameObject.transform.Find("LoggedInMessageContainer");

        // Setting default mode
        displayMode.gameObject.SetActive(false);
        editMode.gameObject.SetActive(false);
        changeAvatar.gameObject.SetActive(false);
        changePassword.gameObject.SetActive(false);
        operationStatus.gameObject.SetActive(true);
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(true);
        statusSuccess.gameObject.SetActive(false);
        statusError.gameObject.SetActive(false);
        statusText.gameObject.GetComponent<TMP_Text>().text = "Closing your account\nplease wait...";
        // Setting status container
        statusContainer.gameObject.SetActive(true);        

        backButton.gameObject.GetComponent<Button>().interactable = false;
        UnityWebRequest request = UnityWebRequest.Get(GetServerBaseURL()+USERS_ROUTE+"/"+SessionManager.GetSessionUsername());
        request.method = UnityWebRequest.kHttpVerbDELETE;
        
        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();
        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);

        APIErrorResponseDTO serverResponse = JsonConvert.DeserializeObject<APIErrorResponseDTO>(responseDTO.getBody());
        backButton.gameObject.GetComponent<Button>().interactable = true;
        if (request.responseCode == (long)HttpStatusCode.OK){
            Debug.Log("Success");
            // Storing user session data to use it on other API endpoints
            SessionManager.ClearSession();
            // Showing status on screen
            statusSuccess.gameObject.SetActive(true);
            statusText.gameObject.GetComponent<TMP_Text>().text = "Sad to see you go!\nyour account has been closed...";
            // Configuring UI
            highScoresButton.SetActive(false);
            // Resetting loggedin panel
            loggedInPanelContainer.gameObject.SetActive(true);
            loggedInMessageContainer.gameObject.SetActive(false);
            // Resseting username and password fields
            GameObject.Find("UsernameInputField").GetComponent<TMP_InputField>().text = "";
            GameObject.Find("PasswordInputField").GetComponent<TMP_InputField>().text = "";
        }
        else{
            Debug.Log("Error");
            statusError.gameObject.SetActive(true);
            statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
            SetloggedInPanelError("ERROR\nPlease try again later");
        }
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(false);
    }

    // Generates a json object from the update prodfile form
    // Ready to be posted to the server
    private string GetJsonStringUpdateUserData(){
        // Getting all input fields
        Transform emailInput = profileMenu.gameObject.transform.Find("EditMode/EditProfileFormContainer/EmailInputField");
        Transform firstNameInput = profileMenu.gameObject.transform.Find("EditMode/EditProfileFormContainer/FirstNameInputField");
        Transform lastNameInput = profileMenu.gameObject.transform.Find("EditMode/EditProfileFormContainer/LastNameInputField");

        // We are not using the phone property, but the server validates in
        // A placeholder is sent
        string newPhone = "555 5555";

        // Placeholders used for testing
        string newFirstName = firstNameInput.gameObject.GetComponent<TMP_InputField>().text;
        string newLastName = lastNameInput.gameObject.GetComponent<TMP_InputField>().text;
        string newEmail = emailInput.gameObject.GetComponent<TMP_InputField>().text;
 
        // Custom DTO class for update
        UpdateUserRequestDTO updateUserRequestDTO = new(newFirstName,newLastName,new(newEmail,newPhone));
        // We return the serialized object 
        return JsonConvert.SerializeObject(updateUserRequestDTO);
    }

    // Sends the request to update a user, in the profile screen 
    private IEnumerator UpdateUserRequest(){
        Debug.Log("Updating your user details!...");
        // Getting data from the UI
        // Form modes
        Transform displayMode = profileMenu.gameObject.transform.Find("DisplayMode");
        Transform editMode = profileMenu.gameObject.transform.Find("EditMode");
        Transform changeAvatar = profileMenu.gameObject.transform.Find("ChangeAvatar");
        Transform changePassword = profileMenu.gameObject.transform.Find("ChangePassword");
        Transform operationStatus = profileMenu.gameObject.transform.Find("OperationStatus");
        // Status
        Transform statusContainer = profileMenu.gameObject.transform.Find("OperationStatus/StatusContainer");
        Transform statusSpinner = statusContainer.Find("StatusSpinner");
        Transform statusSuccess = statusContainer.Find("StatusSuccess");
        Transform statusError = statusContainer.Find("StatusError");
        Transform statusText = statusContainer.Find("StatusText");
        // Logged in panel
        Transform loggedInPanelContainer = loggedPanel.gameObject.transform.Find("LoggedInPanelContainer");
        Transform loggedInMessageContainer = loggedPanel.gameObject.transform.Find("LoggedInMessageContainer");

        // Setting default mode
        displayMode.gameObject.SetActive(false);
        editMode.gameObject.SetActive(false);
        changeAvatar.gameObject.SetActive(false);
        changePassword.gameObject.SetActive(false);
        operationStatus.gameObject.SetActive(true);
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(true);
        statusSuccess.gameObject.SetActive(false);
        statusError.gameObject.SetActive(false);
        statusText.gameObject.GetComponent<TMP_Text>().text = "Updating your profile\nplease wait...";
        // Setting status container
        statusContainer.gameObject.SetActive(true);        
        backButton.gameObject.GetComponent<Button>().interactable = false;

        string updateUserDataJson = GetJsonStringUpdateUserData();
        UnityWebRequest request = UnityWebRequest.Put(GetServerBaseURL()+USERS_ROUTE+"/"+SessionManager.GetSessionUsername(), updateUserDataJson);

        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        // Show data to the user to reflect the result of the request
        showResponseData(responseDTO);

        backButton.gameObject.GetComponent<Button>().interactable = true;
        if (request.responseCode == (long)HttpStatusCode.OK){
            Debug.Log("Success");
            // Showing status on screen
            statusSuccess.gameObject.SetActive(true);
            statusText.gameObject.GetComponent<TMP_Text>().text = "Success!\nyour profile has been updated...";
        }
        else{
            Debug.Log("Error");
            statusError.gameObject.SetActive(true);
            // Formatting data to JSON.
            try {
                APIErrorResponseDTO errorResponse = JsonConvert.DeserializeObject<APIErrorResponseDTO>(responseDTO.getBody());

                // Show error message in panel
                if (request.responseCode == (long)HttpStatusCode.BadRequest)
                {
                    switch(errorResponse.getCode()) 
                    {
                    case -4:
                        statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\n" + errorResponse.getMessage();
                        break;
                    default:
                        statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\n Bad request. This should not happen, please contact support!";
                        break;
                    }
                }
                else
                {
                    statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
                }
            }
            catch
            {
                Debug.LogWarning("This server is not sending the correct messages!");
                statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
            }
        }
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(false);
        Debug.Log("Done!");
   }

    // Generates a json object from the change avatar form
    // Ready to be posted to the server
    private string GetJsonStringChangeAvatarData(){
        // Getting all input fields
        // TODO: read form

        // The new avatar
        bool isUrl = true;
        string avatarUrl = "https://ui-avatars.com/api/?background=321FDB&color=FFFFFF&size=512&name=Z+Z";

        // Custom DTO class for avatar change
        ChangeUserAvatarRequestDTO changeUserAvatarRequestDTO = new(new(isUrl,avatarUrl));
        // We return the serialized object 
        return JsonConvert.SerializeObject(changeUserAvatarRequestDTO);
    }

    // Sends the request to reset an avatar, in the profile screen 
   private IEnumerator ChangeUserAvatarRequest(){
        Debug.Log("Changing your avatar!...");

        // The UnityWebRequest library its pretty tricky, for PATCH method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        string changeAvatarDataJson = GetJsonStringChangeAvatarData();
        UnityWebRequest request = UnityWebRequest.Put(GetServerBaseURL()+USERS_ROUTE+"/"+SessionManager.GetSessionUsername(), changeAvatarDataJson);
        // A hack of a hack, you need to hardcode the verb name to make this work
        // In 2023...
        request.method = "PATCH";

        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        // Show data to the user to reflect the result of the request
        showResponseData(responseDTO);

        Debug.Log("Done!");
   }

    // Generates a json object from the change password form
    // Ready to be posted to the server
    private string GetJsonStringChangePasswordData(){
        // Getting data from the UI
        // Form modes
        Transform changePassword = profileMenu.gameObject.transform.Find("ChangePassword");
        // Source field
        Transform passwordInput = changePassword.gameObject.transform.Find("ChangePasswordFormContainer/PasswordInputField");

        // Custom DTO class for password reset
        string newPassword = passwordInput.gameObject.GetComponent<TMP_InputField>().text;
        ChangeUserPasswordRequestDTO changeUserPasswordRequestDTO = new(newPassword);
        // We return the serialized object 
        return JsonConvert.SerializeObject(changeUserPasswordRequestDTO);
    }

    // Sends the request to reset a password, in the profile screen 
    private IEnumerator ChangeUserPasswordRequest(){
        Debug.Log("Changing your password!...");
        // Getting data from the UI
        // Form modes
        Transform displayMode = profileMenu.gameObject.transform.Find("DisplayMode");
        Transform editMode = profileMenu.gameObject.transform.Find("EditMode");
        Transform changeAvatar = profileMenu.gameObject.transform.Find("ChangeAvatar");
        Transform changePassword = profileMenu.gameObject.transform.Find("ChangePassword");
        Transform operationStatus = profileMenu.gameObject.transform.Find("OperationStatus");
        // Status
        Transform statusContainer = profileMenu.gameObject.transform.Find("OperationStatus/StatusContainer");
        Transform statusSpinner = statusContainer.Find("StatusSpinner");
        Transform statusSuccess = statusContainer.Find("StatusSuccess");
        Transform statusError = statusContainer.Find("StatusError");
        Transform statusText = statusContainer.Find("StatusText");
        // Logged in panel
        Transform loggedInPanelContainer = loggedPanel.gameObject.transform.Find("LoggedInPanelContainer");
        Transform loggedInMessageContainer = loggedPanel.gameObject.transform.Find("LoggedInMessageContainer");

        // Setting default mode
        displayMode.gameObject.SetActive(false);
        editMode.gameObject.SetActive(false);
        changeAvatar.gameObject.SetActive(false);
        changePassword.gameObject.SetActive(false);
        operationStatus.gameObject.SetActive(true);
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(true);
        statusSuccess.gameObject.SetActive(false);
        statusError.gameObject.SetActive(false);
        statusText.gameObject.GetComponent<TMP_Text>().text = "Changing your password\nplease wait...";
        // Setting status container
        statusContainer.gameObject.SetActive(true);        
        backButton.gameObject.GetComponent<Button>().interactable = false;

        // The UnityWebRequest library its pretty tricky, for PATCH method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        string changePasswordDataJson = GetJsonStringChangePasswordData();
        UnityWebRequest request = UnityWebRequest.Put(GetServerBaseURL()+USERS_ROUTE+"/"+SessionManager.GetSessionUsername(), changePasswordDataJson);
        // A hack of a hack, you need to hardcode the verb name to make this work
        // In 2023...
        request.method = "PATCH";

        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        // Show data to the user to reflect the result of the request
        showResponseData(responseDTO);

        backButton.gameObject.GetComponent<Button>().interactable = true;
        if (request.responseCode == (long)HttpStatusCode.OK){
            Debug.Log("Success");
            // Showing status on screen
            statusSuccess.gameObject.SetActive(true);
            statusText.gameObject.GetComponent<TMP_Text>().text = "Success!\nyour password has been changed...";
        }
        else{
            Debug.Log("Error");
            statusError.gameObject.SetActive(true);
            statusText.gameObject.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
        }
        // Status
        // Setting status elements
        statusSpinner.gameObject.SetActive(false);
        Debug.Log("Done!");
   }

    // Sends the request to post a new highscore
    private IEnumerator PostNewHighScoreRequest(string postHighsocreJson){
        Debug.Log("Posting your highscore!...");

        UnityWebRequest request = UnityWebRequest.Put(GetServerBaseURL()+USERS_ROUTE+"/"+SessionManager.GetSessionUsername()+"/"+HIGHSCORES_ROUTE, postHighsocreJson);
        request.method = UnityWebRequest.kHttpVerbPOST;

        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        // Show data to the user to reflect the result of the request
        showResponseData(responseDTO);

        // Status
        Debug.Log("Done!");
    }

    // Sends the request to create a new game progressRecord
    private IEnumerator CreateGameProgressRequest(string updateGameProgressJson){

        // Getting data from the UI
        Transform loginFormContainer = loginPanel.gameObject.transform.Find("LoginFormContainer");
        Transform loginMessageContainer = loginPanel.gameObject.transform.Find("LoginMessageContainer");
        Transform loginMessage = loginMessageContainer.Find("LoginMessage");
        Transform loginSpinner = loginMessageContainer.Find("LoginSpinner");
        Transform loginCloseButton = loginMessageContainer.Find("LoginCloseButton");

        Debug.Log("Creating your game progress record!...");
        UnityWebRequest request = UnityWebRequest.Put(GetServerBaseURL()+USERS_ROUTE+"/"+SessionManager.GetSessionUsername()+"/"+GAMEPROGRESS_ROUTE, updateGameProgressJson);

        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        // Show data to the user to reflect the result of the request
        showResponseData(responseDTO);

        if (responseDTO.getResult() == UnityWebRequest.Result.Success) {
            
            GameProgressResponseDTO gameProgressResponse = JsonConvert.DeserializeObject<GameProgressResponseDTO>(responseDTO.getBody());

            // Load gameprogress from cloud
            GameProgressManager.SetNexLevel(gameProgressResponse.next_level);
            GameProgressManager.SetDifficultyLevel(gameProgressResponse.difficulty_level);
            GameProgressManager.SetGoldCollected(gameProgressResponse.gold_collected);
            GameProgressManager.SetTimeElapsed(gameProgressResponse.time_elapsed);

            // Set UI status
            UpdateGameProgressBadge();
            loginButton.SetActive(false);
            highScoresButton.SetActive(true);
            loginPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            loggedPanel.SetActive(true);
            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            // Resetting the form for next use
            loginFormContainer.gameObject.SetActive(true);
            loginMessageContainer.gameObject.SetActive(false);
			
	        // Enbaling "New Game" button
	        newGameButton.GetComponent<Button>().interactable = true;
			// Enabling "Continue" button
            if (GameProgressManager.PlayerCanContinue()) {
                continueButton.GetComponent<Button>().interactable = true;
            }
			
	        // Reset login message panel
	        loginMessage.GetComponent<TMP_Text>().text = "Logging into your account\nplease wait...";
	        loginSpinner.gameObject.SetActive(true);
	        loginCloseButton.gameObject.SetActive(false);
        }
        else {
            loginMessage.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
            loginSpinner.gameObject.SetActive(false);
	        loginCloseButton.gameObject.SetActive(true);
        }
        // Login process done
        // Enabling menu navigation
        SessionEnableMainControls();
        // Status
        Debug.Log("Done!");
    }

    // Sends the request to update a game progress record
    private IEnumerator UpdateGameProgressRequest(string updateGameProgressJson){

        // Getting data from the UI
        Transform loggedInPanelContainer = loggedPanel.gameObject.transform.Find("LoggedInPanelContainer");
        Transform loggedInUsername = loggedInPanelContainer.Find("LoggedUsername");
        Transform loggedInMessageContainer = loggedPanel.gameObject.transform.Find("LoggedInMessageContainer");
        Transform loggedInMessage = loggedInMessageContainer.Find("LoggedInMessage");
        Transform loggedInSpinner = loggedInMessageContainer.Find("LoggedInSpinner");
        Transform loggedInCloseButton = loggedInMessageContainer.Find("LoggedInCloseButton");

        Debug.Log("Updating your game progress!...");
        UnityWebRequest request = UnityWebRequest.Put(GetServerBaseURL()+USERS_ROUTE+"/"+SessionManager.GetSessionUsername()+"/"+GAMEPROGRESS_ROUTE, updateGameProgressJson);

        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        // Show data to the user to reflect the result of the request
        showResponseData(responseDTO);

        // Enabling "New Game" and "High Scores" button
        newGameButton.GetComponent<Button>().interactable = true;
        highScoresButton.GetComponent<Button>().interactable = true;

        if (responseDTO.getResult() == UnityWebRequest.Result.Success)
        {
            // Setting up logged in panel
            loggedInMessageContainer.gameObject.SetActive(false);
            loggedInMessage.GetComponent<TMP_Text>().text = "Logging out of your account\nplease wait...";
            loggedInMessage.gameObject.SetActive(true);
            loggedInSpinner.gameObject.SetActive(true);
            loggedInCloseButton.gameObject.SetActive(false);
            loggedInUsername.GetComponent<TMP_Text>().text = SessionManager.GetSessionUsername();
            loggedInPanelContainer.gameObject.SetActive(true);

            // Change UI element status
            highScoresButton.SetActive(true);
            loginButton.SetActive(false);            
        }
        else
        {
            // Error updating game progress
            Debug.Log("Error updating game progress!");
            if (request.responseCode == (long)HttpStatusCode.Unauthorized){
                SetloggedInPanelError("ERROR\nyour session has expired");
            }
            else {
                SetloggedInPanelError("ERROR\ncould not update game progress");
            }
        }
        // Session check process done
        // Enabling menu navigation
        SessionEnableMainControls();
        // Status
        Debug.Log("Done!");
    }

    private void showResponseData(UnityWebRequestResponseDTO response) {
        Debug.LogWarning("Result: " + response.getResult());
        Debug.LogWarning("Status Code: " + response.getCode());
        Debug.LogWarning("Response: " + response.getBody());
    }

    // Updates the Game Progress level and difficulty shown on the main menu
    private void UpdateGameProgressBadge() {
        Transform gameProgressLevel = gameProgessBadge.gameObject.transform.Find("LevelFlag/CurrentLevelText");
        Transform gameProgressDifficulty = gameProgessBadge.gameObject.transform.Find("DifficultyFlag/CurrentDifficultyLevelText");
        Transform gameProgressGold = gameProgessBadge.gameObject.transform.Find("ProgressFlag/GoldAmount");
        Transform gameProgressTime = gameProgessBadge.gameObject.transform.Find("ProgressFlag/ElapsedTime");

        gameProgressLevel.GetComponent<TMP_Text>().text = "LEVEL - " + GameProgressManager.GetNextLevel().ToString();
        gameProgressDifficulty.GetComponent<TMP_Text>().text = GameProgressManager.GetDifficultyLevel().ToString();
        gameProgressGold.GetComponent<TMP_Text>().text = GameProgressManager.GetGoldCollected().ToString();
        gameProgressTime.GetComponent<TMP_Text>().text = GameProgressManager.GetTimeElapsed();
    }
}
