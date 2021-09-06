using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour, Collidable
{   
    [SerializeField]
    private int damage = 1;

    //Triggered when collides with another object
    private void OnCollisionEnter2D(Collision2D other) {
        
        //This should run the "Take Damage" function in the Collidable interface
        GameObject entity = other.gameObject;
        entity.GetComponent<Collidable>().TakeDamage(damage);
        
        //Destroys the proyectile because it hitted a target
        Destroy(gameObject);
    }

    //Triggered when the game object leaves the visible screen space
    private void OnBecameInvisible() {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage){}

    public void setDamage(int newDamage){
        damage = newDamage;
    }
}
