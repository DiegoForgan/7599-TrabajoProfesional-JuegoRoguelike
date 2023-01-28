using UnityEngine;

// This class is used to manage a user's session data (token and username)
public static class SessionManager
{
    // Default values
    private const string defaultSessionToken = "";
    private const string defaultSessionUsername = "";

    // Current values
    // Session
    private static string sessionToken;
    private static string sessionUsername;

    // Reads all session values from PlayerPrefs
    // If not found, assigns default
    public static void InitializeSession()
    {
        sessionToken = PlayerPrefs.GetString("session_token", defaultSessionToken);
        sessionUsername = PlayerPrefs.GetString("session_username", defaultSessionUsername);
    }

    // Saves all current session values to PlayerPrefs
    public static void PersistSession()
    {
        PlayerPrefs.SetString("session_token", sessionToken);
        PlayerPrefs.SetString("session_username", sessionUsername);

        PlayerPrefs.Save();
    }

    // Returns if there is a user logged in
    // Session must be checked against server, as the token may be exired!
    public static bool IsUserLoggedIn() { return (sessionToken != defaultSessionToken); }

    // Sets session state with opened in session 
    public static void SetSession(string newSessionToken, string newSessionUsername)
    {
        sessionToken = newSessionToken;
        sessionUsername = newSessionUsername;
    }

    // Sets session state for no user logged in
    public static void ClearSession()
    {
        sessionToken = defaultSessionToken;
        sessionUsername = defaultSessionUsername;
    }

    // Getters
    // Returns the current value for "Session Token"
    public static string GetSessionToken() { return sessionToken; }
    // Returns the current value for setting "Session Username"
    public static string GetSessionUsername() { return sessionUsername; }

    // Setters
    // Sets the current value for "Session Token"
    public static void SetSessionToken(string newSessionToken) { sessionToken = newSessionToken; }
    // Sets the current value for "Session Username"
    public static void SetSessionUsername(string newSessionUsername) { sessionUsername = newSessionUsername; }
}
