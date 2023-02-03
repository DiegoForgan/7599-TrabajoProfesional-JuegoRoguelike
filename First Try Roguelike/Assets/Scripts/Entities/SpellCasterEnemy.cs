using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasterEnemy : Enemy
{
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
        //Creates a copy of the list to prevent changes on the Scriptable Object
        availableSpells = new List<SpellData>(enemyData.availableSpell);
        //Setting data on the movement component
        InitMovementStats(enemyData.attackDistance);
    }

    public override void Attack(){
        Debug.Log("Spell casting from SPELL CASTER CLASS");
        animator.SetSpellCastingWithStaffAnimation();
        CastRandomSpell();
    }
}
