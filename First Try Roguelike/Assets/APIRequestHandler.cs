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
    private string testString = "{ \"username\": \"juan05146\", \"password\": \"123456\", \"first_name\": \"Damián\", \"last_name\": \"Marquesín Fernandez\", \"contact\": { \"email\": \"juanmg0511@gmail.com\", \"phone\": \"5555 5555\" }}";

    public void CheckUsernameAlreadyTaken(){
        StartCoroutine(CheckUsernameRequest());
    }

    public void TestRegisterNewUser(){
        StartCoroutine(RegisterNewUserRequest());
    }

    private IEnumerator CheckUsernameRequest(){
        // Getting data from the UI
        TMP_InputField field = GameObject.Find("UserNameInputField").GetComponent<TMP_InputField>();
        string userToCheck = field.text;
        if (userToCheck == "") yield break;
        Debug.Log("Checking if username is already taken...");
        // Preparing the GET request
        UnityWebRequest request = UnityWebRequest.Get(QA_URL+"/"+userToCheck+"/exists");
        yield return request.SendWebRequest();
        // Processing the response
        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log("Response: " + request.downloadHandler.text);
    }

    /*private IEnumerator RegisterNewUser(string username, string password, string first_name, string last_name,
     string email, string phone, bool isUrl, string data, bool login_service){
        Debug.Log("Register New User with parameters...");
        
    }*/

    private string CreateNewUserDataJSON(string newuser){
        return "{ \"username\": \""+newuser+"\", \"password\": \"123456\", \"first_name\": \"Damián\", \"last_name\": \"Marquesín Fernandez\", \"contact\": { \"email\": \"juanmg0511@gmail.com\", \"phone\": \"5555 5555\" }}";
    }

    private IEnumerator RegisterNewUserRequest(){
        Debug.Log("Registering new user without parameters to the game...");
        Debug.Log(CreateNewUserDataJSON("diegote"));
        // The unitywebRequest library its pretty tricky, for POST method you should start with PUT and then change it on the next lines
        // Implementation based on the tutorial found at https://manuelotheo.com/uploading-raw-json-data-through-unitywebrequest/
        UnityWebRequest request = UnityWebRequest.Put(QA_URL, testString);
        request.method = UnityWebRequest.kHttpVerbPOST;
        
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        yield return request.SendWebRequest();
        
        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log("Response: " + request.downloadHandler.text);
    }

   
}
