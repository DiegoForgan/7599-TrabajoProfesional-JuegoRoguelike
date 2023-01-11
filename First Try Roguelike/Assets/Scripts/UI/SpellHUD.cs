using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SpellHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI spellName;
    [SerializeField] TextMeshProUGUI spellCost;
    [SerializeField] TextMeshProUGUI spellDamage;
    [SerializeField] Image spellAvatar;
    private Dictionary<EffectType,GameObject> spellEffects;

    private void Awake()
    {
        spellEffects = new Dictionary<EffectType, GameObject>();
        // Add here more effects if created
        spellEffects.Add(EffectType.None, GameObject.Find("NoEffect"));
        spellEffects.Add(EffectType.Burn, GameObject.Find("BurnEffect"));
        spellEffects.Add(EffectType.Freeze, GameObject.Find("FreezeEffect"));
        spellEffects.Add(EffectType.Poison, GameObject.Find("PoisonEffect"));
    }

    internal void initializeSpellHUD()
    {
        spellName.SetText("");
        spellCost.SetText("0");
        spellDamage.SetText("0");
        this.setSpellEffect(EffectType.None);
    }

    internal void updateSpellHUD(SpellData currentSpell)
    {
        spellName.SetText(currentSpell.name);
        spellCost.SetText(currentSpell.manaCost.ToString());
        spellDamage.SetText(currentSpell.damage.ToString());
        spellAvatar.sprite = currentSpell.avatar;
        this.setSpellEffect(currentSpell.effect.type);
    }

    private void setSpellEffect(EffectType effectType)
    {
        foreach (GameObject value in spellEffects.Values)
        {
            value.SetActive(false);
        }
        spellEffects[effectType].SetActive(true);
    }
    
}
