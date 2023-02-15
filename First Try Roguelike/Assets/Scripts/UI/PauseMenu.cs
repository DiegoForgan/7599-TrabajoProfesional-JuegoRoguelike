using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject PauseMenuUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject FPSCounter;

    // Update is called once per frame
    void Update()
    {
        if (LevelLoader.isCinematic) return;
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (!gameOverUI.activeSelf) {
                if(GameIsPaused) Resume();
                else Pause();
            }
        }
    }

    public void Pause()
    {
        FPSCounter.SetActive(false);
        PauseMenuUI.SetActive(true);
        GameIsPaused = true;
        GameManager.Instance.StopStopWatch();
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        FPSCounter.SetActive(true);
        PauseMenuUI.SetActive(false);
        GameIsPaused = false;
        GameManager.Instance.StartStopWatch();
        Time.timeScale = 1f;
    }

    public void LoadMainMenu(){
        // Restore normal flow of time
        Time.timeScale = 1f;
        GameIsPaused = false;
        PauseMenuUI.SetActive(false);
        //Scene one is our main menu scene
        SceneManager.LoadScene(1);
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound("MainMenuTheme");
    }
}
