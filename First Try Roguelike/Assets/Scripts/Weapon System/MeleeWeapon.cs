using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField]
    private float weaponRange = 0.5f;
    [SerializeField]
    private int damage = 1;
    private LayerMask _entitiesLayer;
    private LayerMask _furnitureLayer;

    private void Awake() {
        _entitiesLayer = LayerMask.GetMask("Entities");
        _furnitureLayer = LayerMask.GetMask("Furniture");
    }

    public void Attack(Transform attackPoint){
        // Checks if entity hit other entities with melee weapon
        Collider2D[] entitiesHit = Physics2D.OverlapCircleAll(attackPoint.position,weaponRange,_entitiesLayer);
        foreach (Collider2D entity in entitiesHit)
        {
            if(!entity.CompareTag("Player")) entity.GetComponent<Entity>().TakeDamage(damage);
        }
        //Checks if entity hit something breakeable on the dungeon
        Collider2D[] furnitureHit = Physics2D.OverlapCircleAll(attackPoint.position,weaponRange,_furnitureLayer);
        foreach (Collider2D furniture in furnitureHit){
            Debug.Log("Implement hitting furniture with melee weapon");
        }
    }

    public void SetDamage(int newDamage){
        damage = newDamage;
    }

    public void SetWeaponReach(float newReach){
        weaponRange = newReach;
    }
}
