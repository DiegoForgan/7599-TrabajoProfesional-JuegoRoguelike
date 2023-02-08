public class ChangeUserPasswordRequestDTO 
{
    public string op;
    public string path;
    public string value;

    public ChangeUserPasswordRequestDTO(string newPassword)
    {
        this.op = "replace";
        this.path = "/password";
        this.value = newPassword;
    }
}
