using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSessionData{
    public string username;   
    public string user_role;
    public string session_token;
    public string expires;
    public string date_created;
    public string id;
}

public class UserRecoveryData{
    public string username;
    public string email;   
    public string recovery_key;
    public string expires;
    public string date_created;
    public string id;
}

public class ErrorAPIResponse{
    public int code; 
    public string message;
    public string data;
}
