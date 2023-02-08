public class ChangeUserAvatarRequestDTO 
{
    public string op;
    public string path;
    public AvatarInfoDTO value;

    public ChangeUserAvatarRequestDTO(AvatarInfoDTO newAvatar)
    {
        this.op = "replace";
        this.path = "/avatar";
        this.value = newAvatar;
    }
}
