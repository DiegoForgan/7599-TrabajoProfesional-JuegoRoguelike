using CustomizableCharacters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherEnemy : Enemy
{
    [SerializeField] private CustomizableCharacter _customizableCharacter;
    [SerializeField] private CustomizationCategory _bowCategory;
    
    //Called before the Start function
    private void Awake()
    {
        //_enemySpriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _enemyMovement = GetComponent<EnemyMovement>();
        _attackPoint = transform.Find("AttackPoint");
        animator = GetComponent<CharactersAnimator>();
        if (!playerTransform) Debug.LogError("No player Transform aquired!");
        if (!_attackPoint) Debug.LogError("No attack point found!");
    }

    // Start is called before the first frame update
    void Start()
    {
        //Initialites health stats for the current Enemy and it´s healthbar ALSO slowed Down Status.
        InitHealth();
        availableSpells = new List<SpellData>(enemyData.availableSpell);
        //Setting data on the movement component
        InitMovementStats(enemyData.attackDistance);
        //Shows bow for the archer
        _customizableCharacter.Customizer.ShowCategory(_bowCategory);
    }

    public override void Attack()
    {
        animator.SetArrowThrowingAnimation();
        shootArrow();
    }

    private void shootArrow()
    {
        //Only has arrow "spell"
        SpellData selectedSpell = availableSpells[0];
        GameObject proyectile = Instantiate(selectedSpell.proyectilePrefab, _attackPoint.position, _attackPoint.rotation);
        //Setting the new proyectile parameters
        Proyectile newProyectile = proyectile.GetComponent<Proyectile>();
        newProyectile.setDamage(selectedSpell.damage);
        newProyectile.SetSideEffect(selectedSpell.effect);
        //Adding Physics to the new proyectile created
        Rigidbody2D proyectileRigidBody = proyectile.GetComponent<Rigidbody2D>();
        proyectileRigidBody.AddForce(_attackPoint.up * selectedSpell.proyectileForce, ForceMode2D.Impulse);

        //FindObjectOfType<AudioManager>().PlaySound(selectedSpell.name);
    }
}
