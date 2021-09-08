using System;
using UnityEngine;


public enum SpellTypes
{
    Normal,
    Fire, 
    Ice
}

[System.Serializable]
public class Spell
{
    public string spellName;
    public int damage;
    public int manaCost;
    public SpellTypes type;
    public GameObject spellProyectilePrefab;
    public float spellproyectileForce = 10f;
    public Sprite avatar;
    
    public string GetSpellName(){
        return spellName;
    }

    public int GetSpellDamage(){
        return damage;
    }

    public int GetSpellManaCost(){
        return manaCost;
    }

    public Sprite GetSpellAvatar()
    {
        return avatar;
    }

    public GameObject GetSpellPrefab(){
        return spellProyectilePrefab;
    }
}
