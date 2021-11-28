using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    public static GameManager gameManager;
    [SerializeField] private GameObject gameOverUI;
    

    private DungeonGenerator dungeonGenerator;
    private Dungeon currentDungeon;
    private GameObject player;

    public static GameManager Instance{ get{ return gameManager; } }
    private void Awake() {
      Debug.Log(this.name + "executed the AWAKE method!");
      
      if (gameManager == null) gameManager = this;
      else {
          Destroy(gameObject);
          // I think this makes a new dungeon every time a new scene is loaded
          // where the game manager is instantitated
          gameManager.CreateNewDungeon();
          return;
      }
      DontDestroyOnLoad(gameObject);

      dungeonGenerator = GetComponentInChildren<DungeonGenerator>();

      currentDungeon = GameObject.Find("Dungeon").GetComponent<Dungeon>();
      player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start() {
        Debug.Log(this.name + "executed the START method!");
        CreateNewDungeon();
    }

    private void CreateNewDungeon()
    {
        GenerateDungeonByName("Random");
        PlacePlayerOnDungeon();
    }

    // Searches on the new created floor Tilemap, a location where the player can be spawned
    private void PlacePlayerOnDungeon()
    {
        Vector2Int playerPosition = currentDungeon.GetRandomFloorPosition();
        player.transform.position = new Vector3(playerPosition.x,playerPosition.y,player.transform.position.z);
        Debug.Log(playerPosition);
    }

    public void StartNewGame(){
        player.GetComponent<Player>().InitializeStats();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)){
            CreateNewDungeon();
        }
    }

   

    //Creates a new dungeon based on the name given by the string. If no compatible name, it uses a random one.
    public void GenerateDungeonByName(string algorithm){
        switch(algorithm.ToLower()){
            case("randomwalk"):
                dungeonGenerator.GenerateRandomWalkDungeon();
                break;
            case("corridorfirst"):
                dungeonGenerator.GenerateCorridorFirstDungeon();
                break;
            case("roomfirst"):
                dungeonGenerator.GenerateRoomFirstDungeon();
                break;
            default:
                dungeonGenerator.GenerateDungeonUsingRandomAlgorithm();
                break;
        }
    }

    public void ShowGameOver(){
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LoadNextLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
