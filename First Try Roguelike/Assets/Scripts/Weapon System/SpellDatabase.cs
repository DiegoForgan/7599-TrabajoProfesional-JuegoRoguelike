using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpellDatabase : MonoBehaviour
{   
    public Spell[] spellsDB;
    public static SpellDatabase instance;
    private void Awake() {
        
        if (instance == null) instance = this;
        else {
          Destroy(gameObject);
          return;
        }
      

        DontDestroyOnLoad(gameObject);
    }

    //Searches for the desired spell on the database and returns all the data if found
    public Spell GetSpellByName(string name){
        Spell spellFound = Array.Find(spellsDB, spell => spell.GetSpellName() == name);
        if (spellFound == null) Debug.LogWarning("Spell with name: "+ name +" was not found in the Database!");
        return spellFound;
    }
}
