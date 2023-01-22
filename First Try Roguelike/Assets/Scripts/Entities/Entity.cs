using System;
using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour, Collidable, Affectable
{
    protected int maxHealth;
    protected int health;
    protected CharactersAnimator animator;

    protected bool slowedDown = false;
    
    public int GetHealth(){
        return health;
    }
    
    //Implementing "Collidable" interface method
    public virtual void TakeDamage(int damage){
        health -= damage;
        //checks if entity is dead
        if (health <= 0) DestroyElement();
        animator.setHurtAnimation();
    }
    
    public virtual void DestroyElement()
    {
        //Give time to play animation before destroying game object
        animator.setDeadAnimation();
        Destroy(gameObject);
    }

    public void Burn(SpellSideEffectData sideEffectData)
    {
        //Starts the coroutine that deals damage per second to the entity
        StartCoroutine(DamagePerSecond(sideEffectData.damagePerSecond,sideEffectData.duration));
    }

    IEnumerator DamagePerSecond(int dps, int duration) {
        Debug.Log("Burning");
        //This indicates de amount of seconds beetween de dealt damage
        float SECONDS_TO_WAIT = 1;
        int timeElapsed = 0;
        while(timeElapsed != duration)
        {
            yield return new WaitForSeconds(SECONDS_TO_WAIT);
            TakeDamage(dps);
            timeElapsed++;
        }
    }

    public void Poison(SpellSideEffectData sideEffectData)
    {
        //Poison effect makes entity slower and it also takes damage, so it´s like a combination of
        //Burn effect and Freeze Effect
        Burn(sideEffectData);
        Freeze(sideEffectData);
    }

    public void Freeze(SpellSideEffectData sideEffectData)
    {
        if (slowedDown) return;
        //Couroutine that modifies the movement speed temporarily
        StartCoroutine(SlowDown(sideEffectData.slowDown,sideEffectData.duration));
    }

    IEnumerator SlowDown(float slowDown, int duration)
    {
        Debug.Log("Slowing Down");
        slowedDown = true;
        //The slowdown parameter is the percentage (beetween 0 and 1) of speed that remains on the player
        EntityMovement _entityMovement = GetComponent<EntityMovement>();
        float normalMovementSpeed = _entityMovement.GetMovementSpeed();
        //The movement speed is reduced by the slow down parameter
        _entityMovement.SetMovementSpeed(normalMovementSpeed*slowDown);
        yield return new WaitForSeconds(duration);
        //After the duration of the spell effect, the movement speed goes back to normal
        _entityMovement.SetMovementSpeed(normalMovementSpeed);
        slowedDown = false;
    }
}
