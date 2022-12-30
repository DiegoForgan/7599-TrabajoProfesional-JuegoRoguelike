using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class Enemy : Entity
{
    public EnemyData enemyData; 

    protected List<SpellData> availableSpells;
    protected MeleeWeapon meleeWeapon;
    //protected SpriteRenderer _enemySpriteRenderer;
    protected EnemyMovement _enemyMovement;
    protected Transform _attackPoint;
    protected Transform playerTransform;
    protected float attackRate;
    protected float nextAttackTime = 0;
    protected float attackDistance;

    //Called before the Start function
    private void Awake() {
        //_enemySpriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        meleeWeapon = GetComponent<MeleeWeapon>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _attackPoint = transform.Find("AttackPoint");
        animator = GetComponent<CharactersAnimator>();
        
        if (!playerTransform) Debug.LogError("No player Transform aquired!");
        if (!_attackPoint) Debug.LogError("No attack point found!");
        
    }

    private void Start() {
        InitHealth();
        //Assigning a big value cause we count on it being replaced on any of the following "if" statements
        float distance = float.MaxValue;
        if(IsMeleeAttacker()){
            meleeWeapon.SetWeaponData(enemyData.availableMeleeWeapon);
            //distance has to be tweaked in order to respond to the attacking point
            distance = Vector2.Distance(transform.position,_attackPoint.position) + enemyData.availableMeleeWeapon.range;
        }
        if(IsSpellCaster()){
            //Creates a copy of the list to prevent changes on the Scriptable Object
            availableSpells = new List<SpellData>(enemyData.availableSpell);
            distance = enemyData.attackDistance;
        }
        InitMovementStats(distance);
    }

    private void Update()
    {
        if (!IsInAttackDistanceAndAbleToAttack()) return;
        Attack();
        nextAttackTime = Time.time + 1f / attackRate;
    }

    protected void InitMovementStats(float distance){
        //Passing movement data to corresponding component
        _enemyMovement.SetMovementSpeed(enemyData.movementSpeed);
        _enemyMovement.SetPlayerTransform(playerTransform);
        SetAttackingParameters(enemyData.attackRate,distance);
    }

    protected void InitHealth(){
        health = enemyData.health;
        maxHealth = health;
        //healthBar.initializeHealthStatus(enemyData.health);
        slowedDown = false;
    }

    internal void SetAttackingParameters(float attRate, float attDistance)
    {
        attackRate = attRate;
        attackDistance = attDistance;
    }

    /*internal HealthBar GetHealthBar()
    {
        return healthBar;
    }*/
    protected bool IsInAttackDistanceAndAbleToAttack()
    {
        //Checks if player is at the distance required for enemy to attack
        float playerEnemyDistance = Vector2.Distance(transform.position,playerTransform.position);
        return ((playerEnemyDistance <= attackDistance) && (Time.time >= nextAttackTime));
    }

    public override void TakeDamage(int damage)
    {
        //StartCoroutine(flashColor());
        base.TakeDamage(damage);
        //healthBar.SetHealth(health);
    }

    public virtual void Attack(){
        if (IsMeleeAttacker())
        {
            meleeWeapon.AttackPlayer(_attackPoint);
            animator.setAttackAnimation();
        }
        else if (IsSpellCaster())
        {
            CastRandomSpell();
            animator.setSpellCastingAnimation();
        }
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
        //_enemySpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        //_enemySpriteRenderer.color = Color.white;
    }

    //If the enemy has a melee weapon asigned, it´s a "melee attacker"
    public virtual bool IsMeleeAttacker(){
        return (enemyData.availableMeleeWeapon != null);
    }

    //If the enemy has at least one spell, it can cast spells
    public bool IsSpellCaster(){
        return (enemyData.availableSpell.Count != 0);
    }
}
