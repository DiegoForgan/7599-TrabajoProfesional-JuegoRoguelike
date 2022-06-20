using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityData : ScriptableObject
{
    public int health;
    public float movementSpeed;
    //The bigger the number, te faster it can attack
    //Use numbers smaller than 1 to make time beetween attacks slower!
    public float attackRate;
    //This is replaced by enemy prefab on EnemyData Scriptable Object
    //public Sprite sprite;
}
