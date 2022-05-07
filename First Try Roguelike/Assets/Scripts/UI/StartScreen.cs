using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)){
            if(SceneManager.GetActiveScene().name == "Story 3 - Ending") SceneManager.LoadScene("Menu");
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
