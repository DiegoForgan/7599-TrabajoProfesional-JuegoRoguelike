using UnityEngine;


public class HUD : MonoBehaviour
{
    [SerializeField] private SpellHUD _spellHUD;
    [SerializeField] private PlayerStatusHUD _playerStatusHUD;
    [SerializeField] private CollectablesStatusHUD _collectablesStatusHUD;
    [SerializeField] private LevelDifficultyHUD _levelDifficultyHUD;
    [SerializeField] private DeveloperModeHUD _developerModeHUD;

    public static HUD instance;

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        
    }
    public void InitHUD(int maxHealth, int maxMana, int currentGold, int currentKeys){
        _playerStatusHUD.initializePlayerStatus(maxHealth,maxMana);
        _collectablesStatusHUD.initializeCollectablesStatus(currentGold, currentKeys);
        _spellHUD.initializeSpellHUD();
        _developerModeHUD.initializeDeveloperModeHUD();
        //_levelDifficultyHUD.restartLevelAndDifficulty();
    }

    public void UpdateHealth(int health){
        _playerStatusHUD.updateHealth(health);    
    }

    public void UpdateMaxHealth(int newMaxHealth){
        _playerStatusHUD.updateMaxHealth(newMaxHealth);
    }

    public void UpdateMana(int mana){
        _playerStatusHUD.updateMana(mana);
    }

    public void UpdateMaxMana(int newMaxMana){
        _playerStatusHUD.updateMaxMana(newMaxMana);    
    }

    public void UpdateGold(int goldValue){
        _collectablesStatusHUD.updateGoldCollected(goldValue);
    }

    public void UpdateKeys(int keysValue){
        // TODO: check to replace int to boolean
        _collectablesStatusHUD.updateKeysCollected(keysValue);
    }

    public void UpdateSpellUI(SpellData currentSpell){
        if (currentSpell == null) _spellHUD.initializeSpellHUD();
        else _spellHUD.updateSpellHUD(currentSpell);
    }

    public void UpdateLevelName(string name)
    {
        _levelDifficultyHUD.setLevelName(name);
    }

    public void UpdateDifficulty(int difficultyLevel)
    {
        _levelDifficultyHUD.setDifficulty(difficultyLevel.ToString());
    }

    public void UpdateLevelNameAndDifficulty(string name, int difficultyLevel)
    {
        this.UpdateLevelName(name);
        this.UpdateDifficulty(difficultyLevel);
    }

    public void UpdateDeveloperModeInfoPanel(string algorithm, 
                                             string tilemapSize,
                                             string enemiesRemaining,
                                             string healthLevel,
                                             string manaLevel,
                                             string timeElapsed)
    {

        _developerModeHUD.UpdateDeveloperModeInfoHUD(algorithm,
                                                     tilemapSize,
                                                     enemiesRemaining,
                                                     healthLevel,
                                                     manaLevel,
                                                     timeElapsed);
    }
}
