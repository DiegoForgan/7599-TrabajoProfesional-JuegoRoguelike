using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{   
    public static GameManager gameManager;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject player;
    [SerializeField] private HUD _hud;
    [SerializeField] private DungeonGeneratorManager dungeonGenerator;
    [SerializeField] private GameObject[] cinematics;
    [SerializeField] private GameObject hud;
    
    private ItemSpawner itemSpawner;
    private EnemySpawner enemySpawner;
    private DoorSpawner doorSpawner;
    private int difficultyLevel;
    private int level;
    
    public static GameManager Instance{ get{ return gameManager; } }
    private void Awake()
    {
        if (gameManager == null) gameManager = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        GetGameManagerReferences();
    }

    private void Start()
    {   
        LevelLoader.Instance.StartGameLoop();
        level = 0;
        _hud.UpdateDifficulty(difficultyLevel);
    }

    private void GetGameManagerReferences()
    {
        itemSpawner = GetComponent<ItemSpawner>();
        enemySpawner = GetComponent<EnemySpawner>();
        doorSpawner = GetComponent<DoorSpawner>();
        difficultyLevel = SettingsManager.GetStartingDifficulty();
    }

    public void CreateNewDungeon()
    {
        //erases all gameObjects previously Spawned
        destroyAllSpawns();
        // Places the tiles on the tilemap to create the dungeon layout
        GenerateDungeonByName("Random");
        // Sets the position of the main player inside the dungeon
        PlacePlayerOnDungeon();
        // Instantiates the items that will be available on the dungeon created
        PlaceItemsOnDungeon();
        // This method will place the enemies on the dungeon to challenge the main player on its quest
        PlaceEnemiesOnDungeon();
        // This method will place the door on the dungeon to escape from it once you got the key
        PlaceDungeonDoor();
    }

    private void PlaceDungeonDoor()
    {
        doorSpawner.Spawn(difficultyLevel, dungeonGenerator.GetDungeon());
    }

    private void PlaceItemsOnDungeon()
    {
        Debug.Log("Placing Items on the current dungeon!");
        itemSpawner.Spawn(difficultyLevel,dungeonGenerator.GetDungeon());
    }

    private void PlaceEnemiesOnDungeon()
    {
        Debug.Log("Placing enemies on dungeon!");
        enemySpawner.Spawn(difficultyLevel, dungeonGenerator.GetDungeon());
    }

    // Searches on the new created floor Tilemap, a location where the player can be spawned
    private void PlacePlayerOnDungeon()
    {
        Vector2Int playerPosition = dungeonGenerator.GetRandomFloorPositionOnDungeon();
        player.transform.position = new Vector3(playerPosition.x,playerPosition.y,player.transform.position.z);
    }

    // Developer Mode Settings
    // Generate New Dungeon In Place 'SPACE'
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)){
            if (SettingsManager.GetRegenerateDungeonOn()) {
                CreateNewDungeon();
            }
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

    internal void SetCinematicScene(int currentLevel)
    {
        destroyAllSpawns();
        player.SetActive(false);
        hud.SetActive(false);
        if (currentLevel == 0) cinematics[0].SetActive(true);
        else if (currentLevel == 6) cinematics[1].SetActive(true);
        else cinematics[2].SetActive(true);
    }

    private void destroyAllSpawns()
    {
        itemSpawner.destroyAllSpawedObjects();
        enemySpawner.destroyAllSpawedObjects();
        doorSpawner.destroyAllSpawedObjects();
    }

    internal void CreateNewLevel()
    {
        level++;
        disableAllCinematics();
        player.SetActive(true);
        hud.SetActive(true);
        _hud.UpdateLevelName("Level - " + level);
        CreateNewDungeon();
    }

    private void disableAllCinematics()
    {
        if (cinematics == null) return;
        foreach (var cinematic in cinematics)
        {
            cinematic.SetActive(false);
        }
    }
}
