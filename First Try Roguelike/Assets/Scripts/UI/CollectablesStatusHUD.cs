using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class CollectablesStatusHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldCollected;
    [SerializeField] private Image keyFoundStatus;
    [SerializeField] private Sprite keyFound;
    [SerializeField] private Sprite keyNotFound;


    public void initializeCollectablesStatus(int currentGold, int currentKeys)
    {
        updateGoldCollected(currentGold);
        updateKeysCollected(currentKeys);
    }

    internal void updateGoldCollected(int goldValue)
    {
        //TODO: Add condition to format 1000 to 1k maybe
        goldCollected.SetText(goldValue.ToString());
    }

    internal void updateKeysCollected(int keysValue)
    {
        if (keysValue > 0) keyFoundStatus.sprite = keyFound;
        else keyFoundStatus.sprite = keyNotFound;
    }
}