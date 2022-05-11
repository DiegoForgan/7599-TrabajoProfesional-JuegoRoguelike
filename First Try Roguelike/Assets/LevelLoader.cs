using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader levelLoader;
    public static LevelLoader Instance { get{ return levelLoader; } }
    
    [SerializeField] private Animator transition;
    [SerializeField] private float waitTime = 1f;

    private void Awake() {
        
        if (levelLoader == null) levelLoader = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public int GetCurrentSceneIndex(){
        return SceneManager.GetActiveScene().buildIndex;
    }

    public string GetCurrentSceneName(){
        return SceneManager.GetActiveScene().name;
    }

    public void LoadNextLevel(){
        int index = 0;
        if (SceneManager.GetActiveScene().name == "Story 3 - Ending") index = 0;
        else index = SceneManager.GetActiveScene().buildIndex+1;
        StartCoroutine(LoadLevel(index));
    }

    IEnumerator LoadLevel(int levelIndex){
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(waitTime);
        transition.SetTrigger("End");
        SceneManager.LoadScene(levelIndex);

    }   
}
