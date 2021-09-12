using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityData : ScriptableObject
{
    public int health;
    public float movementSpeed;
    public float attackRate;
    public float nextAttackTime;
    public Sprite sprite;
}
