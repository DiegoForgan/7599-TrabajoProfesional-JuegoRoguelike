using UnityEngine;

public class StartScreen : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)){
            LevelLoader.Instance.LoadNextScene();
        }
    }
}
