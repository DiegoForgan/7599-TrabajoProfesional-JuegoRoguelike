using UnityEngine;


public class HUD : MonoBehaviour
{
    [SerializeField] private SpellHUD _spellHUD;
    [SerializeField] private PlayerStatusHUD _playerStatusHUD;
    [SerializeField] private CollectablesStatusHUD _collectablesStatusHUD;

    public static HUD instance;

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }
    public void InitHUD(int maxHealth, int maxMana, int currentGold, int currentKeys){
        _playerStatusHUD.initializePlayerStatus(maxHealth,maxMana);
        _collectablesStatusHUD.initializeCollectablesStatus(currentGold, currentKeys);
        _spellHUD.initializeSpellHUD();
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
        _spellHUD.updateSpellHUD(currentSpell);
    }

    public void NoSpellUI(){
        _spellHUD.initializeSpellHUD();
    }
}
