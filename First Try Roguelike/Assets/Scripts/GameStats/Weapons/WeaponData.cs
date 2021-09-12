using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponData : ScriptableObject
{
    public new string name;
    public string description;
    public int damage;

    public virtual void Attack(){
        Debug.Log("You´ve attacked with " + name + " which deals " + damage + " damage points");
    }
}
