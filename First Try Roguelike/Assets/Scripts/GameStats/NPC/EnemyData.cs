using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Enemy Data", menuName = "Stats/New Enemy")]
public class EnemyData : EntityData
{
    public MeleeWeaponData availableMeleeWeapon;
    public List<SpellData> availableSpell;
       
}
