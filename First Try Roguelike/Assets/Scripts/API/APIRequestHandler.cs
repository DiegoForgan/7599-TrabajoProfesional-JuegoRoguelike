using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

public class APIRequestHandler : MonoBehaviour
{
    private const string DEV_URL = "http://127.0.0.1/api/v1/";
    private const string PR_URL = "https://app.7599-fiuba-cs.net/api/v1/"; // DOWN TEMPORARILY
    private const string QA_URL = "https://app-qa.7599-fiuba-cs.net/api/v1/";
    private const string HIGHSCORES_ROUTE = "highscores?start=0&limit=50&sort_column=difficulty_level,achieved_level,gold_collected,time_elapsed&sort_order=-1,-1,-1,1";
    private const string SESSIONS_ROUTE = "sessions";
    private const string RECOVERY_ROUTE = "recovery";
    private const string USERS_ROUTE = "users";
    [SerializeField] private GameObject loggedPanel;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject highScoresButton;
    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject highScoresTableMessage;
    [SerializeField] private GameObject highScoresEntryContainer;
    [SerializeField] private GameObject highScoresEntryTemplate;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void CheckUsernameAlreadyTaken(){
        StartCoroutine(CheckUsernameRequest());
    }

    public void RegisterNewUser(){
        StartCoroutine(RegisterNewUserRequest());
    }

    public void ForgotPassword(){
        StartCoroutine(ForgotPasswordRequest());
    }

    public void UserLogin(){
        StartCoroutine(UserLoginRequest());
    }

    public void UserLogOut(){
        if (SessionManager.IsUserLoggedIn()) StartCoroutine(UserLogOutRequest());
        else Debug.LogWarning("Cant logout user because is already logged out!");
    }

    public void ShowHighScores(){
        StartCoroutine(ShowHighScoresRequest());
    }

    public void GetUserGameProgress() {
        if (SessionManager.IsUserLoggedIn()) StartCoroutine(GetGameProgress());
        else Debug.LogWarning("User Session not found, cant get progress");
    }

    private IEnumerator GetGameProgress()
    {
        UnityWebRequest request = UnityWebRequest.Get(QA_URL + USERS_ROUTE + "/" + SessionManager.GetSessionUsername() + "/gameprogress");
        
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

    //private void ShowStatusMessage(string title, string message, bool shouldClose){
    //    if(statusPanel == null) this.statusPanel = GameObject.Find("StatusPanel");
    //    
    //    TextMeshProUGUI statusTitle = this.statusPanel.transform.Find("StatusTitle").GetComponent<TextMeshProUGUI>();
    //    TextMeshProUGUI statusMsg = this.statusPanel.transform.Find("StatusMessage").GetComponent<TextMeshProUGUI>();
    //    
    //    statusTitle.SetText(title);
    //    statusMsg.SetText(message);
    //
    //    statusPanel.SetActive(true);
    //    
    //    if(closeButton == null) this.closeButton = GameObject.Find("CloseButton");
    //    closeButton.SetActive(shouldClose);
    //}

    //private void CloseStatusMessage(){
    //    if(statusPanel == null) this.statusPanel = GameObject.Find("StatusPanel");
    //    statusPanel.SetActive(false);
    //}

    private IEnumerator ShowHighScoresRequest(){

        Transform highScoresTableMessageText = highScoresTableMessage.gameObject.transform.Find("HighscoresTableMessageText");
        Transform highScoresTableMessageSpinner = highScoresTableMessage.gameObject.transform.Find("HighscoresTableSpinner");
        
        UnityWebRequest request = UnityWebRequest.Get(QA_URL+HIGHSCORES_ROUTE);
        
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
            //ShowStatusMessage(request.result.ToString(), errorResponse.message, true);
            highScoresTableMessageText.GetComponent<TextMeshProUGUI>().text = "Error fetching data!";
            highScoresTableMessageSpinner.gameObject.SetActive(false);
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

        // Formatting JSON string
        LoginRequestDTO loginRequest = new(username, password);
        string loginRequestJSON = JsonConvert.SerializeObject(loginRequest);
     
        // The UnityWebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        UnityWebRequest request = UnityWebRequest.Put(QA_URL+SESSIONS_ROUTE, loginRequestJSON);
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
            APIErrorResponseDTO errorResponse = JsonConvert.DeserializeObject<APIErrorResponseDTO>(responseDTO.getBody());
            // Show data to the user to reflect the result of the request
            Debug.Log(errorResponse.getCode());
            Debug.Log(errorResponse.getMessage());
            Debug.Log(errorResponse.getData());
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
    }

    private void setLoggedPanel(LoginResponseDTO loginResponse)
    {
        GameObject.Find("LoggedUsername").GetComponent<TextMeshProUGUI>().SetText(loginResponse.getUsername());
    }

    private IEnumerator UserLogOutRequest(){
        // The UnityWebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        if(SessionManager.IsUserLoggedIn()){
        
            UnityWebRequest request = UnityWebRequest.Get(QA_URL+SESSIONS_ROUTE+"/"+SessionManager.GetSessionToken());
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
        
            if (request.result == UnityWebRequest.Result.Success){
                Debug.Log("Success");
            }
            else{
                Debug.Log("Error");
            }

            // Storing user session data to use it on other API endpoints
            SessionManager.ClearSession();
            loginButton.SetActive(true);
            highScoresButton.SetActive(false);
            loggedPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            loggedPanel.SetActive(false);
            loginPanel.GetComponent<Animator>().SetTrigger("ShowOrHide");
            // Resseting username and password fields
            GameObject.Find("UsernameInputField").GetComponent<TMP_InputField>().text = "";
            GameObject.Find("PasswordInputField").GetComponent<TMP_InputField>().text = "";
        }
    }

    private IEnumerator ForgotPasswordRequest(){
        // Getting data from the UI
        string username = GameObject.Find("UserNameForgotInputField").GetComponent<TMP_InputField>().text;
        //TextMeshProUGUI mailSentUI = GameObject.Find("MailSentMsg").GetComponent<TextMeshProUGUI>();
 
        PasswordRecoveryRequestDTO passwordRecoveryDTO = new(username);
        string passwordRecoveryBody = JsonConvert.SerializeObject(passwordRecoveryDTO);
        
        UnityWebRequest request = UnityWebRequest.Put(QA_URL+RECOVERY_ROUTE, passwordRecoveryBody);
        request.method = UnityWebRequest.kHttpVerbPOST;
        
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();

        UnityWebRequestResponseDTO responseDTO = new(request);
        showResponseData(responseDTO);

        if (responseDTO.getResult() == UnityWebRequest.Result.Success){
            Debug.Log("Success");
            // Formatting data to JSON.
            PasswordRecoveryResponseDTO passwordRecoveryResponse = JsonConvert.DeserializeObject<PasswordRecoveryResponseDTO>(responseDTO.getBody());
            // Masks the current user recovery email address address@hosting.com => *****ss@hosting.com
            string input = passwordRecoveryResponse.getEmail();
            string pattern = @".(?=.*..@)";     //@"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";
            string maskedEmail = Regex.Replace(input, pattern, m => new string('*', m.Length));
            // Shows the user the correct UI
            //mailSentUI.SetText("Recovery mail sent to: "+maskedEmail+"\n Check your account to complete the process");
        }
        else{
            // Shows the user the correct UI
            //mailSentUI.SetText("Invalid Username");
            //Debug.Log("Result: " + request.result);
            //Debug.Log("Status Code: " + request.responseCode);
            //Debug.Log("Response: " + request.downloadHandler.text);
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
        UnityWebRequest request = UnityWebRequest.Get(QA_URL+USERS_ROUTE+"/"+userToCheck+"/exists");
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
        string avatarUrl = "https://ui-avatars.com/api/?background=1A82E2&color=FFFFFF&size=256&name="+newFirstName+"+"+newLastName;
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
        UnityWebRequest request = UnityWebRequest.Put(QA_URL+USERS_ROUTE, registerDataJson);
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
