using System;
using UnityEngine;

public class Player : Entity
{
    public PlayerData playerData; 
    private int maxMana;
    private int mana;
    private int keys;
    private int gold;
    private HUD _hud;

    private void Awake() {
        _hud = GetComponent<HUD>(); 
    }
    private void Start() {
        health = playerData.health;
        maxHealth = health;
        mana = playerData.mana;
        maxMana = mana;
        keys = playerData.keys;
        gold = playerData.gold;
        _hud.InitHUD(playerData.health,playerData.mana,playerData.gold,playerData.keys);
    }

    public override void TakeDamage(int damage){
        base.TakeDamage(damage);
        _hud.UpdateHealth(health);
    }

    public override void DestroyElement(){
        GameOverMenu.IsPlayerDead = true;
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
        keys = keys-1;
        _hud.UpdateKeys(keys);
    }

    //Gold currency logic when gold item was picked up by the player
    public void AddGold(int goldGranted)
    {
        gold += goldGranted;
        _hud.UpdateGold(gold);
    }
}
