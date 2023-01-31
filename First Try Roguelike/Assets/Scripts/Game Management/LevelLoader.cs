using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader levelLoader;
    public static LevelLoader Instance { get{ return levelLoader; } }
    
    [SerializeField] private Animator transition;
    [SerializeField] private float waitTime = 1f;

    private const int DEFAULT_STARTING_LEVEL = -1;
    private const int FIRST_CINEMATIC_INDEX = 0;
    private const int SECOND_CINEMATIC_INDEX = 6;
    private const int FINAL_CINEMATIC_INDEX = 12;
    private const int START_SCREEN_SCENE_INDEX = 0;
    private const int MAIN_MENU_SCENE_INDEX = 1;

    private int lastSceneIndex;
    private int currentLevel;
    private bool isCinematic = false;
    private bool gamestarted = false;

    private void Awake() {
        
        if (levelLoader == null) levelLoader = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
    }
    public int GetCurrentSceneIndex(){
        return SceneManager.GetActiveScene().buildIndex;
    }
    public string GetCurrentSceneName(){
        return SceneManager.GetActiveScene().name;
    }
    public bool IsLastScene()
    {
        return GetCurrentSceneIndex() == lastSceneIndex;
    }
    public void LoadNextScene(){
        // Index
        int index = IsLastScene() ? MAIN_MENU_SCENE_INDEX : GetCurrentSceneIndex() + 1;
        LoadSceneByIndex(index);
    }
    public void LoadNextSceneByName(string name)
    {
        StartCoroutine(LoadSceneByName(name));
    }
    public void LoadSceneByIndex(int index)
    {
        StartCoroutine(LoadSceneByIndexCoroutine(index));
    }
    IEnumerator LoadSceneByName(string sceneName){
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(waitTime);
        transition.SetTrigger("End");
        SceneManager.LoadScene(sceneName);
    }
    IEnumerator LoadSceneByIndexCoroutine(int levelIndex){
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(waitTime);
        transition.SetTrigger("End");
        SceneManager.LoadScene(levelIndex);
    }

    internal void LoadNextLevel()
    {
        currentLevel++;
        Debug.Log("current level: " + currentLevel);
        isCinematic = (
            currentLevel == FIRST_CINEMATIC_INDEX 
            || currentLevel == SECOND_CINEMATIC_INDEX 
            || currentLevel == FINAL_CINEMATIC_INDEX
            );
        if (isCinematic) GameManager.Instance.SetCinematicScene(currentLevel);
        else GameManager.Instance.CreateNewLevel(currentLevel);
    }

    internal void StartGameLoop()
    {
        gamestarted = true;
        currentLevel = DEFAULT_STARTING_LEVEL;
        LoadNextLevel();
    }

    private void Update()
    {
        if (!gamestarted || !isCinematic) return;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentLevel == FINAL_CINEMATIC_INDEX)
            {
                gamestarted = false;
                this.LoadNextScene();
            }
            else this.LoadNextLevel();
        }
    }
}
