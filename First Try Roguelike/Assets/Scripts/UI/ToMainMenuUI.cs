using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToMainMenuUI : MonoBehaviour
{
    //Any user interface that brings the user to the main menu, should eliminate all instances of preserved gameObjects
   protected void DestroyAllPreservedInstances()
    {
        // Destroying the GameManager and its dungeon creator child
        Destroy(GameObject.Find("GameManager"));
        // Destroying this instance of the player
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        // Destroying the user interface on screen for the player
        Destroy(GameObject.Find("HUD"));
        // Destroying the Spell Database
        Destroy(GameObject.Find("SpellDatabase"));
        // Destroy Game Over UI
        Destroy(GameObject.Find("GameOverScreen"));
    }
}
