using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour
{
    [SerializeField] int MaxPage;
    int currentPage;
    Vector3 targetPro;
    [SerializeField] Vector3 PageStep;
    [SerializeField] RectTransform LevelPagesRect;

    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;

    private void Awake()
    {
        currentPage = 1;
        targetPro = LevelPagesRect.localPosition;
    }

    public void Next()
    {
        if (currentPage < MaxPage)
        {
            currentPage++;
            targetPro += PageStep;
            MovePage(); 
        }
    }

    public void Previous()
    {
        if(currentPage > 1)
        {
            currentPage--;
            targetPro -= PageStep;
            MovePage();
        }
    }

    public void MovePage()
    {
        LevelPagesRect.LeanMoveLocal(targetPro, tweenTime).setEase(tweenType);
    }

}
