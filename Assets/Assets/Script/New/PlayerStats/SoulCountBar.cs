using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoulCountBar : MonoBehaviour
{
    public TextMeshProUGUI soulCountText;
    
    public void UpdateSoulCountText(int soulCount)
    {
        soulCountText.text = soulCount.ToString();
    }
}
