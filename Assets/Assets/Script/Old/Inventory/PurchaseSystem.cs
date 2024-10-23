using Inventory.Model;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseSystem : MonoBehaviour
{
    [SerializeField]
    private InventorySO inventoryData;
    [SerializeField]
    private GoldManager goldManager;
    public GameObject SuccessBuy;
    public GameObject FailedBuy;
    public GameObject NoInventoySlot;
    public Button purchaseButton;
    public ItemSO itemToPurchase;
    public int itemCost;

    private void Start()
    {
        purchaseButton.onClick.AddListener(PurchaseItem);
    }

    private void PurchaseItem()
    {
        if (goldManager.currentGold >= itemCost)
        {
            int reminder = inventoryData.AddItem(itemToPurchase, 1);
            if (reminder == 0)
            {
                goldManager.SpendGold(itemCost);
                Debug.Log("Item purchased successfully!");
                SuccessBuy.SetActive(true);
            }
            else
            {
                Debug.Log("Inventory is full, item not purchased.");
                NoInventoySlot.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Not enough gold to purchase this item.");
            FailedBuy.SetActive(true);
        }
    }
}
