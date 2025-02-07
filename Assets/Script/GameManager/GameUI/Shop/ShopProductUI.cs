using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopProductUI : MonoBehaviour
{
    public Image productImage;
    public TMP_Text priceText;
    public TMP_Text valueText;
    public Image valueIcon;
    public Button buyBtn;

    ShopItem shopItem;

    public void Init(ShopItem item)
    {
        shopItem = item;
        productImage.sprite = item.itemInfo.itemImage;
        priceText.text = item.price.ToString();
        valueText.text = item.value;
        valueIcon.sprite = item.valueIcon;
    }
}
