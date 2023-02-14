using UnityEngine;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    public GameObject _GameOverUI;
    [SerializeField] private GameObject gameOverText;

    public void Awake() {
        
        // Sets the appropiate text for the game over window
        if (GameProgressManager.GetDifficultyLevel() <= (GameProgressManager.GetLowestDifficultyLevel()+1)) {
            gameOverText.GetComponent<TMP_Text>().text = "You can do better than that... now you'll have to start over!";
        }
        else {
            gameOverText.GetComponent<TMP_Text>().text = "No worries, you can always try again. To help you hone your skills, the difficulty level has been lowered.";
        }
    }

    public void TryAgain() {
        // Update game progress for player
        // Time elapsed is always added, as a penalty to the player
        GameProgressManager.AddTimeElapsed(GameManager.Instance.GetTimeElapsed());
        // When you lose, the game resets to the first level
        GameProgressManager.SetNexLevel(GameProgressManager.GetDefaultNextLevel());
        // If the difficulty level is 1 or 2, the game progress level RESETS
        if (GameProgressManager.GetDifficultyLevel() <= (GameProgressManager.GetLowestDifficultyLevel()+1)) {
            // Goes back to:
            // Level 1
            // The difficulty level set in settings
            // No elapsed time
            GameProgressManager.ResetGameProgress();
        }
        else {
            // Lowers difficulty level by 1
            GameProgressManager.SetDifficultyLevel(GameProgressManager.GetDifficultyLevel()-1);
        }
        // Restore normal flow of time
        Time.timeScale = 1f;
        _GameOverUI.SetActive(false);
        // Goes back to the main menu to start again
        LevelLoader.Instance.LoadSceneByIndex(1);
        // Sound management
        // This command stops the desired sound clip
        FindObjectOfType<AudioManager>().StopSound("GameOverTheme");
        // This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound("MainMenuTheme");
    }
}
