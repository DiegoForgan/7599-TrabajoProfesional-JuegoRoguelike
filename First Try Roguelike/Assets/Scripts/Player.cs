using TMPro;
using UnityEngine;

public class Player : Entity
{
    public int maxMana = 100;
    public int mana;
    private int keys = 0;
    public ManaBar manaBar;
    public TextMeshProUGUI keysUI;

    private void Start() {
        initializeHealth();
        initializeMana();
    } 
    public void initializeMana(){
        mana = maxMana;
        manaBar.Initialize(mana);
    }

    public void AddHealth(int amount){
        health += amount;
        if(health >= maxHealth) health = maxHealth;
        //Update the health bar HUD value aswell
        healthBar.SetHealth(health);
    }

    public void AddMana(int amountToGrant)
    {
        mana += amountToGrant;
        if (mana >= maxMana) mana = maxMana;
        //Update the health bar HUD value aswell
        manaBar.SetBarMana(mana);
    }

    public int GetMana() { return mana;}

    public void SpendMana(int amountSpent){
        mana -= amountSpent;
        if(mana < 0) mana = 0;
        manaBar.SetBarMana(mana);
    }

    //Key Pickup Logic
    public void ObtainKey()
    {   
        keys = keys+1;
        keysUI.SetText("Keys: " + keys);
    }

    public void SpendKey(){
        keys = keys-1;
        keysUI.SetText("Keys: " + keys);
    }
}
