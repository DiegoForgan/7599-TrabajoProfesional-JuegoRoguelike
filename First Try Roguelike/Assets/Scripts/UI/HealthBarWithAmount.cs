using TMPro;
using UnityEngine;

public class HealthBarWithAmount : HealthBar
{
    public TextMeshProUGUI barText;

    //Changes the font color of the health bar based on proportion
    //Can be changed
    private void setFontColor()
    {
        float currentHealth = _slider.value;
        float maxHealth = _slider.maxValue;
        barText.color = Color.black;
        if (currentHealth / maxHealth <= 0.5f) barText.color = Color.white;
    }


    public override void SetHealth(int health){
        base.SetHealth(health);
        barText.SetText(health+" / "+ _slider.maxValue);
        setFontColor();
    }

    public override void SetMaxHealth(int maxHealth)
    {
        base.SetMaxHealth(maxHealth);
        barText.SetText(_slider.value+" / "+maxHealth);
        setFontColor();
    }
}
