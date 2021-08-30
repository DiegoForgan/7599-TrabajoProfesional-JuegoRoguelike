using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, Collidable
{
    public int maxHealth = 100;
    public int maxMana = 100;
    public int health;
    public int mana;
    public HealthBar healthBar;

    

    public ManaBar manaBar;


    private void Start() {
        initializeHealth();
        initializeMana();
    }

    public void initializeHealth(){
        health = maxHealth;
        healthBar.initialize(health);
    }

    public void initializeMana(){
        mana = maxMana;
        manaBar.Initialize(mana);
    }
    
    //Implementing "Collidable" interface method
    public virtual void TakeDamage(int damage){
         health -= damage;
         healthBar.SetHealth(health);

         if(health <= 0) DestroyElement();
     }

    public virtual void DestroyElement()
    {
        Destroy(gameObject);
    }

    public int GetMana()
    {
        return mana;
    }

    public int GetHealth(){
        return health;
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

    public void SpendMana(int amountSpent){
        mana -= amountSpent;
        if(mana < 0) mana = 0;
        manaBar.SetBarMana(mana);
    }
}
