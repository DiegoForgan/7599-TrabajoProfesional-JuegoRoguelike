public class PasswordRecoveryRequestDTO 
{
    public string username;

    public PasswordRecoveryRequestDTO(string username)
    {
        this.username = username;
    }
    public string getUsername() { return this.username; }
    public void setUsername(string username) { this.username = username; }
}
