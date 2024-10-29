using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIAutoAnimation), typeof(CanvasGroup))]
public class UIAnimationController : MonoBehaviour
{
    public bool isActive = false;
    CanvasGroup canvasGroup;
    UIAutoAnimation uIAutoAnimation;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        uIAutoAnimation = GetComponent<UIAutoAnimation>();
        UpdateCanvasGroup();
    }

    public void UpdateObjectChange()
    {
        uIAutoAnimation.SetAllEntranceState();
        uIAutoAnimation.GetComponentsList();
        if(!isActive)
        {
            uIAutoAnimation.SetAllExitState();
        } 
    }

    public void Activate()
    {
        if (!isActive)
        {
            CancelInvoke();
            isActive = true;
            UpdateCanvasGroup();

            //run animation
            uIAutoAnimation.EntranceAnimation();
        }
    }

    public void Deactivate()
    {
        if (isActive)
        {
            CancelInvoke();
            isActive = false;

            //run animation
            uIAutoAnimation.ExitAnimation();
            Invoke("UpdateCanvasGroup", 1f);
        }
    }

    public void UpdateCanvasGroup()
    {
        canvasGroup.alpha = isActive ? 1 : 0;
    }
}
