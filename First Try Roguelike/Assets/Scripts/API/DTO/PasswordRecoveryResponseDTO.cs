

using System;

public class PasswordRecoveryResponseDTO 
{
    public string username;
    public string email;
    public string recovery_key;
    public string expires;
    public string date_created;
    public string id;

    public PasswordRecoveryResponseDTO(string username, string email, string recovery_key, string expires, string date_created, string id)
    {
        this.username = username;
        this.email = email;
        this.recovery_key = recovery_key;
        this.expires = expires;
        this.date_created = date_created;
        this.id = id;
    }

    public string getEmail()
    {
        return this.email;
    }
}
