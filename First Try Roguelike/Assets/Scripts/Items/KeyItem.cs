using UnityEngine;

public class KeyItem : Item
{
    protected override void MakeEffect(Player player)
    {
        //Calls the method to add one key to the player
        player.ObtainKey();
        FindObjectOfType<AudioManager>().PlaySound("KeyPickup");
    }
}
