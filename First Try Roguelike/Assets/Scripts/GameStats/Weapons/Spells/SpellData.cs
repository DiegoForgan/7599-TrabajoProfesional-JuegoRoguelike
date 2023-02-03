using UnityEngine;

[CreateAssetMenu (fileName = "New Spell Data", menuName = "Stats/New Spell") ]
public class SpellData : WeaponData
{
    public int manaCost;
    public GameObject proyectilePrefab;
    public float proyectileForce;
    public Sprite avatar;
    public SpellSideEffectData effect;
}
