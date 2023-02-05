using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;

// This class handles the communication with the FIUBA CloudSync API
// Temporarily, it also controls the Main Menu UI after an async call is resolved
// TODO: separate Request and UI handling responsibilites 
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
    private const string DEFAULT_AVATAR_URL = "https://ui-avatars.com/api/?background=1A82E2&color=FFFFFF&size=256&name=";
    private const string HIGHSCORES_ROUTE = "highscores?start=0&limit=50&sort_column=difficulty_level,achieved_level,gold_collected,time_elapsed&sort_order=-1,-1,-1,1";
    private const string SESSIONS_ROUTE = "sessions";
    private const string RECOVERY_ROUTE = "recovery";
    private const string USERS_ROUTE = "users";

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
    [SerializeField] private GameObject resetPasswordMenu;


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
        StartCoroutine(RegisterNewUserRequest());
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
            StartCoroutine(UserLoginRequest());
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
    public void ShowHighScores(){
        StartCoroutine(ShowHighScoresRequest());
    }
    // Gets the user's game progress information
    // This is used as a way to save the game and sync it with multiple devices
    public void GetUserGameProgress() {
        if (SessionManager.IsUserLoggedIn()) StartCoroutine(GetGameProgress());
        else Debug.LogWarning("User Session not found, cant get progress");
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

        // Enabling "New Game" and "High Scores" button
        newGameButton.GetComponent<Button>().interactable = true;
        highScoresButton.GetComponent<Button>().interactable = true;

        if (responseDTO.getResult() == UnityWebRequest.Result.Success)
        {
            // Session is still valid, and now is renwed
            Debug.Log("Valid session");

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
            // Session is no longer valid, show error message
            Debug.Log("Invalid session!");
            SetloggedInPanelError("ERROR\nyour session has expired");
        }
        // Session check process done
        // Enabling menu navigation
        SessionEnableMainControls();
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

    private IEnumerator GetGameProgress()
    {
        UnityWebRequest request = UnityWebRequest.Get(GetServerBaseURL()+USERS_ROUTE+"/"+SessionManager.GetSessionUsername()+"/gameprogress");
        
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("X-Auth-Token", SessionManager.GetSessionToken());
        
        yield return request.SendWebRequest();
        
        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);

        GameProgressResponseDTO gameProgress = JsonConvert.DeserializeObject<GameProgressResponseDTO>(responseDTO.getBody());
        
        Debug.Log("Gold Collected: " + gameProgress.getGoldCollected());
        Debug.Log("Next Level: " + gameProgress.getNextLevel());
        Debug.Log("Difficulty Level: " + gameProgress.getDifficultyLevel());
        Debug.Log("Elapsed Time: " + gameProgress.getTimeElapsed());
    }

    private IEnumerator ShowHighScoresRequest(){

        Transform highScoresTableMessageText = highScoresTableMessage.gameObject.transform.Find("HighscoresTableMessageText");
        Transform highScoresTableMessageSpinner = highScoresTableMessage.gameObject.transform.Find("HighscoresTableSpinner");
        
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
            // If the result was "Unauthorized", we show the session expired message in the logged in panel
            if (request.responseCode == 401)
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
        // Enbaling "New Game" button
        newGameButton.GetComponent<Button>().interactable = true;
        if (responseDTO.getResult() == UnityWebRequest.Result.Success){
            Debug.Log("Success");
            // Formatting data to JSON.
            LoginResponseDTO loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseDTO.getBody());
            // Storing user session data to use it on other API endpoints
            SessionManager.SetSession(loginResponse.getSessionToken(), loginResponse.getUsername());
            Debug.Log("Logged In!");
            loginButton.SetActive(false);
            highScoresButton.SetActive(true);
            loginPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            loggedPanel.SetActive(true);
            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            // Resetting the form for next use
            loginFormContainer.gameObject.SetActive(true);
            loginMessageContainer.gameObject.SetActive(false);
            setLoggedPanel(loginResponse);
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
            if (request.responseCode == 400 || request.responseCode == 401)
            {
                loginMessage.GetComponent<TMP_Text>().text = "ERROR\nWrong user or password";
            }
            else
            {
                loginMessage.GetComponent<TMP_Text>().text = "ERROR\nPlease try again later";
            }
            loginSpinner.gameObject.SetActive(false);
            loginCloseButton.gameObject.SetActive(true);
        }
        // Login process done
        // Enabling menu navigation
        SessionEnableMainControls();
    }

    private void setLoggedPanel(LoginResponseDTO loginResponse)
    {
        GameObject.Find("LoggedUsername").GetComponent<TextMeshProUGUI>().SetText(loginResponse.getUsername());
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

            // Storing user session data to use it on other API endpoints
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
            string pattern = @".(?=.*..@)";
            string maskedEmail = Regex.Replace(input, pattern, m => new string('*', m.Length));
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

    private string GetJsonStringRegisterData(){
        // Getting all register data from input fields (maybe should optimize this!)
        string newUsername = GameObject.Find("UserNameInputField").GetComponent<TMP_InputField>().text;
        string newPassword = GameObject.Find("PasswordInputField").GetComponent<TMP_InputField>().text;
        string newFirstName = GameObject.Find("FirstNameInputField").GetComponent<TMP_InputField>().text;
        string newLastName = GameObject.Find("LastNameInputField").GetComponent<TMP_InputField>().text;
        string newEmail = GameObject.Find("EmailInputField").GetComponent<TMP_InputField>().text;
        string newPhone = GameObject.Find("PhoneInputField").GetComponent<TMP_InputField>().text;
        if(newPhone == "") newPhone = "-1";
        // TO DO: Add custom Avatar support, in the meantime default avatar is set
        bool isUrl = true;
        string avatarUrl = DEFAULT_AVATAR_URL + newFirstName + "+" + newLastName;
        //TO DO: ADD missing info to the json -> Avatar and login service
        //return "{ \"username\": \""+newUsername+"\", \"password\": \""+newPassword+"\", \"first_name\": \""+newFirstName+"\", \"last_name\": \""+newLastName+"\", \"contact\": { \"email\": \""+newEmail+"\", \"phone\": \""+newPhone+"\" }, \"avatar\": { \"isUrl\": "+isUrl.ToString().ToLower()+", \"data\": \""+avatarUrl+"\" }}"; 
        RegisterNewUserRequestDTO registerNewUserRequestDTO = new(newUsername,newPassword,newFirstName,newLastName,new(newEmail,newPhone),new(isUrl,avatarUrl));
        return JsonConvert.SerializeObject(registerNewUserRequestDTO);
    }

    private IEnumerator RegisterNewUserRequest(){
        Debug.Log("Registering new user to the game...");
        string registerDataJson = GetJsonStringRegisterData();
        // The UnityWebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        UnityWebRequest request = UnityWebRequest.Put(GetServerBaseURL()+USERS_ROUTE, registerDataJson);
        request.method = UnityWebRequest.kHttpVerbPOST;
        
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();
        
        //Response handling
        if(request.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Missing data to fullfill the request or User already exists");
        }
        else if(request.result == UnityWebRequest.Result.Success){
            Debug.Log("Success");
        }
        else Debug.Log(request.result);
        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log("Response: " + request.downloadHandler.text);
        Debug.Log(request.result);
    }

    private void showResponseData(UnityWebRequestResponseDTO response) {
        Debug.LogWarning("Result: " + response.getResult());
        Debug.LogWarning("Status Code: " + response.getCode());
        Debug.LogWarning("Response: " + response.getBody());
    }
}
