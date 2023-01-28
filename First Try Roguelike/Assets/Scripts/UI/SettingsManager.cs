using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class has all application settings, and the default values
public static class SettingsManager
{
    // Default values
    // General settings
    private const int defaultSoundVolume = 75;
    private const int defaultStartingDifficulty = 5;
    // Developer mode
    private const bool defaultDeveloperModeOn = false;
    private const bool defaultUseQaServersOn = false;
    private const bool defaultRegenerateDungeonOn = false;
    private const bool defaultLoadNextLevelOn = false;
    private const bool defaultKillEnemiesOn = false;
    private const bool defaultRegenerateHealthOn = false;
    private const bool defaultRegenerateManaOn = false;
    private const bool defaultLevelDumpOn = false;
    private const bool defaultShowInfoOn = false;

    // Current values
    // General settings
    private static int soundVolume;
    private static int startingDifficulty;
    // Developer mode
    private static bool developerModeOn;
    private static bool useQaServersOn;
    private static bool regenerateDungeonOn;
    private static bool loadNextLevelOn;
    private static bool killEnemiesOn;
    private static bool regenerateHealthOn;
    private static bool regenerateManaOn;
    private static bool levelDumpOn;
    private static bool showInfoOn;

    // Reads all settings values from PlayerPrefs
    // If not found, assigns default
    public static void InitializeSettings()
    {
        soundVolume = PlayerPrefs.GetInt("settings_soundvolume", defaultSoundVolume);
        startingDifficulty = PlayerPrefs.GetInt("settings_startingdif", defaultStartingDifficulty);
        // Booleans cannot be stored as PlayerPrefs, so ints are used
        // 0 = false, 1 = true
        // These inline IFs translate boolean values to ints and viceversa during initialization
        developerModeOn = (PlayerPrefs.GetInt("settings_developeron", (defaultDeveloperModeOn == true ? 1 : 0)) == 1 ? true : false);
        useQaServersOn = (PlayerPrefs.GetInt("settings_useqaservon", (defaultUseQaServersOn == true ? 1 : 0)) == 1 ? true : false);
        regenerateDungeonOn = (PlayerPrefs.GetInt("settings_redungeonon", (defaultRegenerateDungeonOn == true ? 1 : 0)) == 1 ? true : false);
        loadNextLevelOn = (PlayerPrefs.GetInt("settings_loadnextlon", (defaultLoadNextLevelOn == true ? 1 : 0)) == 1 ? true : false);
        killEnemiesOn = (PlayerPrefs.GetInt("settings_killenemion", (defaultKillEnemiesOn == true ? 1 : 0)) == 1 ? true : false);
        regenerateHealthOn = (PlayerPrefs.GetInt("settings_reghealthon", (defaultRegenerateHealthOn == true ? 1 : 0)) == 1 ? true : false);
        regenerateManaOn = (PlayerPrefs.GetInt("settings_regeemanaon", (defaultRegenerateManaOn == true ? 1 : 0)) == 1 ? true : false);
        levelDumpOn = (PlayerPrefs.GetInt("settings_leveldumpon", (defaultLevelDumpOn == true ? 1 : 0)) == 1 ? true : false);
        showInfoOn = (PlayerPrefs.GetInt("settings_showinforon", (defaultShowInfoOn == true ? 1 : 0)) == 1 ? true : false);
    }

    // Saves all current settings values to PlayerPrefs
    public static void PersistSettings()
    {
        PlayerPrefs.SetInt("settings_soundvolume", soundVolume);
        PlayerPrefs.SetInt("settings_startingdif", startingDifficulty);
        PlayerPrefs.SetInt("settings_developeron", (developerModeOn == true ? 1 : 0));
        PlayerPrefs.SetInt("settings_useqaservon", (useQaServersOn == true ? 1 : 0));
        PlayerPrefs.SetInt("settings_redungeonon", (regenerateDungeonOn == true ? 1 : 0));
        PlayerPrefs.SetInt("settings_loadnextlon", (loadNextLevelOn == true ? 1 : 0));
        PlayerPrefs.SetInt("settings_killenemion", (killEnemiesOn == true ? 1 : 0));
        PlayerPrefs.SetInt("settings_reghealthon", (regenerateHealthOn == true ? 1 : 0));
        PlayerPrefs.SetInt("settings_regeemanaon", (regenerateManaOn == true ? 1 : 0));
        PlayerPrefs.SetInt("settings_leveldumpon", (levelDumpOn == true ? 1 : 0));
        PlayerPrefs.SetInt("settings_showinforon", (showInfoOn == true ? 1 : 0));

        PlayerPrefs.Save();
    }

    // Getters
    // Returns the current value for setting "Sound Volume"
    public static int GetSoundVolume() { return soundVolume; }
    // Returns the current value for setting "Starting Difficulty"
    public static int GetStartingDifficulty() { return startingDifficulty; }
    // Returns the current value for setting "Developer Mode ON"
    public static bool GetDeveloperModeOn() { return developerModeOn; }
    // Returns the current value for setting "Use QA Servers ON"
    public static bool GetUseQaServersOn() { return (developerModeOn && useQaServersOn); }
    // Returns the current value for setting "Regenerate Dungon ON"
    public static bool GetRegenerateDungeonOn() { return (developerModeOn && regenerateDungeonOn); }
    // Returns the current value for setting "Load Next Level ON"
    public static bool GetLoadNextLevelOn() { return (developerModeOn && loadNextLevelOn); }
    // Returns the current value for setting "Kill Enemies ON"
    public static bool GetKillEnemiesOn() { return (developerModeOn && killEnemiesOn); }
    // Returns the current value for setting "Regenerate Health ON"
    public static bool GetRegenerateHealthOn() { return (developerModeOn && regenerateHealthOn); }
    // Returns the current value for setting "Regenerate Mana ON"
    public static bool GetRegenerateManaOn() { return (developerModeOn && regenerateManaOn); }
    // Returns the current value for setting "Level Dump ON"
    public static bool GetLevelDumpOn() { return (developerModeOn && levelDumpOn); }
    // Returns the current value for setting "Show Info ON"
    public static bool GetShowInfoOn() { return (developerModeOn && showInfoOn); }

    // Setters
    // Sets the current value for setting "Sound Volume"
    public static void SetSoundVolume(int newSoundVolume) { soundVolume = newSoundVolume; }
    // Sets the current value for setting "Starting Difficulty" and saves settings
    public static void SetStartingDifficulty(int newStartingDifficulty) { startingDifficulty = newStartingDifficulty; }
    // Sets the current value for setting "Developer Mode ON"
    public static void SetDeveloperModeOn(bool newDeveloperModeOn) { developerModeOn = newDeveloperModeOn; }
    // Sets the current value for setting "Use QA Servers ON"
    public static void SetUseQaServersOn(bool newUseQaServersOn) { useQaServersOn = newUseQaServersOn; }
    // Sets the current value for setting "Regenerate Dungon ON"
    public static void SetRegenerateDungeonOn(bool newRegenerateDungeonOn) { regenerateDungeonOn = newRegenerateDungeonOn; }
    // Sets the current value for setting "Load Next Level ON"
    public static void SetLoadNextLevelOn(bool newLoadNextLevelOn) { loadNextLevelOn = newLoadNextLevelOn; }
    // Sets the current value for setting "Kill Enemies ON"
    public static void SetKillEnemiesOn(bool newKillEnemiesOn) { killEnemiesOn = newKillEnemiesOn; }
    // Sets the current value for setting "Regenerate Health ON"
    public static void SetRegenerateHealthOn(bool newRegenerateHealthOn) { regenerateHealthOn = newRegenerateHealthOn; }
    // Sets the current value for setting "Regenerate Mana ON"
    public static void SetRegenerateManaOn(bool newRegenerateManaOn) { regenerateManaOn = newRegenerateManaOn; }
    // Sets the current value for setting "Level Dump ON"
    public static void SetLevelDumpOn(bool newLevelDumpOn) { levelDumpOn = newLevelDumpOn; }
    // Sets the current value for setting "Show Info ON"
    public static void SetShowInfoOn(bool newShowInfoOn) { showInfoOn = newShowInfoOn; }

}
