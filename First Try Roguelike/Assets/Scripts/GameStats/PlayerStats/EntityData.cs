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
    public Sprite sprite;
}
