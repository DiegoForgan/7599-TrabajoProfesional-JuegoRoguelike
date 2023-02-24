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

    public int GetMaxHealth() { return maxHealth; }
    
    //Implementing "Collidable" interface method
    public virtual void TakeDamage(int damage){
        health -= damage;
        //checks if entity is dead
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        // Dark magic, don´t ask => probably i will forget how i did this
        else animator.setHurtAnimation();
    }
    
    protected virtual void Die()
    {
        animator.setDeadAnimation();
    }

    public void ExecuteAfterDeathAnimation()
    {
        DestroyEntity();
    }

    // This will be called by the animation event
    protected virtual void DestroyEntity()
    {
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
        FinalBossMovement _finalBossMovement = GetComponent<FinalBossMovement>();
        float normalMovementSpeed;
        if (!_entityMovement) { 
            normalMovementSpeed = _finalBossMovement.GetMovementSpeed();
            _finalBossMovement.SetMovementSpeed(normalMovementSpeed * slowDown);
        }
        else
        {
            normalMovementSpeed = _entityMovement.GetMovementSpeed();
            //The movement speed is reduced by the slow down parameter
            _entityMovement.SetMovementSpeed(normalMovementSpeed * slowDown);
        }
        yield return new WaitForSeconds(duration);
        //After the duration of the spell effect, the movement speed goes back to normal
        if (!_entityMovement) _finalBossMovement.SetMovementSpeed(normalMovementSpeed);
        else _entityMovement.SetMovementSpeed(normalMovementSpeed);
        
        slowedDown = false;
    }
}
