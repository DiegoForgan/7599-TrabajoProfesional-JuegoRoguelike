using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour
{   
    public int damage = 1;

    //Triggered when collides with another object
    private void OnCollisionEnter2D(Collision2D other) {
        
        //This should run the "Take Damage" function in the enemy class
        GameObject enemy = other.gameObject;
        enemy.GetComponent<Enemy>().TakeDamage(damage);
        
        
        Destroy(gameObject);
    }

    //Triggered when the game object leaves the visible screen space
    private void OnBecameInvisible() {
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
