using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;



public class RegisterData
{
    public string username;
    public string password;
    public string first_name;
    public string last_name;
    public ContactData contact;
    public string avatar;
    public bool login_service;

    public RegisterData(string usr, string pass, string first, string last, string email, string phone,
        bool isurl, string data, bool login){
        
        username = usr;
        password = pass;
        first_name = first;
        last_name = last;
        //contact = new ContactData(email, phone);
        Debug.Log(contact);
        //avatar = JsonUtility.ToJson(new AvatarData(isurl, data));
        //Debug.Log(avatar);
        login_service = login;
    }
}



public class ContactData
{
    public string email;
    public string phone;

    public ContactData(string e, string p){
        email = e;
        phone = p;
    }
}

public class AvatarData
{
    public bool isUrl;
    public string data;

    public AvatarData(bool u, string d){
        isUrl = u;
        data = d;
    }
}


public class APIRequestHandler : MonoBehaviour
{
    private const string QA_URL = "http://fiuba-qa-7599-cs-app-server.herokuapp.com/api/v1/users";
    private const string DEV_URL = ""; // TO DEFINE
    private string testString = "{ \"username\": \"juan05146\", \"password\": \"123456\", \"first_name\": \"Damián\", \"last_name\": \"Marquesín Fernandez\", \"contact\": { \"email\": \"juanmg0511@gmail.com\", \"phone\": \"5555 5555\" }}";

    public void TestGetEndpoint(){
        StartCoroutine(MakeGetRequest());
    }

    public void TestRegisterNewUser(){
        StartCoroutine(RegisterNewUserRequest());
    }

    private IEnumerator MakeGetRequest(){
        Debug.Log("Sending GET request to QA server...");
        string userQuery = "";
        UnityWebRequest request = UnityWebRequest.Get(QA_URL+"/juan0511/exists");
        //UnityWebRequest request = UnityWebRequest.Get(QA_URL+"/"+userQuery+"/exists");
        yield return request.SendWebRequest();
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
