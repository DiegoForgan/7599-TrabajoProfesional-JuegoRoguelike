using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBarWithAmount : HealthBar
{
    public TextMeshProUGUI barText;

    public override void SetHealth(int health){
        base.SetHealth(health);
        barText.SetText(health+" / "+ _slider.maxValue);
    }

    public override void SetMaxHealth(int maxHealth)
    {
        base.SetMaxHealth(maxHealth);
        barText.SetText(_slider.value+" / "+maxHealth);
    }
}
