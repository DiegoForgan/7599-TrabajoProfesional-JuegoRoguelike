using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeveloperModeHUD : MonoBehaviour
{
    [SerializeField] private GameObject optionRegerateDungeon;
    [SerializeField] private GameObject optionSkipLevel;
    [SerializeField] private GameObject optionSuicide;
    [SerializeField] private GameObject optionDumpLevel;
    [SerializeField] private GameObject optionInfiniteHealth;
    [SerializeField] private GameObject optionInfiniteMana;

    public void initializeDeveloperModeHUD() {

        Transform optionRegerateDungeonEnabled = optionRegerateDungeon.transform.Find("Enabled");
        Transform optionSkipLevelEnabled = optionSkipLevel.transform.Find("Enabled");
        Transform optionSuicideEnabled = optionSuicide.transform.Find("Enabled");
        Transform optionDumpLevelEnabled = optionDumpLevel.transform.Find("Enabled");
        Transform optionInfiniteHealthEnabled = optionInfiniteHealth.transform.Find("Enabled");
        Transform optionInfiniteManaEnabled = optionInfiniteMana.transform.Find("Enabled");

        Transform optionRegerateDungeonDisabled = optionRegerateDungeon.transform.Find("Disabled");
        Transform optionSkipLevelDisabled = optionSkipLevel.transform.Find("Disabled");
        Transform optionSuicideDisabled = optionSuicide.transform.Find("Disabled");
        Transform optionDumpLevelDisabled = optionDumpLevel.transform.Find("Disabled");
        Transform optionInfiniteHealthDisabled = optionInfiniteHealth.transform.Find("Disabled");
        Transform optionInfiniteManaDisabled = optionInfiniteMana.transform.Find("Disabled");

        if (SettingsManager.GetRegenerateDungeonOn()) {
            optionRegerateDungeonEnabled.gameObject.SetActive(true);
            optionRegerateDungeonDisabled.gameObject.SetActive(false);
        }
        if (SettingsManager.GetLoadNextLevelOn()) {
            optionSkipLevelEnabled.gameObject.SetActive(true);
            optionSkipLevelDisabled.gameObject.SetActive(false);
        }
        if (SettingsManager.GetKillEnemiesOn()) {
            optionSuicideEnabled.gameObject.SetActive(true);
            optionSuicideDisabled.gameObject.SetActive(false);
        }
        if (SettingsManager.GetLevelDumpOn()) {
            optionDumpLevelEnabled.gameObject.SetActive(true);
            optionDumpLevelDisabled.gameObject.SetActive(false);
        }
        if (SettingsManager.GetInfiniteHealthOn()) {
            optionInfiniteHealthEnabled.gameObject.SetActive(true);
            optionInfiniteHealthDisabled.gameObject.SetActive(false);
        }
        if (SettingsManager.GetInfiniteHealthOn()) {
            optionInfiniteManaEnabled.gameObject.SetActive(true);
            optionInfiniteManaDisabled.gameObject.SetActive(false);
        }
    }
}
