using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    None, //No side effect
    Burn, //Makes damage per second
    Poison,//Slows down entity and damages per second
    Freeze //Makes entity stop moving for an amount of seconds
}

[CreateAssetMenu (fileName = "New Spell SideEffect Data", menuName = "Stats/New Spell SideEffect") ]
public class SpellSideEffectData : ScriptableObject
{
   public new string name;
   public string description;
   public int duration; // in seconds
   public int damagePerSecond; //in case the effect requires this
   public float slowDown; //percentage to slow down affected entity
   public EffectType type;
}
