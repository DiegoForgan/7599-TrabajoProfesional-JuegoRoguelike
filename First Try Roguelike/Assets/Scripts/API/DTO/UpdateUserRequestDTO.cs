public class UpdateUserRequestDTO
{
    public string first_name;
    public string last_name;
    public ContactInfoDTO contact;

    public UpdateUserRequestDTO(string new_first_name, string new_last_name, ContactInfoDTO contact)
    {
        this.first_name = new_first_name;
        this.last_name = new_last_name;
        this.contact = contact;
    }
}
