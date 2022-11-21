public class AvatarInfoDTO
{
    public bool isUrl;
    public string data;

    public AvatarInfoDTO(bool isUrl, string data)
    {
        this.isUrl = isUrl;
        this.data = data;
    }
}