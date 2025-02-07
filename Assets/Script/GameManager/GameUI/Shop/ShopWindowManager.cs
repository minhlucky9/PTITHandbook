using Interaction;
using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindowManager : MonoBehaviour
{
    public static ShopWindowManager instance;

    public GameObject categoryPrefab;
    public GameObject productPrefab;
    public Transform itemContainer;
    public Scrollbar containerScroll;
    public Button prevBtn;
    public Button nextBtn;
    public Button closeBtn;

    UIAnimationController openAnimation;
    ShopWindowDataSO data;
    int currentStep = 0;
    int stemNums = 5;

    private void Awake()
    {
        instance = this;
        openAnimation = GetComponent<UIAnimationController>();
    }

    private void Start()
    {
        prevBtn.onClick.AddListener(delegate { PreviousTab(); });
        nextBtn.onClick.AddListener(delegate { NextTab(); });
        closeBtn.onClick.AddListener(delegate { CloseWindow(); });
    }

    public void OpenWindow(ShopWindowDataSO data)
    {
        
        this.data = data;
        InitShop();
        StartCoroutine(ActivateWindow());
    }

    public void InitShop()
    {
        //delete old objects
        for(int i = 0; i < itemContainer.childCount; i ++)
        {
            Destroy(itemContainer.GetChild(i).gameObject);
        }

        //create new objects
        for (int i = 0; i < data.shopCategories.Count; i ++)
        {
            GameObject category = Instantiate(categoryPrefab, itemContainer);
            List<ShopItem> items = data.shopCategories[i].shopItems;
            
            for(int j = 0; j < items.Count; j ++)
            {
                ShopProductUI productUI = Instantiate(productPrefab, category.transform).GetComponent<ShopProductUI>();
                productUI.Init(items[j]);
            }
        }

        stemNums = data.shopCategories.Count;

        openAnimation.UpdateObjectChange();
    }

    IEnumerator ActivateWindow()
    {
        yield return ConservationManager.instance.DeactivateConservationDialog();
        yield return new WaitForSeconds(0.7f);
        openAnimation.Activate();
    }

    public void CloseWindow()
    {
        StartCoroutine(DeactivateWindow());
    }

    IEnumerator DeactivateWindow()
    {
        openAnimation.Deactivate();
        yield return new WaitForSeconds(0.7f);
        PlayerManager.instance.ActivateController();
    }

    public void NextTab()
    {
        currentStep++;
        currentStep = currentStep % stemNums;
        containerScroll.value = currentStep * (1f / (stemNums - 1));
    }

    public void PreviousTab()
    {
        currentStep--;
        if(currentStep < 0)
        {
            currentStep = stemNums - 1;
        }
        containerScroll.value = currentStep * (1f / (stemNums - 1));
    }
}

