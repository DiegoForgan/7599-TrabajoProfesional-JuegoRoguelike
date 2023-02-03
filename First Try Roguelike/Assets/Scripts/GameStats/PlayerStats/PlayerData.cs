using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Player Data", menuName = "Stats/New Player Stats")]
public class PlayerData : EntityData
{
    public int mana;
    public int gold;
    public int keys;
    public MeleeWeaponData meleeWeapon;
    public List<SpellData> learnedSpells;
}
