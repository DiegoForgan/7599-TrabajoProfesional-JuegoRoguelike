using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
   [SerializeField]
   private HealthBarWithAmount healthBar;
   [SerializeField]
   private ManaBar manaBar;
   [SerializeField]
   private TextMeshProUGUI gold;
   [SerializeField]
   private TextMeshProUGUI keys;
   [SerializeField]
   public TextMeshProUGUI spellText;
   [SerializeField]
   public Image spellAvatar;

   public void InitHUD(int maxHealth, int maxMana, int currentGold, int currentKeys){
       healthBar.initialize(maxHealth);
       manaBar.Initialize(maxMana);
       UpdateGold(currentGold);
       UpdateKeys(currentKeys);
   }

   public void UpdateHealth(int health){
       healthBar.SetHealth(health);
   }

   public void UpdateMaxHealth(int newMaxHealth){
       healthBar.SetMaxHealth(newMaxHealth);
   }

   public void UpdateMana(int mana){
       manaBar.SetBarMana(mana);
   }

   public void UpdateMaxMana(int newMaxMana){
       manaBar.SetMaxBarMana(newMaxMana);
   }

   public void UpdateGold(int goldValue){
       gold.SetText(": " + goldValue);
   }

   public void UpdateKeys(int keysValue){
       keys.SetText(": " + keysValue);
   }

   public void UpdateSpellUI(SpellData currentSpell){
       spellText.SetText(currentSpell.name + "\n\nMana cost: "  
                        + currentSpell.manaCost+ "   Spell Damage: " +
                        currentSpell.damage);
       spellAvatar.sprite = currentSpell.avatar;
   }
}
