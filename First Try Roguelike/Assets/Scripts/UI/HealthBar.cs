using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class HealthBar : MonoBehaviour
{
    protected Slider _slider;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Image _fill;
    

    private void Awake() {
        _slider = GetComponent<Slider>();
    }

    public void initializeHealthStatus(int maxHealth){
        SetMaxHealth(maxHealth);
        SetHealth(maxHealth);
    }

    public virtual void SetMaxHealth(int maxHealth){
        _slider.maxValue = maxHealth;
        _fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }

    public virtual void SetHealth(int health){
        _slider.value = health;
        _fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }

    internal void SetPosition(Vector2 position)
    {
        //This moves the canvas object that contains the enemy Health Bar
        GameObject canvas = transform.parent.gameObject;
        canvas.transform.position = position;
    }
}
