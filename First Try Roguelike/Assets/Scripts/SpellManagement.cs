using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpellManagement : MonoBehaviour
{
    public List<Spell> spells;
    private Spell currentSpell;

    private Entity _entity;

    private int currentIndex;
    public Transform castPoint;

    public TextMeshProUGUI spellText;
    public Image spellAvatar;
    
    private void Awake() {
        _entity = GetComponent<Entity>();
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        AddSpell("Basic Spell");
        AddSpell("Fire Spell");
        AddSpell("Poison Spell");
        currentIndex = 0;
        currentSpell = spells[0];
        ShowSpellDataOnUI();
    }

    // Update is called once per frame
    void Update()
    {
        if(PauseMenu.GameIsPaused == false && spells.Count != 0){
            //This code I think its highly refactorable
            if (Input.GetAxis("Mouse ScrollWheel")>0f)
            {
                if (currentIndex >= spells.Count - 1)
                    currentIndex = 0;
                else
                    currentIndex++;
                currentSpell = spells[currentIndex];
                ShowSpellDataOnUI();
            }
            if (Input.GetAxis("Mouse ScrollWheel")<0f)
            {
                if (currentIndex <= 0)
                    currentIndex = spells.Count - 1;
                else
                    currentIndex--;
                currentSpell = spells[currentIndex];
                ShowSpellDataOnUI();
            }
            int currentManaCost = currentSpell.GetSpellManaCost();
            if(Input.GetButtonDown("Fire1") && (_entity.GetMana() >= currentManaCost)){
                _entity.SpendMana(currentManaCost);
                CastSpell();
            }
        }

        
    }

    //Creates a proyectile based on the current selected spell by the user
    private void CastSpell()
    {
        GameObject spellProyectile = Instantiate(currentSpell.GetSpellPrefab(),castPoint.position,castPoint.rotation);
        spellProyectile.GetComponent<Proyectile>().setDamage(currentSpell.GetSpellDamage());
        Rigidbody2D proyectileRigidBody = spellProyectile.GetComponent<Rigidbody2D>();
        proyectileRigidBody.AddForce(castPoint.up * currentSpell.spellproyectileForce, ForceMode2D.Impulse);
        //This command plays the desired sound clip
        FindObjectOfType<AudioManager>().PlaySound(currentSpell.GetSpellName());
    }

    private void ShowSpellDataOnUI(){
        spellText.SetText(currentSpell.GetSpellName() + "\n\nMana cost: "  
                        + currentSpell.GetSpellManaCost()+ "   Spell Damage: " +
                        currentSpell.GetSpellDamage() );
        spellAvatar.sprite = currentSpell.GetSpellAvatar();
    }
    private void ChangeCurrentWeapon(int currentIndex)
    {
        if(currentIndex > (spells.Count-1)) currentIndex = 0;
        else if (currentIndex < 0) currentIndex = spells.Count-1;
        currentSpell = spells[currentIndex];
        Debug.Log(currentIndex);
        ShowSpellDataOnUI();
    }

    //This method Adds a spell to the main character ONLY if the character doesn´t already have ir
    public void AddSpell(string newSpellName){
        //Search if spell already obtained by the player
        if (spells.FindIndex(element => element.spellName == newSpellName ) != -1) return;
        //Search the spell data in the spell Database to add it
        Spell newSpell = FindObjectOfType<SpellDatabase>().GetSpellByName(newSpellName);
        spells.Add(newSpell);
    }
    public List<Spell> GetSpells(){
        return spells;
    }

    public Spell GetCurrentSpell(){
        return currentSpell;
    }
}
