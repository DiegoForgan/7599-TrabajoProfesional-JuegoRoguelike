using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponManagement : MonoBehaviour
{
    public List<SpellData> spells;
    private MeleeWeapon _mainWeapon;
    //CoolDown parameters to prevent spamming attacks
    private float attackRate = 2f;
    private float nextAttackTime = 0f;
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
    
    
    // Start is called before the first frame update
    void Start()
    {
        //Test Granted Spells
        AddSpell("Basic Spell");
        AddSpell("Fire Spell");
        AddSpell("Poison Spell");
        AddSpell("Hammer Throw");
        //
        //
        currentIndex = 0;
        currentSpell = spells[0];
        _hud.UpdateSpellUI(currentSpell);
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
                    int currentManaCost = currentSpell.manaCost;
                    if (_player.GetMana() >= currentManaCost){
                        _player.SpendMana(currentManaCost);
                        CastSpell();
                    }
                    else 
                    //This will play the "not enough mana sound"
                    FindObjectOfType<AudioManager>().PlaySound("NoMana");
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
        spells.Add(newSpell);
    }
    public List<SpellData> GetSpells(){
        return spells;
    }

    public SpellData GetCurrentSpell(){
        return currentSpell;
    }
}
