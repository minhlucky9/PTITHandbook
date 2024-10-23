using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI goldText2;

    private void OnEnable() 
    {
        GameEventsManager.instance.goldEvents.onGoldChange += GoldChange;
    }

    private void OnDisable() 
    {
        GameEventsManager.instance.goldEvents.onGoldChange -= GoldChange;
    }

    private void GoldChange(int gold) 
    {
        goldText.text = gold.ToString();
        goldText2.text = gold.ToString();
    }
}
