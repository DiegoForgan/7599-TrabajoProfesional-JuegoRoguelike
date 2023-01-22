using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarEnemies : HealthBar
{
    [SerializeField] private float plusOffset;
    [SerializeField] private float minusOffset;

    public void setTopPosition(Vector2 position)
    {
        GameObject canvas = transform.parent.gameObject;
        canvas.transform.position = new Vector2(position.x, position.y + plusOffset);
    }

    public void setBottomPosition(Vector2 position)
    {
        GameObject canvas = transform.parent.gameObject;
        canvas.transform.position = new Vector2(position.x, position.y + minusOffset);
    }

}
