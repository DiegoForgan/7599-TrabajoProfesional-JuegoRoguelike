using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class APIRequestHandler : MonoBehaviour
{
    private const string QA_URL = "http://fiuba-qa-7599-cs-app-server.herokuapp.com/api/v1/users";
    private const string DEV_URL = ""; // TO DEFINE
    //private string testString = "{ \"username\": \"juan05146\", \"password\": \"123456\", \"first_name\": \"Damián\", \"last_name\": \"Marquesín Fernandez\", \"contact\": { \"email\": \"juanmg0511@gmail.com\", \"phone\": \"5555 5555\" }}";

    public void CheckUsernameAlreadyTaken(){
        StartCoroutine(CheckUsernameRequest());
    }

    public void RegisterNewUser(){
        StartCoroutine(RegisterNewUserRequest());
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
        UnityWebRequest request = UnityWebRequest.Get(QA_URL+"/"+userToCheck+"/exists");
        yield return request.SendWebRequest();
        //Debug.Log("Result: " + request.result);
        //Debug.Log("Status Code: " + request.responseCode);
        //Debug.Log("Response: " + request.downloadHandler.text);
        
        // Processing the response
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
        //TO DO: ADD missing info to the json -> Avatar and login service
        return "{ \"username\": \""+newUsername+"\", \"password\": \""+newPassword+"\", \"first_name\": \""+newFirstName+"\", \"last_name\": \""+newLastName+"\", \"contact\": { \"email\": \""+newEmail+"\", \"phone\": \""+newPhone+"\" }}";
    }

    private IEnumerator RegisterNewUserRequest(){
        Debug.Log("Registering new user to the game...");
        string registerDataJson = GetJsonStringRegisterData();
        // The UnityWebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        UnityWebRequest request = UnityWebRequest.Put(QA_URL, registerDataJson);
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
        //Debug.Log("Status Code: " + request.responseCode);
        //Debug.Log("Response: " + request.downloadHandler.text);
        //Debug.Log(request.result);
    }

   
}
