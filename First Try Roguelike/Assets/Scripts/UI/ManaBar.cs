using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI barText;
    
    
    public void Initialize(int mana){
        SetMaxBarMana(mana);
        SetBarMana(mana);
    }
    
    public void SetBarMana(int mana){
        slider.value = mana;
        barText.SetText(slider.value+" / "+ slider.maxValue);
    }

    public void SetMaxBarMana(int newMaxMana){
        slider.maxValue = newMaxMana;
        barText.SetText(slider.value+" / "+ slider.maxValue);
    }

}
