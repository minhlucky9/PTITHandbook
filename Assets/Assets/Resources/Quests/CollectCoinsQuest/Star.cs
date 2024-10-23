using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Star : MonoBehaviour
{
    public static Star instance; // Singleton instance

    [Header("Configuration")]
    public int star = 0;
    public TextMeshProUGUI StarScore;
    public Slider StarSlider;
    public int currentGold { get; private set; }

    private void Update()
    {
        StarScore.text = star + "/15";
        StarSlider.value = star;
    }

    private void Awake()
    {
        // Implement the singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy any duplicate instances
        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onStarCollected += GoldGained;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onStarCollected -= GoldGained;
    }

    private void GoldGained()
    {
        star++;
    }

    public int GetStar()
    {
        return star;
    }
}
