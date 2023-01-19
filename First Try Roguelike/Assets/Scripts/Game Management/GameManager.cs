using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{   
    public static GameManager gameManager;
    [SerializeField] private GameObject gameOverUI;
    private DungeonGeneratorManager dungeonGenerator;
    private Dungeon currentDungeon;
    private ItemSpawner itemSpawner;
    private EnemySpawner enemySpawner;
    private GameObject player;
    [SerializeField] HUD _hud;
    [SerializeField] int difficultyLevel;
    private const int INITIAL_DIFFICULTY_LEVEL = 5;
    public static GameManager Instance{ get{ return gameManager; } }
    private void Awake()
    {
        GetGameManagerReferences();

        if (gameManager == null) gameManager = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void GetGameManagerReferences()
    {
        dungeonGenerator = GetComponentInChildren<DungeonGeneratorManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        itemSpawner = GetComponent<ItemSpawner>();
        enemySpawner = GetComponent<EnemySpawner>();
        difficultyLevel = INITIAL_DIFFICULTY_LEVEL;
    }

    public void CreateNewDungeon()
    {
        // Places the tiles on the tilemap to create the dungeon layout
        GenerateDungeonByName("Random");
        // Sets the position of the main player inside the dungeon
        PlacePlayerOnDungeon();
        // Instantiates the items that will be available on the dungeon created
        PlaceItemsOnDungeon();
        // This method will place the enemies on the dungeon to challenge the main player on its quest
        PlaceEnemiesOnDungeon();
    }

    private void PlaceItemsOnDungeon()
    {
        Debug.Log("Placing Items on the current dungeon!");
        itemSpawner.Spawn(currentDungeon);
    }

    private void PlaceEnemiesOnDungeon()
    {
        Debug.Log("Placing enemies on dungeon!");
        enemySpawner.Spawn(difficultyLevel, currentDungeon);
    }

    // Searches on the new created floor Tilemap, a location where the player can be spawned
    private void PlacePlayerOnDungeon()
    {
        Vector2Int playerPosition = currentDungeon.GetRandomFloorPosition();
        player.transform.position = new Vector3(playerPosition.x,playerPosition.y,player.transform.position.z);
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
        LevelLoader.Instance.LoadNextLevel();
    }

    public void SetNewTileMap(Dungeon activeDungeon, Tilemap floor, Tilemap walls)
    {
        currentDungeon = activeDungeon;
        TilemapVisualizer tilemapVisualizer = dungeonGenerator.GetComponent<TilemapVisualizer>();
        tilemapVisualizer.SetTilemaps(floor,walls);
    }
}
