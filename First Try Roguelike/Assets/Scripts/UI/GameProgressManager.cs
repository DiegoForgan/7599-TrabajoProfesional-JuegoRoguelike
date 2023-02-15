using System;
using UnityEngine;
using Newtonsoft.Json;

// This class is used to manage a user's Game Progress data
// Use Stopwatch class to measure time and export to TimeSpan objects for use with this class
// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-7.0
public static class GameProgressManager
{
    // Default values
    private const int DEFAULT_NEXT_LEVEL = 1;
    private const int DEFAULT_GOLD_COLLECTED = 0;
    private const int LOWEST_DIFFICULTY_LEVEL = 1;
    private const int HIGHEST_DIFFICULTY_LEVEL = 10;

    // Current values
    // Game Progress
    private static int nextLevel;
    private static int difficultyLevel;
    private static int goldCollected;
    // Since PlayerPrefs and the FIUBA CloudSync API store text, we use strings for storing and persisting this property
    // Time measurements must be passed to this class' setters as TimeSpan objects
    private static string timeElapsed;
    // Use this to mark if the user beat level 10, at any difficulty level
    // This is used so that the final message can be shown!
    private static bool finishedGame = false;


    // Converts a TimeSpan object to a format compatible with the gameprogress format 
    public static string FormatTimeSpanAsString(TimeSpan timeToConvert) {

        return String.Format("{0:00}:{1:00}:{2:00}.{3:000}", timeToConvert.Hours, timeToConvert.Minutes, timeToConvert.Seconds, timeToConvert.Milliseconds / 10);
    }

    // Reads all gameprogress values from PlayerPrefs
    // If not found, assigns default
    public static void InitializeGameProgress()
    {
        nextLevel = PlayerPrefs.GetInt("gameprogress_next_level", DEFAULT_NEXT_LEVEL);
        difficultyLevel = PlayerPrefs.GetInt("gameprogress_difficulty_level", SettingsManager.GetStartingDifficulty());
        goldCollected = PlayerPrefs.GetInt("gameprogress_gold_collected", DEFAULT_GOLD_COLLECTED);
        timeElapsed = PlayerPrefs.GetString("gameprogress_time_elapsed", FormatTimeSpanAsString(TimeSpan.Zero));
    }

    // Resets gameprogess to initial values 
    public static void ResetGameProgress()
    {
        nextLevel = DEFAULT_NEXT_LEVEL;
        difficultyLevel = SettingsManager.GetStartingDifficulty();
        goldCollected = DEFAULT_GOLD_COLLECTED;
        timeElapsed = FormatTimeSpanAsString(TimeSpan.Zero);
    }

    public static bool PlayerCanContinue() {

        TimeSpan ts = TimeSpan.Parse(timeElapsed);
        return ts > TimeSpan.Zero;
    }

    // Saves all current session values to PlayerPrefs
    public static void PersistGameProgress()
    {
        PlayerPrefs.SetInt("gameprogress_next_level", nextLevel);
        PlayerPrefs.SetInt("gameprogress_difficulty_level", difficultyLevel);
        PlayerPrefs.SetInt("gameprogress_gold_collected", goldCollected);
        PlayerPrefs.SetString("gameprogress_time_elapsed", timeElapsed);

        PlayerPrefs.Save();
    }

    // Getters
    // Returns the current value for "Next Level"
    public static int GetNextLevel() { return nextLevel; }
    // Returns the current value for setting "Difficulty Level"
    public static int GetDifficultyLevel() { return difficultyLevel; }
    // Returns the current value for setting "Gold Collected"
    public static int GetGoldCollected() { return goldCollected; }
    // Returns the current value for setting "Time Elapsed"
    public static string GetTimeElapsed() { return timeElapsed; }
    // Returns the current value for constant "LOWEST_DIFFICULTY_LEVEL"
    public static int GetLowestDifficultyLevel() { return LOWEST_DIFFICULTY_LEVEL; }
    // Returns the current value for constant "HIGHEST_DIFFICULTY_LEVEL"
    public static int GetHighestDifficultyLevel() { return HIGHEST_DIFFICULTY_LEVEL; }
    // Returns the current value for constant "DEFAULT_NEXT_LEVEL"
    public static int GetDefaultNextLevel() { return DEFAULT_NEXT_LEVEL; }

    // Setters
    // Sets the current value for "Next Level"
    public static void SetNexLevel(int newNextLevel) { nextLevel = newNextLevel; }
    // Sets the current value for "Difficulty Level"
    public static void SetDifficultyLevel(int newDifficultyLevel) { difficultyLevel = newDifficultyLevel; }
    // Sets the current value for "Gold Collected"
    public static void SetGoldCollected(int newGoldCollected) { goldCollected = newGoldCollected; }

    // Sets complete gameprogress
    // Time must be passed as a TimeSpan object, and is added to the current value!
    public static void SetGameProgress(int newNextLevel, int newDifficultyLevel, int newGoldCollected, TimeSpan timeToAdd)
    {
        nextLevel = newNextLevel;
        difficultyLevel = newDifficultyLevel;
        goldCollected = newGoldCollected;

        TimeSpan currentTime = TimeSpan.Parse(timeElapsed);
        TimeSpan totaltime = currentTime + timeToAdd;
        timeElapsed = FormatTimeSpanAsString(totaltime);
    }

    // Adds a timeToAdd to the current time
    public static void AddTimeElapsed(TimeSpan timeToAdd)
    {
        TimeSpan currentTime = TimeSpan.Parse(timeElapsed);
        TimeSpan totaltime = currentTime + timeToAdd;
        timeElapsed = FormatTimeSpanAsString(totaltime);
    }

    // Resets time elapsed to default value
    public static void ResetTimeElapsed()
    {
        timeElapsed = FormatTimeSpanAsString(TimeSpan.Zero);
    }

    // Logs game progress data
    public static void LogGameProgressData() {

        Debug.Log(
            "GameProgress data\n" +
            "  next_level: " + GameProgressManager.GetNextLevel() + "\n" +
            "  difficulty_level: " + GameProgressManager.GetDifficultyLevel() + "\n" +
            "  gold_collected: " + GameProgressManager.GetGoldCollected() + "\n" +
            "  time_elapsed: " + GameProgressManager.GetTimeElapsed() + "\n"
        );
    }

    // Setter for finishedGame: always sets to *true*
    // Used to signal a game where level 10 has been beat
    public static void SetFinishedGame() {
        finishedGame = true;
    }
    // Shoud be re-setted to false after excecuting post game actions!
    public static void ResetFinishedGame() {
        finishedGame = false;
    }
    // Query for game finished condition
    public static bool IsFinishedGame() {
        return finishedGame;
    }

    // Generates a json object for an empty gameprogress record
    // Ready to be posted to the server
    public static string GetJsonStringUpdateGameProgress(string newUsername){
 
        // Custom DTO class for registration
        // Id and dates are sent as nulls
        UpdateGameProgressRequestDTO updateGameProgressRequestDTO = new(
            DEFAULT_NEXT_LEVEL,
            SettingsManager.GetStartingDifficulty(),
            DEFAULT_GOLD_COLLECTED,
            FormatTimeSpanAsString(TimeSpan.Zero)
        );

        // Generate the output
        return JsonConvert.SerializeObject(updateGameProgressRequestDTO);
    }
}
