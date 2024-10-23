using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalResponseData_Login;

public class GoldManager : MonoBehaviour
{
  //  public PlayfabMannager playfabmanager;
    [Header("Configuration")]
    [SerializeField] private int startingGold = 5;

    public int currentGold { get; private set; }

    private void Awake()
    {
        currentGold = GlobalResponseData.gold;
    }

    private void Update()
    {
        /*
        if (currentGold >= 6)
        {
            playfabmanager.SendLeaderBoard(currentGold);

        }
        */
    }
    private void OnEnable() 
    {
        GameEventsManager.instance.goldEvents.onGoldGained += GoldGained;
    }

    private void OnDisable() 
    {
        GameEventsManager.instance.goldEvents.onGoldGained -= GoldGained;
    }

    private void Start()
    {
        GameEventsManager.instance.goldEvents.GoldChange(currentGold);
    }

    private void GoldGained(int gold) 
    {
        currentGold += gold;
        GameEventsManager.instance.goldEvents.GoldChange(currentGold);
    }



    public void SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            GameEventsManager.instance.goldEvents.GoldChange(currentGold);
        }
    }
}
