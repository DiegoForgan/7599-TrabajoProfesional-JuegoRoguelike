using System;
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
    private int lastSceneIndex;

    private void Awake() {
        
        if (levelLoader == null) {
            // Initialize player prefs in case they not exist
            //if(!PlayerPrefs.HasKey("session_token")) {
            //    PlayerPrefs.SetString("session_token", "");
            //    PlayerPrefs.SetString("username", "");
            //    PlayerPrefs.Save();
            //}
            levelLoader = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        lastSceneIndex = SceneManager.sceneCountInBuildSettings;
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
        int index = IsLastScene() ? 0 : GetCurrentSceneIndex() + 1;
        StartCoroutine(LoadSceneByIndex(index));
    }
    public void LoadNextSceneByName(string name)
    {
        StartCoroutine(LoadSceneByName(name));
    }
    IEnumerator LoadSceneByName(string sceneName){
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(waitTime);
        transition.SetTrigger("End");
        SceneManager.LoadScene(sceneName);
    }
    IEnumerator LoadSceneByIndex(int levelIndex){
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(waitTime);
        transition.SetTrigger("End");
        SceneManager.LoadScene(levelIndex);
    }

    internal void LoadNextLevel()
    {
        Debug.Log("Advancing to new level!");
    }
}
