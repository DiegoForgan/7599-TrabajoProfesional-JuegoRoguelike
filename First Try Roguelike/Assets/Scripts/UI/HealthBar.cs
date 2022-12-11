using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    protected Slider _slider;
    protected Quaternion startRotation;

    private void Awake() {
        _slider = GetComponent<Slider>();
    }

    private void Start() {
        startRotation = transform.rotation;
    }

    private void Update() {
        //this keeps the healthbar from rotating
        transform.rotation = startRotation;
    }

    public void initializeHealthStatus(int maxHealth){
        SetMaxHealth(maxHealth);
        SetHealth(maxHealth);
    }

    public virtual void SetMaxHealth(int maxHealth){
        _slider.maxValue = maxHealth;
    }

    public virtual void SetHealth(int health){
        _slider.value = health;
    }

    internal void SetPosition(Vector2 position)
    {
        //This moves the canvas object that contains the enemy Health Bar
        GameObject canvas = transform.parent.gameObject;
        canvas.transform.position = position;
    }
}
