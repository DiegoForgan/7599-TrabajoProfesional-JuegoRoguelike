using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item
{
    public int amountToGrant = 10;
    protected override void MakeEffect(Collider2D other)
    {
        Entity player = other.GetComponent<Entity>();
        player.AddHealth(amountToGrant);
    }
}
