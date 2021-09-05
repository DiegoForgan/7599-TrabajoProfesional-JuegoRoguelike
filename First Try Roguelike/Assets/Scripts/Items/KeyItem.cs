using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Item
{
    protected override void MakeEffect(Collider2D other)
    {
        Entity player = other.GetComponent<Entity>();
        player.ObtainKey();
        FindObjectOfType<AudioManager>().PlaySound("KeyPickup");
    }
}
