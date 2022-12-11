using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class CollectablesStatusHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldCollected;
    [SerializeField] private Toggle keysCollected;

    public void initializeCollectablesStatus(int currentGold, int currentKeys)
    {
        //goldCollected.SetText(currentGold.ToString());
        //keysCollected.isOn = (currentKeys > 0);   
    }

    internal void updateGoldCollected(int goldValue)
    {
        //goldCollected.SetText(goldValue.ToString());
    }

    internal void updateKeysCollected(int keysValue)
    {
        
    }
}