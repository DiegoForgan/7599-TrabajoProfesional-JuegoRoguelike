using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour, Collidable
{   
    [SerializeField]
    private int damage = 1;
    private SpellSideEffectData sideEffectData;

    //Triggered when collides with another object
    private void OnCollisionEnter2D(Collision2D other) {
        
        //This should run the "Take Damage" function in the Collidable interface
        GameObject entity = other.gameObject;
        entity.GetComponent<Collidable>().TakeDamage(damage);
        //Side effect from the spell logic
        Affectable affectableEntity = entity.GetComponent<Affectable>();
        //The casted spell must have a side effect associated and the entity that was hit must be
        //capable of being affected by it
        if(sideEffectData != null && affectableEntity != null){
           ApplySpellSideEffect(affectableEntity); 
        }
        //Destroys the proyectile because it hitted a target
        Destroy(gameObject);
    }

    private void ApplySpellSideEffect(Affectable entity)
    {
        switch (sideEffectData.type)
        {
            case (EffectType.Burn):
                entity.Burn(sideEffectData);
                break;
            case (EffectType.Freeze):
                entity.Freeze(sideEffectData);
                break;
            case (EffectType.Poison):
                entity.Poison(sideEffectData);
                break;
        }
    }

    //Triggered when the game object leaves the visible screen space
    private void OnBecameInvisible() {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage){}

    public void setDamage(int newDamage){
        damage = newDamage;
    }

    public void SetSideEffect(SpellSideEffectData sideEffect){
        sideEffectData = sideEffect; 
    }
}
