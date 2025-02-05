using Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Functional/Shop Data")]
public class ShopWindowDataSO : FunctionalWindowDataSO
{
    public List<ShopItem> shopItems;

    public override void Init(GameObject target)
    {
        base.Init(target);
        ShopWindowManager.instance.OpenWindow(this);
    }

    public void BuyItem(int index)
    {

    }
}

[Serializable]
public struct ShopItem
{
    public ItemSO itemInfo;
    public int price;
    public int amount;
}