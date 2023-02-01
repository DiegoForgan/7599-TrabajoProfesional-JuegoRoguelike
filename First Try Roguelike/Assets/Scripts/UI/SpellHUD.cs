using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SpellHUD : MonoBehaviour
{
    //Spell HUD components
    [SerializeField] TextMeshProUGUI spellName;
    [SerializeField] TextMeshProUGUI spellCost;
    [SerializeField] TextMeshProUGUI spellDamage;
    [SerializeField] Image spellAvatar;

    [Space]
    //Spell Effects HUD components
    private Dictionary<EffectType,GameObject> spellEffects;
    [SerializeField] private GameObject NoEffect;
    [SerializeField] private GameObject BurnEffect;
    [SerializeField] private GameObject FreezeEffect;
    [SerializeField] private GameObject PoisonEffect;

    //Constants and Defaults
    private const string DEFAULT_SPELL_NAME = "";
    private const string DEFAULT_SPELL_COST = "0";
    private const string DEFAULT_SPELL_DAMAGE = "0";
    private const EffectType DEFAULT_SPELL_EFFECT = EffectType.None;

    private void Awake()
    {
        spellEffects = new Dictionary<EffectType, GameObject>();
        // Add here more effects if created
        spellEffects.Add(EffectType.None, NoEffect);
        spellEffects.Add(EffectType.Burn, BurnEffect);
        spellEffects.Add(EffectType.Freeze, FreezeEffect);
        spellEffects.Add(EffectType.Poison, PoisonEffect);
    }

    internal void initializeSpellHUD()
    {
        spellName.SetText(DEFAULT_SPELL_NAME);
        spellCost.SetText(DEFAULT_SPELL_COST);
        spellDamage.SetText(DEFAULT_SPELL_DAMAGE);
        this.setSpellEffect(DEFAULT_SPELL_EFFECT);
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
