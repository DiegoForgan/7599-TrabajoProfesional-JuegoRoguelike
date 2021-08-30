using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    private SpriteRenderer _enemySpriteRenderer;
    private ShootProyectile _shootProyectile;
    
    //Called before the Start function
    private void Awake() {
        _enemySpriteRenderer = GetComponent<SpriteRenderer>();
        _shootProyectile = GetComponent<ShootProyectile>();
    }


    // Start is called before the first frame update
    void Start()
    {
        _enemySpriteRenderer.color = Color.green;
        initializeHealth();
    }

    public override void TakeDamage(int damage)
    {
        StartCoroutine(flashColor());
        base.TakeDamage(damage);
        _shootProyectile.Shoot();
    }

    IEnumerator flashColor(){
        _enemySpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _enemySpriteRenderer.color = Color.green;
    }
}
