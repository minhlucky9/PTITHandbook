using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using static GlobalResponseData_Login;
using static scr_PlayerController;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        public static InventoryController Instance { get; private set; }

        public scr_PlayerController playerController;
        public scr_PlayerController playerController2;
        public Character3D_Manager_Ingame character;
        [SerializeField]
        private TextMeshProUGUI effectDurationText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        [SerializeField]
        private UIInventoryPage inventoryUI;

        [SerializeField]
        private InventorySO inventoryData;


        public List<InventoryItem> initialItems = new List<InventoryItem>();


        [SerializeField]
        private GameObject studentCardObject; // The GameObject to be activated or deactivated

        [SerializeField]
        private GameObject PermitA2;
        [SerializeField]
        private GameObject PermitA2Class;
        [SerializeField]
        private GameObject EnterA2;
        [SerializeField]
        private GameObject EnterA2Class;

        [SerializeField]
        private HealthBar healthBar; // Reference to the HealthBar script
        [SerializeField]
        private StaminaBar staminahBar; // Reference to the HealthBar script

        private void Start()
        {
            initialItems = GlobalResponseData.inventoryItems;
            PrepareUI();
            PrepareInventoryData();
            CheckForStudentCard();
        }


        private void PrepareInventoryData()
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItem item in initialItems)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        private void PrepareUI()
        {
            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                    item.Value.quantity);
            }
            CheckForStudentCard(); // Check for student card whenever inventory is updated
        }

        private void HandleItemActionRequest(int itemIndex)
        {

        }

        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            inventoryData.SwapItems(itemIndex_1, itemIndex_2);
        }

        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            //  string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage,
                item.name, item.Description);
        }

        public void UseItem(int itemIndex)
        {
            InventoryItem item = inventoryData.GetItemAt(itemIndex);
            if (item.IsEmpty) return;

            ItemSO itemDetails = item.item;

            // Check for health increase
            if (itemDetails.Tags.Contains("Food"))
            {
                healthBar.SetHealth(healthBar.slider.value + itemDetails.HealthIncrease);
            }
            // Speed boost effect
            if (itemDetails.Tags.Contains("SpeedBoost"))
            {
                if(character.index == 0)
                {
                    playerController.StartSpeedBoost(itemDetails.SpeedMultiplier, itemDetails.EffectDuration);
                }
                else
                {
                    playerController2.StartSpeedBoost(itemDetails.SpeedMultiplier, itemDetails.EffectDuration);
                }
            }
               

            // Mana lock effect (stamina lock)
            if (itemDetails.Tags.Contains("StaminaLock"))
            {
                if (character.index == 0)
                {
                    playerController.StartStaminaLock(itemDetails.EffectDuration);
                }
                else
                {
                    playerController2.StartStaminaLock(itemDetails.EffectDuration);
                }
                
            }
            if (!itemDetails.Tags.Contains("NonConsumable"))
            {
                // Decrease the item quantity by 1
                inventoryData.RemoveItem(itemIndex, 1);
            }
        } 


        private void CheckForStudentCard()
        {
            bool hasStudentCard = true;
            bool IsEnterUI = false;
            foreach (var item in inventoryData.GetCurrentInventoryState())
            {
                if (item.Value.item.Name == "thẻ sinh viên")
                {
                    hasStudentCard = false;
                    IsEnterUI = true;
                    break;
                }
            }
            studentCardObject.SetActive(hasStudentCard);
            PermitA2.SetActive(hasStudentCard);
            EnterA2.SetActive(IsEnterUI);
            PermitA2Class.SetActive(hasStudentCard);
            EnterA2Class.SetActive(IsEnterUI);
        }


        public void Update()
        {
            CheckForStudentCard();

            if (inventoryUI.isActiveAndEnabled == false)
            {
                inventoryUI.Show();
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key,
                        item.Value.item.ItemImage,
                        item.Value.quantity);
                }
            }


            if (playerController.speedBoostDuration > 0 || playerController2.speedBoostDuration > 0)
            {
                if(character.index == 0)
                {
                    effectDurationText.text = $"X2 Speed Boost";
                }
                else
                {
                    effectDurationText.text = $"X2 Speed Boost";
                }
                
            }
            else if (playerController.isStaminaLocked || playerController2.isStaminaLocked)
            {
                effectDurationText.text = "";
            }
            else
            {
                effectDurationText.text = "";
            }

        }

    }
}