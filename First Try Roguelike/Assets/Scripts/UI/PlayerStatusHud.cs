using System;
using UnityEngine;

public class PlayerStatusHUD : MonoBehaviour
{
    [SerializeField] private HealthBarWithAmount healthBarWithAmount;
    [SerializeField] private ManaBar manaBar;
 
    internal void initializePlayerStatus(int maxHealth, int maxMana)
    {
        /*healthBarWithAmount.initializeHealthStatus(maxHealth);
        manaBar.initializeManaStatus(maxMana);*/
    }

    internal void updateHealth(int health)
    {
        //healthBarWithAmount.SetHealth(health);
    }

    internal void updateMaxHealth(int newMaxHealth)
    {
       // healthBarWithAmount.SetMaxHealth(newMaxHealth);
    }

    internal void updateMana(int mana)
    {
       // manaBar.SetBarMana(mana);
    }

    internal void updateMaxMana(int newMaxMana)
    {
        manaBar.SetMaxBarMana(newMaxMana);
    }
}