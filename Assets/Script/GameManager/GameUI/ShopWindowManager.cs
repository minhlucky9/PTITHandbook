using Interaction;
using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindowManager : MonoBehaviour
{
    public static ShopWindowManager instance;

    ShopWindowDataSO data;
    public Transform itemContainer;
    public Scrollbar containerScroll;
    public Button prevBtn;
    public Button nextBtn;
    public Button closeBtn;
    
    UIAnimationController openAnimation;
    
    int currentStep = 0;

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
        StartCoroutine(ActivateWindow());
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
        containerScroll.value = currentStep * (1f / containerScroll.numberOfSteps);
    }

    public void PreviousTab()
    {
        currentStep--;
        containerScroll.value = currentStep * (1f / containerScroll.numberOfSteps);
    }
}
