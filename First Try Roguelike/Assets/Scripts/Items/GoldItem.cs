using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldItem : Item
{
    [SerializeField]
    private int goldGranted = 100;
    protected override void MakeEffect(Player player)
    {
        //Calls the method to grant the player an amount of gold
        player.AddGold(goldGranted);
        FindObjectOfType<AudioManager>().PlaySound("GoldPickup");
    }
}
