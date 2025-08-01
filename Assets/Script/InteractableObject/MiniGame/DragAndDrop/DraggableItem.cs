using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;

    public string itemID;

    [HideInInspector] public Transform originalParent;

    [HideInInspector] public Transform parentAfterDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {

        originalParent = transform.parent;

        parentAfterDrag = transform.parent; 

        transform.SetParent(transform.root); 

        transform.SetAsLastSibling();

        image.raycastTarget = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
     transform.position = Input.mousePosition; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       transform.SetParent(parentAfterDrag);

       image.raycastTarget = true;
    }

    
}
