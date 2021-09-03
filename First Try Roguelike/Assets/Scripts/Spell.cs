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
    

    public Spell (string name, int damage, int manaCost, SpellTypes type, GameObject prefab){
        this.spellName = name;
        this.damage = damage;
        this.manaCost = manaCost;
        this.type = type;
        this.spellProyectilePrefab = prefab;
    }

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
}
