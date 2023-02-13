using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{   
    public static GameManager gameManager;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject player;
    [SerializeField] private Player playerComponent;
    [SerializeField] private HUD _hud;
    [SerializeField] private DungeonGeneratorManager dungeonGenerator;
    [SerializeField] private GameObject[] cinematics;
    [SerializeField] private GameObject hud;
    
    private System.Diagnostics.Stopwatch stopWatch;
    private ItemSpawner itemSpawner;
    private EnemySpawner enemySpawner;
    private DoorSpawner doorSpawner;
    private int difficultyLevel;
    private int startingLevel;
    private int level;

    //CONSTANTS
    private const string proyectileTag = "proyectile";
    private const int FINAL_LEVEL = 10;
    
    //ALGORITHM NAMES CONSTANTS
    private const string RANDOM_WALK_ALGORITHM = "randomwalk";
    private const string CORRIDOR_FIRST_ALGORITHM = "corridorfirst";
    private const string ROOM_FIRST_ALGORITHM = "roomfirst";
    private const string RANDOM_ALGORITHM = "Random";

    
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

        // Initialize the stopwatch for counting elapsed time
        stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Reset();
    }

    private void Start()
    {
        level = startingLevel;
        if (level == 0) LevelLoader.Instance.StartGameLoop();
        else LevelLoader.Instance.ResumeGameplay(level);
        _hud.UpdateDifficulty(difficultyLevel);
    }

    // Returns the current elapsed time in game
    public TimeSpan GetTimeElapsed() {
        return stopWatch.Elapsed;
    }

    // Starts counting time
    public void StartStopWatch() {
        stopWatch.Start();
    }

    // Stops counting time
    public void StopStopWatch() {
        stopWatch.Stop();
    }

    // Resets timer to 0
    public void ResetStopWatch() {
        stopWatch.Reset();
    }

    private void GetGameManagerReferences()
    {
        itemSpawner = GetComponent<ItemSpawner>();
        enemySpawner = GetComponent<EnemySpawner>();
        doorSpawner = GetComponent<DoorSpawner>();
        // We need to get the starting level and difficulty from
        // the game progress manager
        startingLevel = GameProgressManager.GetNextLevel() - 1;
        difficultyLevel = GameProgressManager.GetDifficultyLevel();
    }

    public void CreateNewDungeon()
    {
        //erases all gameObjects previously Spawned
        destroyAllSpawns();
        // Places the tiles on the tilemap to create the dungeon layout
        GenerateDungeonByName(RANDOM_ALGORITHM);
        // Sets the position of the main player inside the dungeon
        PlacePlayerOnDungeon();
        // Instantiates the items that will be available on the dungeon created
        PlaceItemsOnDungeon();
        // This method will place the enemies on the dungeon to challenge the main player on its quest
        PlaceEnemiesOnDungeon();
        // This method will place the door on the dungeon to escape from it once you got the key
        PlaceDungeonDoor();
    }

    private void CreateFinalBossDungeon()
    {
        //erases all gameObjects previously Spawned
        destroyAllSpawns();
        // Places the tiles on the tilemap to create the dungeon layout
        GenerateDungeonByName(RANDOM_WALK_ALGORITHM);
        // Sets the position of the main player inside the dungeon
        PlacePlayerOnDungeon();
        // Instantiates the items that will be available on the dungeon created
        PlaceBossLevelItemsOnDungeon();
        // This method will place the final boss on the dungeon to challenge the main player on its quest
        PlaceFinalBossOnDungeon();
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

    private void PlaceBossLevelItemsOnDungeon()
    {
        Debug.Log("Placing boss level items");
        itemSpawner.SpawnOnlyHealthAndMana(difficultyLevel, dungeonGenerator.GetDungeon());
    }

    private void PlaceEnemiesOnDungeon()
    {
        Debug.Log("Placing enemies on dungeon!");
        enemySpawner.Spawn(difficultyLevel, dungeonGenerator.GetDungeon());
    }

    private void PlaceFinalBossOnDungeon()
    {
        Debug.Log("Placing Final Boss on dungeon!");
        enemySpawner.SpawnFinalBoss(difficultyLevel, dungeonGenerator.GetDungeon());
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

    // Exports the level map to a file, in the
    // same directory as the main game files
    public void DumpLevelToFile() {
        dungeonGenerator.DumpLevelToFile();
    }

    //Creates a new dungeon based on the name given by the string. If no compatible name, it uses a random one.
    public void GenerateDungeonByName(string algorithm){
        switch(algorithm.ToLower()){
            case(RANDOM_WALK_ALGORITHM):
                dungeonGenerator.GenerateRandomWalkDungeon();
                break;
            case(CORRIDOR_FIRST_ALGORITHM):
                dungeonGenerator.GenerateCorridorFirstDungeon();
                break;
            case(ROOM_FIRST_ALGORITHM):
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
        destroyProyectilesCasted();
    }

    private void destroyProyectilesCasted()
    {
        GameObject[] proyectilesActiveOnScene = GameObject.FindGameObjectsWithTag(proyectileTag);
        if (proyectilesActiveOnScene.Length == 0) return;
        foreach (var proyectile in proyectilesActiveOnScene)
        {
            Destroy(proyectile);
        }
    }

    internal void CreateNewLevel()
    {
        level++;
        disableAllCinematics();
        player.SetActive(true);
        hud.SetActive(true);
        _hud.UpdateLevelName("Level - " + level);

        if (IsFinalLevel()) CreateFinalBossDungeon();
        else CreateNewDungeon();

        rechargePlayerManaAndHealth();

        // Start the stopwatch
        GameManager.Instance.ResetStopWatch();
        GameManager.Instance.StartStopWatch();
    }

    public bool IsFinalLevel() {
        return (level == FINAL_LEVEL);
    }

    private void rechargePlayerManaAndHealth()
    {
        playerComponent.AddHealth(playerComponent.GetMaxHealth());
        playerComponent.AddMana(playerComponent.GetMaxMana());
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
