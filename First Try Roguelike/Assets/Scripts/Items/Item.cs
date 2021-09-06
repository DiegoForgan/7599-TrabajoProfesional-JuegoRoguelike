using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
       //check if the entity that touched the item is the main player
       if(other.CompareTag("Player")){
           PickUp(other);
       }
   }

    private void PickUp(Collider2D other)
    {
        //Instantiate visual effect that the item was picked up
        //TO ADD
        //Make the desired effect of the item
        MakeEffect(other.GetComponent<Player>());
        //Destroy the item from the scene (the second parameter is the time before it´s destroyed)
        Destroy(gameObject);
    }

    //Method that every item should implement in order to make the desired effect on the player
    protected abstract void MakeEffect(Player player);
}
