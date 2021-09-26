using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponManagement : MonoBehaviour
{
    //public PlayerData playerData;
    private List<SpellData> spells;
    private MeleeWeapon _mainWeapon;
    //CoolDown parameters to prevent spamming attacks
    private float attackRate;
    private float nextAttackTime;
    private SpellData currentSpell;

    private Player _player;

    private int currentIndex = 0;
    private Transform _attackPoint;

    private HUD _hud;

    
    
    private void Awake() {
        _player = GetComponent<Player>();
        _hud = GetComponent<HUD>();
        _mainWeapon = GetComponent<MeleeWeapon>();
        _attackPoint = transform.Find("ShootPoint");
    }

    private bool HasAnySpells(){
        return (spells.Count > 0);
    }

    public void SetSpellAndAttackStats(PlayerData data){
        //Load spells based on the player stats defined
        //Creates a copy of the list to prevent changes on the Scriptable Object
        spells = new List<SpellData>(data.learnedSpells);
        attackRate = data.attackRate;
        nextAttackTime = data.nextAttackTime;
        _mainWeapon.SetWeaponData(data.meleeWeapon);
        currentIndex = 0;
        //
        //check if spells list is empty (Might happen if player is new)
        if(HasAnySpells()){ 
            currentSpell = spells[0];
            _hud.UpdateSpellUI(currentSpell);
        }    
    }
    
    // Update is called once per frame
    void Update()
    {
        //If game is paused any user input should be ignored
        if(PauseMenu.GameIsPaused) return;
        //
        //
        //
        //
        if(HasAnySpells()){
            // Mouse ScrollWheel logic to change beetween spells
            if (Input.GetAxis("Mouse ScrollWheel")>0f)
            {
                if (currentIndex >= spells.Count - 1)
                    currentIndex = 0;
                else
                    currentIndex++;
                currentSpell = spells[currentIndex];
                _hud.UpdateSpellUI(currentSpell);
            }
            if (Input.GetAxis("Mouse ScrollWheel")<0f)
            {
                if (currentIndex <= 0)
                    currentIndex = spells.Count - 1;
                else
                    currentIndex--;
                currentSpell = spells[currentIndex];
                _hud.UpdateSpellUI(currentSpell);
            }
        }
        else _hud.NoSpellUI();
        //
        //
        //
        //
        //
        if(Time.time >= nextAttackTime){
            //Mouse Logic to Attack or Throw spells
            if(Input.GetKeyDown(KeyCode.Mouse0)){
                //Left Shift key must be pressed in order to cast a spell
                if(Input.GetKey(KeyCode.LeftShift)){
                    if(currentSpell != null){
                        int currentManaCost = currentSpell.manaCost;
                        if (_player.GetMana() >= currentManaCost){
                            _player.SpendMana(currentManaCost);
                            CastSpell();
                        }
                        else 
                            //This will play the "not enough mana sound"
                            FindObjectOfType<AudioManager>().PlaySound("NoMana");
                    }
                }
                // Mouse click without holding the left shift key will result in basic melee attack
                else {
                    _mainWeapon.Attack(_attackPoint);
                    //TODO: Add Sword swinging sound
                    Debug.Log("Remember to add sword swinging sound!");    
                }
                nextAttackTime = Time.time + 1f/attackRate;
            }
        }
    }

    //Creates a proyectile based on the current selected spell by the user
    private void CastSpell()
    {
        GameObject spellProyectile = Instantiate(currentSpell.proyectilePrefab,_attackPoint.position,_attackPoint.rotation);
        spellProyectile.GetComponent<Proyectile>().setDamage(currentSpell.damage);
        Rigidbody2D proyectileRigidBody = spellProyectile.GetComponent<Rigidbody2D>();
        proyectileRigidBody.AddForce(_attackPoint.up * currentSpell.proyectileForce, ForceMode2D.Impulse);
        
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound(currentSpell.name);
    }
   
    //This method Adds a spell to the main character ONLY if the character doesn´t already have it
    public void AddSpell(string newSpellName){
        //Search if spell already obtained by the player
        if (spells.FindIndex(element => element.name == newSpellName ) != -1) return;
        //Search the spell data in the spell Database to add it
        SpellData newSpell = FindObjectOfType<SpellDatabase>().GetSpellByName(newSpellName);
        //Bug fix for no spell scenarios
        if (newSpell != null){
            spells.Add(newSpell);
            //Every time you get a new spell it automatically sets as your new selected spell
            currentIndex = spells.Count - 1;
            currentSpell = spells[currentIndex];
            _hud.UpdateSpellUI(newSpell); 
        }
        else Debug.Log("Spell not found and therefore not added!"); 
    }
    public List<SpellData> GetSpells(){
        return spells;
    }

    public SpellData GetCurrentSpell(){
        return currentSpell;
    }
}
