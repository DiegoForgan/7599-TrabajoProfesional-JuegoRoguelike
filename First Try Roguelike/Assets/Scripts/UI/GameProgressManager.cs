using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used to manage a user's game progress data
public static class GameProgressManager
{
    // Default values
    private const int DEFAULT_NEXT_LEVEL = 1;
    private const int DEFAULT_GOLD_COLLECTED = 0;
    // Use Stopwatch class to measure time and export to this string format
    // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-7.0
    private const string DEFAULT_TIME_ELAPSED = "00:00:00.000";

    // Current values
    // Game Progress
    private static int nextLevel;
    private static int difficultyLevel;
    private static int goldCollected;
    private static string timeElapsed;


    // Reads all gameprogress values from PlayerPrefs
    // If not found, assigns default
    public static void InitializeGameProgress()
    {
        nextLevel = PlayerPrefs.GetInt("gameprogress_next_level", DEFAULT_NEXT_LEVEL);
        difficultyLevel = PlayerPrefs.GetInt("gameprogress_difficulty_level", SettingsManager.GetStartingDifficulty());
        goldCollected = PlayerPrefs.GetInt("gameprogress_gold_collected", DEFAULT_GOLD_COLLECTED);
        timeElapsed = PlayerPrefs.GetString("gameprogress_time_elapsed", DEFAULT_TIME_ELAPSED);
    }

    // Sets complete gameprogress 
    public static void SetGameProgress(int newNextLevel, int newDifficultyLevel, int newGoldCollected, string newTimeElapsed)
    {
        nextLevel = newNextLevel;
        difficultyLevel = newDifficultyLevel;
        goldCollected = newGoldCollected;
        timeElapsed = newTimeElapsed;
    }

    // Resets gameprogess to initial values 
    public static void ResetGameProgress(string newSessionToken, string newSessionUsername)
    {
        nextLevel = DEFAULT_NEXT_LEVEL;
        difficultyLevel = SettingsManager.GetStartingDifficulty();
        goldCollected = DEFAULT_GOLD_COLLECTED;
        timeElapsed = DEFAULT_TIME_ELAPSED;
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
    public static int getDifficultyLevel() { return difficultyLevel; }
    // Returns the current value for setting "Gold Collected"
    public static int getGoldCollected() { return goldCollected; }
    // Returns the current value for setting "Time Elapsed"
    public static string getTimeElapsed() { return timeElapsed; }

    // Setters
    // Sets the current value for "Next Level"
    public static void SetNexLevel(int newNextLevel) { nextLevel = newNextLevel; }
    // Sets the current value for "Difficulty Level"
    public static void SetDifficultyLevel(int newDifficultyLevel) { difficultyLevel = newDifficultyLevel; }
    // Sets the current value for "Gold Collected"
    public static void SetGoldCollected(int newGoldCollected) { goldCollected = newGoldCollected; }
    // Sets the current value for "Gold Collected"
    public static void SetTimeElapsed(string newTimeElapsed) { timeElapsed = newTimeElapsed; }
}
