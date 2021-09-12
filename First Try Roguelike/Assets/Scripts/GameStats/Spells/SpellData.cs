using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Spell Data", menuName = "Stats/SpellData") ]
public class SpellData : ScriptableObject
{
    public new string name;
    public string description;
    public int damage;
    public int manaCost;
    public GameObject proyectilePrefab;
    public float proyectileForce;
    public Sprite avatar;
}
