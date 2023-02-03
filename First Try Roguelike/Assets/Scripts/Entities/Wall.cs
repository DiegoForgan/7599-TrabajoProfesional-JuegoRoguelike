using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, Collidable
{
    public void TakeDamage(int damage) {
    
        FindObjectOfType<AudioManager>().PlaySound("Explosion");
        Debug.Log("You've hit a wall!");
    } 
}
