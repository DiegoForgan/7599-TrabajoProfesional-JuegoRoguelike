using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void initialize(int maxHealth){
        SetMaxHealth(maxHealth);
        SetHealth(maxHealth);
    }

    public virtual void SetMaxHealth(int maxHealth){
        slider.maxValue = maxHealth;
    }

    public virtual void SetHealth(int health){
        slider.value = health;
    }
}
