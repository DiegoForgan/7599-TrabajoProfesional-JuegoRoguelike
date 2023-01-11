public class LoginResponseDTO
{
    public string username;
    public string user_role;
    public string session_token;
    public string expires;
    public string date_created;
    public string id;

    public string getUsername() {
        return this.username;
    }
    public string getUserRole()
    {
        return this.user_role;
    }
    public string getSessionToken()
    {
        return this.session_token;
    }
    public string getExpireDate()
    {
        return this.expires;
    }
    public string getDateCreated()
    {
        return this.date_created;
    }
    public string getId()
    {
        return this.id;
    }

    public void setUsername(string username) { this.username = username; }
    public void setUserRole(string userRole) { this.user_role = userRole; }
    public void setSessionToken(string sessionToken) { this.session_token = sessionToken; }
    public void setExpireDate(string expireDate) { this.expires = expireDate; }
    public void setDateCreated(string dateCreated) { this.date_created = dateCreated; }
    public void setId(string id) { this.id = id; }
}
