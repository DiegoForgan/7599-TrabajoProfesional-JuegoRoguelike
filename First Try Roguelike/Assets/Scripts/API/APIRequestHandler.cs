using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class APIRequestHandler : MonoBehaviour
{
    private const string DEV_URL = "http://127.0.0.1/api/v1/";
    private const string QA_URL = "https://fiuba-qa-7599-cs-app-server.herokuapp.com/api/v1/";
    private const string PR_URL = "https://fiuba-pr-7599-cs-app-server.herokuapp.com/api/v1/"; // DOWN TEMPORARILY
    private const string HIGHSCORES_ROUTE = "highscores?start=0&limit=0";
    private const string SESSIONS_ROUTE = "sessions";
    private const string RECOVERY_ROUTE = "recovery";
    private const string USERS_ROUTE = "users";
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject loginBtn;
    [SerializeField] private GameObject logOutBtn;

    
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
        StartCoroutine(UserLogOutRequest());
    }

    public void ShowHighScores(){
        StartCoroutine(ShowHighScoresRequest());
    }

    private void ShowStatusMessage(string title, string message, bool shouldClose){
        if(statusPanel == null) this.statusPanel = GameObject.Find("StatusPanel");
        
        TextMeshProUGUI statusTitle = this.statusPanel.transform.Find("StatusTitle").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI statusMsg = this.statusPanel.transform.Find("StatusMessage").GetComponent<TextMeshProUGUI>();
        
        statusTitle.SetText(title);
        statusMsg.SetText(message);

        statusPanel.SetActive(true);
        
        if(closeButton == null) this.closeButton = GameObject.Find("CloseButton");
        closeButton.SetActive(shouldClose);
    }

    private void CloseStatusMessage(){
        if(statusPanel == null) this.statusPanel = GameObject.Find("StatusPanel");
        statusPanel.SetActive(false);
    }

    private IEnumerator ShowHighScoresRequest(){
        
        UnityWebRequest request = UnityWebRequest.Get(QA_URL+HIGHSCORES_ROUTE);
        
        request.SetRequestHeader("X-Auth-Token", PlayerPrefs.GetString("session_token"));
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success){
            Debug.Log("Result: " + request.result);
            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log("Response: " + request.downloadHandler.text);
            PaginatedHighScoreResponse paginatedTest = JsonUtility.FromJson<PaginatedHighScoreResponse>(request.downloadHandler.text);
            //HighScoresResponse highScoresResponse = JsonUtility.FromJson<HighScoresResponse>(paginatedTest.results);
            Debug.Log(paginatedTest.results);
        }
        else{
            ErrorAPIResponse errorResponse = JsonUtility.FromJson<ErrorAPIResponse>(request.downloadHandler.text);
            ShowStatusMessage(request.result.ToString(), errorResponse.message, true);
        }

    }
    
    private IEnumerator UserLoginRequest(){
        // Getting data from the UI
        string username = GameObject.Find("LoginUserNameInputField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("LoginPasswordInputField").GetComponent<TMP_InputField>().text;
        // Formatting JSON string
        string loginData = "{ \"username\": \""+username+"\", \"password\": \""+password+"\"}";
    
        // The UnityWebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        UnityWebRequest request = UnityWebRequest.Put(QA_URL+SESSIONS_ROUTE, loginData);
        request.method = UnityWebRequest.kHttpVerbPOST;
        
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        ShowStatusMessage("Login Status", "Logging into your account, please wait...", false);
        yield return request.SendWebRequest();
        CloseStatusMessage();
        Debug.Log("Result: " + request.result);
        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log("Response: " + request.downloadHandler.text);
        
        if (request.result == UnityWebRequest.Result.Success){
            Debug.Log("Success");
            // Formatting data to JSON.
            UserSessionData userSessionData = JsonUtility.FromJson<UserSessionData>(request.downloadHandler.text);
            // Storing user session data to use it on other API endpoints
            PlayerPrefs.SetString("session_token", userSessionData.session_token);
            PlayerPrefs.SetString("username", userSessionData.username);
            PlayerPrefs.Save();
            ShowStatusMessage("Login Succesful", "You logged in as: "+userSessionData.username, true);
            loginBtn.SetActive(false);
            logOutBtn.SetActive(true);
        }
        else{
            Debug.Log("Error");
            // Formatting data to JSON.
            ErrorAPIResponse errorResponse = JsonUtility.FromJson<ErrorAPIResponse>(request.downloadHandler.text);
            // Show data to the user to reflect the result of the request
            Debug.Log(errorResponse.code);
            Debug.Log(errorResponse.message);
            Debug.Log(errorResponse.data);
            ShowStatusMessage("Login Error", errorResponse.message, true);
        }
    }

    private bool IsUserLoggedIn(){
        return PlayerPrefs.GetString("session_token") != "";
    }

    private IEnumerator UserLogOutRequest(){
        // The UnityWebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        if(IsUserLoggedIn()){
        
            UnityWebRequest request = UnityWebRequest.Get(QA_URL+SESSIONS_ROUTE+"/"+PlayerPrefs.GetString("session_token"));
            request.method = UnityWebRequest.kHttpVerbDELETE;
        
            request.SetRequestHeader("X-Auth-Token", PlayerPrefs.GetString("session_token"));
            request.SetRequestHeader("Accept", "application/json");
        
            yield return request.SendWebRequest();
            Debug.Log("Result: " + request.result);
            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log("Response: " + request.downloadHandler.text);
        
            ErrorAPIResponse serverResponse = JsonUtility.FromJson<ErrorAPIResponse>(request.downloadHandler.text);
            // Show data to the user to reflect the result of the request
            Debug.Log(serverResponse.code);
            Debug.Log(serverResponse.message);
            
        
            if (request.result == UnityWebRequest.Result.Success){
                Debug.Log("Success");
                // Storing user session data to use it on other API endpoints
                PlayerPrefs.SetString("session_token", "");
                PlayerPrefs.SetString("username", "");
                PlayerPrefs.Save();
                loginBtn.SetActive(true);
                logOutBtn.SetActive(false);
            }
            else{
                Debug.Log("Error");
                ShowStatusMessage("LogOut Error", serverResponse.message, true);    
            }
        }
    }

    private IEnumerator ForgotPasswordRequest(){
        // Getting data from the UI
        string username = GameObject.Find("UserNameForgotInputField").GetComponent<TMP_InputField>().text;
        TextMeshProUGUI mailSentUI = GameObject.Find("MailSentMsg").GetComponent<TextMeshProUGUI>();
        string passwordRecoveryData = "{ \"username\": \""+username+"\" }";
        
        UnityWebRequest request = UnityWebRequest.Put(QA_URL+RECOVERY_ROUTE, passwordRecoveryData);
        request.method = UnityWebRequest.kHttpVerbPOST;
        
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success){
            Debug.Log("Success");
            // Formatting data to JSON.
            UserRecoveryData recoveryData = JsonUtility.FromJson<UserRecoveryData>(request.downloadHandler.text);
            // Masks the current user recovery email address address@hosting.com => *****ss@hosting.com
            string input = recoveryData.email;
            string pattern = @".(?=.*..@)";     //@"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";
            string maskedEmail = Regex.Replace(input, pattern, m => new string('*', m.Length));
            // Shows the user the correct UI
            mailSentUI.SetText("Recovery mail sent to: "+maskedEmail+"\n Check your account to complete the process");
        }
        else{
            // Shows the user the correct UI
            mailSentUI.SetText("Invalid Username");
            Debug.Log("Result: " + request.result);
            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log("Response: " + request.downloadHandler.text);
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
        //Debug.Log("Result: " + request.result);
        //Debug.Log("Status Code: " + request.responseCode);
        //Debug.Log("Response: " + request.downloadHandler.text);
        
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
        return "{ \"username\": \""+newUsername+"\", \"password\": \""+newPassword+"\", \"first_name\": \""+newFirstName+"\", \"last_name\": \""+newLastName+"\", \"contact\": { \"email\": \""+newEmail+"\", \"phone\": \""+newPhone+"\" }, \"avatar\": { \"isUrl\": "+isUrl.ToString().ToLower()+", \"data\": \""+avatarUrl+"\" }}"; 
        
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

   
}
