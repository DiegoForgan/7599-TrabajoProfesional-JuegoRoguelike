using System.Text.RegularExpressions;

// Use this class to validate data input into forms
// Using same definitions as FIUBA CloudSync WebAdmin
public static class FormDataValidation
{
    private const string GUID_REGEX = @"^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$";
    private const string URL_REGEX = @"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$";    
    private const string USERNAME_REGEX = @"^[a-zA-Z0-9_.]+$";
    private const string NAME_REGEX = @"(.|\s)*\S(.|\s)*";
    private const string EMAIL_REGEX = @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$";
    private const string PASSWORD_REGEX = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$";
 
    public static bool IsValidGuid(string guid) {
        return Regex.IsMatch(guid, GUID_REGEX, RegexOptions.IgnoreCase);
    }

    public static bool IsValidURL(string url) {
        return Regex.IsMatch(url, URL_REGEX, RegexOptions.IgnoreCase);
    }

    public static bool IsValidUsername(string username) {
        return Regex.IsMatch(username, USERNAME_REGEX, RegexOptions.IgnoreCase);
    }

    public static bool IsValidName(string name) {
        return Regex.IsMatch(name, NAME_REGEX, RegexOptions.IgnoreCase);
    }

    public static bool IsValidEmail(string email) {
        return Regex.IsMatch(email, EMAIL_REGEX, RegexOptions.IgnoreCase);
    }

    public static bool IsValidPassword(string password) {
        return Regex.IsMatch(password, PASSWORD_REGEX, RegexOptions.IgnoreCase);
    }
}
