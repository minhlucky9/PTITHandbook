using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(UIAnimationController))]
public class UIAnimationControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UIAnimationController uiAnimation = (UIAnimationController)target;

        if(!Application.isPlaying)
        {
            uiAnimation.GetComponent<CanvasGroup>().alpha = uiAnimation.isActive ? 1 : 0;
            uiAnimation.GetComponent<CanvasGroup>().interactable = uiAnimation.isActive;
            uiAnimation.GetComponent<CanvasGroup>().blocksRaycasts = uiAnimation.isActive;
        }
            
    }
}
#endif