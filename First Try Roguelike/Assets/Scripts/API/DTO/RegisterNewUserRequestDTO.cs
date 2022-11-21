public class RegisterNewUserRequestDTO 
{
    public string username;
    public string password;
    public string first_name;
    public string last_name;
    public ContactInfoDTO contact;
    public AvatarInfoDTO avatar;

    public RegisterNewUserRequestDTO(string username, string password, string first_name, string last_name, ContactInfoDTO contact, AvatarInfoDTO avatar)
    {
        this.username = username;
        this.password = password;
        this.first_name = first_name;
        this.last_name = last_name;
        this.contact = contact;
        this.avatar = avatar;
    }
}
