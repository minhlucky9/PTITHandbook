using PlayerController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public TMP_Text goldText;
    public GameObject itemPrefab;
    public Transform inventoryContainer;
    public Button closeBtn;

    UIAnimationController uiAnimation;

    private void Awake()
    {
        uiAnimation = GetComponent<UIAnimationController>();
        closeBtn.onClick.AddListener(delegate
        {
            StartCoroutine(CloseWindow());
        });
        //
    }

    private void Start()
    {
        PlayerInventory.instance.OnGoldChanged += UpdateGoldText;
        PlayerInventory.instance.OnInventoryUpdated += OnInventoryChange;
    }

    public void UpdateGoldText(int changeAmount, int goldAmount)
    {
        goldText.text = goldAmount.ToString();
    }

    public void OnInventoryChange(Dictionary<int, InventoryItem> inventory)
    {
        InitInventory();
    }

    public void InitInventory()
    {
        List<InventoryItem> inventoryItems = PlayerInventory.instance.inventoryItems;
        //

        for(int i = 0; i < inventoryItems.Count; i ++)
        {
            InventoryItemUI inventoryItemUI = inventoryContainer.GetChild(i).GetComponent<InventoryItemUI>();
            inventoryItemUI.ResetInventorySlot();
            
            if (!inventoryItems[i].IsEmpty)
            {
                Debug.Log("Inventory Item: " + inventoryItems[i].item.itemName + " Quantity: " + inventoryItems[i].quantity);
                inventoryItemUI.InitInventorySlot(inventoryItems[i], i);
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.I))
        {
            InitInventory();
            StartCoroutine(OpenWindow());
        }
    }

    public IEnumerator CloseWindow()
    {
        uiAnimation.Deactivate();
        yield return new WaitForSeconds(0.7f);
        PlayerManager.instance.ActivateController();
    }

    public IEnumerator OpenWindow()
    {

        PlayerManager.instance.DeactivateController();
        yield return new WaitForSeconds(0.7f);
        uiAnimation.Activate();
    }
}
