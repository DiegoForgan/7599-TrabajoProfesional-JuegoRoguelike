public class LoginRequestDTO
{
    public string username;
    public string password;

    public LoginRequestDTO(string username, string password) {
        this.username = username;
        this.password = password;
    }

    public string getUsername() { return this.username; }
    public string getPassword() { return this.password; }

    public void setUsername(string username) {
        this.username = username;
    }

    public void setPassword(string password) {
        this.password = password;
    }

}
