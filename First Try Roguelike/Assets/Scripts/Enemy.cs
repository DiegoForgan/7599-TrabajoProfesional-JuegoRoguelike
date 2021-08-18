using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private SpriteRenderer _enemySpriteRenderer;
    public int maxHealth = 5; 
    public int health;

    public HealthBar healthBar;
    
    //Called before the Start function
    private void Awake() {
        _enemySpriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Start is called before the first frame update
    void Start()
    {
        _enemySpriteRenderer.color = Color.green;
        health = maxHealth;
        healthBar.initialize(health);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetHealth(health);
        
        StartCoroutine(flashColor());
    
        if(health <= 0) Destroy(gameObject);

    }

    IEnumerator flashColor(){
        _enemySpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _enemySpriteRenderer.color = Color.green;
    }
}
