using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Enemy Data", menuName = "Stats/New Enemy")]
public class EnemyData : EntityData
{
    //This distance is relevant when the enemy tries to cast spells on the player
    public float attackDistance;
    public MeleeWeaponData availableMeleeWeapon;
    public List<SpellData> availableSpell;
       
}
