using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public EnemyData enemyData;
    public HealthBar healthBar;

    protected List<SpellData> availableSpells;
    protected MeleeWeapon meleeWeapon;
    protected SpriteRenderer _enemySpriteRenderer;
    protected EnemyMovement _enemyMovement;
    protected Transform _attackPoint;
    
    //Called before the Start function
    private void Awake() {
        _enemySpriteRenderer = GetComponent<SpriteRenderer>();
        meleeWeapon = GetComponent<MeleeWeapon>();
        _enemyMovement = GetComponent<EnemyMovement>(); 
        _attackPoint = transform.Find("ShootPoint");    
    }

    private void Start() {
        InitHealth();
        //Assigning a big value cause we count on it being replaced on any of the following "if" statements
        float distance = 100;
        if(IsMeleeAttacker()){
            meleeWeapon.SetWeaponData(enemyData.availableMeleeWeapon);
            //distance has to be tweaked in order to respond to the attacking point
            distance = Vector2.Distance(transform.position,_attackPoint.position);
            distance += enemyData.availableMeleeWeapon.range;
        }
        if(IsSpellCaster()){
            //Creates a copy of the list to prevent changes on the Scriptable Object
            availableSpells = new List<SpellData>(enemyData.availableSpell);
            distance = enemyData.attackDistance;
        }
        InitMovementStats(distance);
    }

    protected void InitMovementStats(float distance){
        //Passing movement data to corresponding component
        _enemyMovement.SetMovementSpeed(enemyData.movementSpeed);
        _enemyMovement.SetAttackingParameters(enemyData.attackRate,distance);
    }

    protected void InitHealth(){
        health = enemyData.health;
        maxHealth = health;
        healthBar.initialize(enemyData.health);
        slowedDown = false;
    }

    internal HealthBar GetHealthBar()
    {
        return healthBar;
    }

    public override void TakeDamage(int damage)
    {
        StartCoroutine(flashColor());
        base.TakeDamage(damage);
        healthBar.SetHealth(health);
    }

    public virtual void Attack(){
        if(IsMeleeAttacker()) 
            meleeWeapon.AttackPlayer(_attackPoint);
        else if(IsSpellCaster()) 
            CastRandomSpell();
    }
    
    public void CastRandomSpell(){
        SpellData selectedSpell = availableSpells[Random.Range(0,availableSpells.Count)];
        GameObject proyectile = Instantiate(selectedSpell.proyectilePrefab,_attackPoint.position,_attackPoint.rotation);
        //Setting the new proyectile parameters
        Proyectile newProyectile = proyectile.GetComponent<Proyectile>();
        newProyectile.setDamage(selectedSpell.damage);
        newProyectile.SetSideEffect(selectedSpell.effect);
        //Adding Physics to the new proyectile created
        Rigidbody2D proyectileRigidBody = proyectile.GetComponent<Rigidbody2D>();
        proyectileRigidBody.AddForce(_attackPoint.up * selectedSpell.proyectileForce, ForceMode2D.Impulse);
        
        FindObjectOfType<AudioManager>().PlaySound(selectedSpell.name);
        
    }

    IEnumerator flashColor(){
        _enemySpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _enemySpriteRenderer.color = Color.white;
    }

    //If the enemy has a melee weapon asigned, it´s a "melee attacker"
    public bool IsMeleeAttacker(){
        return (enemyData.availableMeleeWeapon != null);
    }

    //If the enemy has at least one spell, it can cast spells
    public bool IsSpellCaster(){
        return (enemyData.availableSpell.Count != 0);
    }
}
