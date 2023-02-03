using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaItem : Item
{
    [SerializeField]
    private int amountToGrant = 5;
    protected override void MakeEffect(Player player)
    {
        player.AddMana(amountToGrant);
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound("ManaPickup");
    }
}
