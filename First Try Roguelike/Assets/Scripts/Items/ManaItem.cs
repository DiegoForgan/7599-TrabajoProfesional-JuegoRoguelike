using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaItem : Item
{
    public int amountToGrant = 5;
    protected override void MakeEffect(Collider2D other)
    {
        Entity player = other.GetComponent<Entity>();
        player.AddMana(amountToGrant);
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound("ManaPickup");
    }
}
