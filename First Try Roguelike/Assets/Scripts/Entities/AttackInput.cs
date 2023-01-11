using UnityEngine;

public class AttackInput
{
    private KeyCode meleeAttackKey = KeyCode.Mouse0;
    private KeyCode spellCastKey = KeyCode.Mouse1;
    private string spellSelector = "Mouse ScrollWheel";
    private bool meleeAttackKeyPressed = false;
    private bool spellCastKeyPressed = false;
    private bool nextSpell = false;
    private bool previousSpell = false;


    public bool isMeleeAttackKeyPressed()
    {
        return meleeAttackKeyPressed;
    }

    public bool isSpellCastKeyPressed()
    {
        return spellCastKeyPressed;
    }

    public bool shouldDisplayNextSpell()
    {
        return nextSpell;
    }

    public bool shouldDisplayPreviousSpell()
    {
        return previousSpell;
    }

    public void updateInputs() {
        meleeAttackKeyPressed = Input.GetKeyDown(meleeAttackKey);
        spellCastKeyPressed = Input.GetKeyDown(spellCastKey);
        
        float spellSelectorValue = Input.GetAxis(spellSelector);
        nextSpell = (spellSelectorValue > 0f);
        previousSpell = (spellSelectorValue < 0f);
    }

}
