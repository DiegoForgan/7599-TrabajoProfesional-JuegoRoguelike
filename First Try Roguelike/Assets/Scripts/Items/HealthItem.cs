using UnityEngine;

public class HealthItem : Item
{
    [SerializeField]
    private int amountToGrant = 10;
    protected override void MakeEffect(Player player)
    {
        player.AddHealth(amountToGrant);
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound("HealthPickup");
    }
}
