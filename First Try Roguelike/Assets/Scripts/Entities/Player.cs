using System;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private PlayerData playerData; 
    private int maxMana;
    private int mana;
    private int keys;
    private int gold;
    [SerializeField] private HUD _hud;
    private PlayerMovementController _playerMovement;
    private WeaponManagement _weaponManagement;
    private bool canOpenDoor;
    public static Player instance;

    // CONSTANTS
    private const int INITIAL_KEYS_AMOUNT = 0;

    private void Awake()
    {
        // Gets the player references to UI and other script components
        GetPlayerReferences();
        // Singleton implementation
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void GetPlayerReferences()
    {
        _playerMovement = GetComponent<PlayerMovementController>();
        _weaponManagement = GetComponent<WeaponManagement>();
        animator = GetComponent<CharactersAnimator>();
        animator.SetShowWeapon(true);
    }

    internal void DisableKeyAction()
    {
        canOpenDoor = false;
    }

    public void InitializeStats(){
        canOpenDoor = false;
        slowedDown = false;
        health = playerData.health;
        maxHealth = health;
        mana = playerData.mana;
        maxMana = mana;
        keys = INITIAL_KEYS_AMOUNT;
        gold = GameProgressManager.GetGoldCollected();
        //Initialize the HUD
        _hud.InitHUD(health,mana,gold,keys);
    }

    public void InitializeMovementStats(){
        //Passing movement data to the movement component
        _playerMovement.SetMovementSpeed(playerData.movementSpeed);
    }

    public void InitializeSpellsAndAttacksStats(){
        //Passing spells and attack data to the weapon management component
        _weaponManagement.SetSpellAndAttackStats(playerData);
    }
    private void Start() {
        InitializeStats();
        InitializeMovementStats();
        InitializeSpellsAndAttacksStats();
    }

    internal void EnableKeyAction()
    {
        if(keys >= 1) canOpenDoor = true;
    }

    private void Update() {

        // Level completion condition
        if(canOpenDoor && Input.GetKeyDown(KeyCode.E))
        {
            OpenDungeonDoor();
        }

        // Developer mode settings
        // REMEMBER TO DESTROY THESE OBJECTS BECAUSE THEY KEEP LISTENING FOR THE INPUTS IN THE GAME MENU!!!!
        // Skip level 'N'
        if (Input.GetKeyDown(KeyCode.N)) {
            if (SettingsManager.GetLoadNextLevelOn()) {

                OpenDungeonDoor();
            }
        }

        // Suicide mode 'K'
        if(Input.GetKeyDown(KeyCode.K)) { 
            if (SettingsManager.GetKillEnemiesOn()) {
                Die();
            }
        }

        // Regenerate Health 'H'
        if(Input.GetKeyDown(KeyCode.H)) {
            if (SettingsManager.GetRegenerateHealthOn()) {
                AddHealth(maxHealth);
            }
        }

        // Regenerate Mana 'M'
        if(Input.GetKeyDown(KeyCode.M)) {
            if (SettingsManager.GetRegenerateHealthOn()) {
                AddMana(maxMana);
            }
        }

        // Dump level to text file 'X'
        if(Input.GetKeyDown(KeyCode.X)) {
            if (SettingsManager.GetLevelDumpOn()) {
                GameManager.Instance.DumpLevelToFile();
            }
        }
    }

    private void OpenDungeonDoor()
    {
        // Use the key to open the dungeon door
        SpendKey();
        DisableKeyAction();

        // Update game progress record
        if (GameManager.Instance.IsFinalLevel()) {
            GameProgressManager.SetNexLevel(1);
            GameProgressManager.SetFinishedGame();
        }
        else {
            GameProgressManager.SetNexLevel(GameProgressManager.GetNextLevel()+1);
        }
        GameProgressManager.SetGoldCollected(gold);
        // Stop the stopwatch
        GameManager.Instance.StopStopWatch();
        TimeSpan ts = GameManager.Instance.GetTimeElapsed();
        // Log elapsed time
        GameProgressManager.AddTimeElapsed(ts);

        // Load next level
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        Debug.Log("Door Opened!\nNew level loaded");
        // Maybe play a level completion sound before loading next scene
        GameManager.Instance.LoadNextLevel();

        // Start the stopwatch
        GameManager.Instance.StartStopWatch();
    }

    public override void TakeDamage(int damage){
        base.TakeDamage(damage);
        _hud.UpdateHealth(health);
    }

    //Called by the animation event
    protected override void DestroyEntity(){
        GameManager.Instance.ShowGameOver();
        FindObjectOfType<AudioManager>().PlaySound("GameOverTheme");
        Destroy(gameObject);
    } 
    
    public void AddHealth(int amount){
        health += amount;
        if(health >= maxHealth) health = maxHealth;
        //Update the health bar HUD value aswell
        _hud.UpdateHealth(health);
    }

    public void AddMana(int amountToGrant)
    {
        mana += amountToGrant;
        if (mana >= maxMana) mana = maxMana;
        //Update the health bar HUD value aswell
        _hud.UpdateMana(mana);
    }

    public int GetMana() { return mana;}

    public void SpendMana(int amountSpent){
        mana -= amountSpent;
        if(mana < 0) mana = 0;
        _hud.UpdateMana(mana);
    }

    //Key Pickup Logic
    public void ObtainKey()
    {   
        keys = keys+1;
        _hud.UpdateKeys(keys);
    }

    public void SpendKey(){
        if (keys == 0) return;
        keys = keys-1;
        _hud.UpdateKeys(keys);
    }

    //Gold currency logic when gold item was picked up by the player
    public void AddGold(int goldGranted)
    {
        gold += goldGranted;
        _hud.UpdateGold(gold);
    }

    public int GetMaxMana() { return maxMana; }
}
