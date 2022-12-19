using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponManagement : MonoBehaviour
{
    private CharactersAnimator animator;
    private List<SpellData> spells;
    private MeleeWeapon _mainWeapon;
    private AttackInput inputs = new AttackInput();
    //CoolDown parameters to prevent spamming attacks
    private float attackRate;
    private float nextAttackTime;
    private SpellData currentSpell = null;

    private Player _player;

    private int currentIndex = 0;
    private Transform _attackPoint;

    private HUD _hud;

    
    
    private void Awake() {
        _player = GetComponent<Player>();
        _hud = GameObject.Find("HUD").GetComponent<HUD>();
        _mainWeapon = GetComponent<MeleeWeapon>();
        _attackPoint = transform.Find("AttackPoint");
        animator = GetComponent<CharactersAnimator>();
    }

    private bool HasAnySpells(){
        return (spells.Count > 0);
    }

    public void SetSpellAndAttackStats(PlayerData data){
        //Load spells based on the player stats defined
        //Creates a copy of the list to prevent changes on the Scriptable Object
        spells = new List<SpellData>(data.learnedSpells);
        attackRate = data.attackRate;
        nextAttackTime = 0;
        _mainWeapon.SetWeaponData(data.meleeWeapon);
        currentIndex = 0;
        //
        //check if spells list is empty (Might happen if player is new)
        if(HasAnySpells()){ 
            currentSpell = spells[currentIndex];
            _hud.UpdateSpellUI(currentSpell);
        }
        animator.SetShowWeapon(true);
    }
    
    // Update is called once per frame
    void Update()
    {
        //If game is paused any user input should be ignored
        if(PauseMenu.GameIsPaused) return;
        
        HandleInput();
        
        if(HasAnySpells()){
            // Mouse ScrollWheel logic to change beetween spells
            if (inputs.shouldDisplayNextSpell()) currentIndex = getNextSpellIndex();
            
            if (inputs.shouldDisplayPreviousSpell()) currentIndex = getPreviousSpellIndex();
            
            currentSpell = spells[currentIndex];
        }
        
        _hud.UpdateSpellUI(currentSpell);
        
        if(Time.time >= nextAttackTime){
            //Mouse Logic to Melee 
            if (inputs.isMeleeAttackKeyPressed()) {
                //_mainWeapon.Attack(_attackPoint);
                animator.setAttackAnimation();
                //TODO: Add Sword swinging sound
                Debug.Log("Remember to add sword swinging sound!");
                nextAttackTime = Time.time + 1f / attackRate;
            }
            //Mouse Logic to cast spells
            else if(inputs.isSpellCastKeyPressed())
            {
                if (currentSpell == null) return;

                int currentManaCost = currentSpell.manaCost;
                if (playerCanAffordSpellWithCost(currentManaCost))
                {
                    _player.SpendMana(currentManaCost);
                    CastSpell();
                    animator.setSpellCastingAnimation();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
                //This will play the "not enough mana sound"
                else FindObjectOfType<AudioManager>().PlaySound("NoMana");
            }
        }
    }

    private bool playerCanAffordSpellWithCost(int currentManaCost)
    {
        return _player.GetMana() >= currentManaCost;
    }

    private int getPreviousSpellIndex()
    {
        return (currentIndex <= 0) ? getSpellsLastIndex() : currentIndex - 1;
    }

    private int getNextSpellIndex()
    {
        return (currentIndex >= getSpellsLastIndex()) ? 0 : currentIndex + 1;   
    }

    private void HandleInput()
    {
        inputs.updateInputs();
    }

    //Creates a proyectile based on the current selected spell by the user
    private void CastSpell()
    {
        //Instantiate the proyectile prefab
        GameObject spellProyectile = Instantiate(currentSpell.proyectilePrefab,_attackPoint.position,_attackPoint.rotation);
        //Set attributes to the new created proyectile
        Proyectile newSpellProyectile = spellProyectile.GetComponent<Proyectile>();
        newSpellProyectile.setDamage(currentSpell.damage);
        newSpellProyectile.SetSideEffect(currentSpell.effect);
        //Apply physics to the proyectile to make it move like a bullet
        Rigidbody2D proyectileRigidBody = spellProyectile.GetComponent<Rigidbody2D>();
        proyectileRigidBody.AddForce(_attackPoint.up * currentSpell.proyectileForce, ForceMode2D.Impulse);
        
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound(currentSpell.name);
    }
   
    //This method Adds a spell to the main character ONLY if the character doesn´t already have it
    public void AddSpell(string newSpellName)
    {
        //Search if spell already obtained by the player
        if (spells.FindIndex(element => element.name == newSpellName) != -1) return;
        //Search the spell data in the spell Database to add it
        SpellData newSpell = FindObjectOfType<SpellDatabase>().GetSpellByName(newSpellName);
        //Bug fix for no spell scenarios
        if (newSpell == null)
        {
            Debug.Log("Spell not found and therefore not added!");
            return;
        }

        spells.Add(newSpell);
        //Every time you get a new spell it automatically sets as your new selected spell
        currentIndex = getSpellsLastIndex();
        currentSpell = spells[currentIndex];
        _hud.UpdateSpellUI(newSpell);

    }

    private int getSpellsLastIndex()
    {
        return spells.Count - 1;
    }

    public List<SpellData> GetSpells(){
        return spells;
    }

    public SpellData GetCurrentSpell(){
        return currentSpell;
    }
}
