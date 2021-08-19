using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour, Collidable
{   
    public int damage = 1;

    //Triggered when collides with another object
    private void OnCollisionEnter2D(Collision2D other) {
        
        //This should run the "Take Damage" function in the Collidable interface
        GameObject enemy = other.gameObject;
        enemy.GetComponent<Collidable>().TakeDamage(damage);
        
        
        Destroy(gameObject);
    }

    //Triggered when the game object leaves the visible screen space
    private void OnBecameInvisible() {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage){}
}
