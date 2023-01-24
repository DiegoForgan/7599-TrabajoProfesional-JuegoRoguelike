using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelDifficultyHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private TextMeshProUGUI currentDifficulty;
    private const string DEFAULT_DIFFICULTY = "5";
    private const string DEFAULT_LEVEL = "LEVEL - 1";

    public void restartLevelAndDifficulty() {
        levelName.SetText(DEFAULT_LEVEL);
        currentDifficulty.SetText(DEFAULT_DIFFICULTY);
    }
    public void setLevelName(string newName) {
        this.levelName.SetText(newName);
    }

    public void setDifficulty(string newDifficulty)
    {
        this.currentDifficulty.SetText(newDifficulty);
    }

    public void setLevelNameAndDifficulty(string newName, string newDifficulty)
    {
        this.setLevelName(newName);
        this.setDifficulty(newDifficulty);
    }
}
