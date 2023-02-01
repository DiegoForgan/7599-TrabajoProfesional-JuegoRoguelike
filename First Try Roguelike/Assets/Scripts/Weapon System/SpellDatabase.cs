using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpellDatabase : MonoBehaviour
{   
    public SpellData[] spellsDB;
    public static SpellDatabase instance;
    private void Awake() {
        
        if (instance == null) instance = this;
        else {
          Destroy(gameObject);
          return;
        }
    }

    //Searches for the desired spell on the database and returns all the data if found
    public SpellData GetSpellByName(string name){
        SpellData spellFound = Array.Find(spellsDB, spell => spell.name == name);
        if (spellFound == null) Debug.LogWarning("Spell with name: "+ name +" was not found in the Database!");
        return spellFound;
    }
}
