using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
   //When not inside the range of the door, player cannot open it 
   private void OnTriggerExit2D(Collider2D other) {
       if (other.CompareTag("Player")){
           Player player = other.GetComponent<Player>();
           player.DisableKeyAction();
       }
   }

   //This method enables the usage of the key (if the player has already aquired it) 
   private void OnTriggerEnter2D(Collider2D other) {
       if (other.CompareTag("Player")){
           Player player = other.GetComponent<Player>();
           player.EnableKeyAction();
       }
   }
}
