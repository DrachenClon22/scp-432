using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public Item item;
    public int amount = 1;
    public Transform curParent;

    private Vector2 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = (Vector2)transform.position - eventData.position;

        if (item != null)
        {
            if (transform.parent.parent.GetComponent<GridLayoutGroup>().enabled)
                transform.parent.parent.GetComponent<GridLayoutGroup>().enabled = false;

            curParent = transform.parent;

            curParent.GetComponent<Slot>().storage = null;

            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.position = eventData.position;
            transform.SetParent(transform.parent.parent);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            transform.position = eventData.position + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        curParent.GetComponent<Slot>().storage = this;
        StartCoroutine(EndDrag());
    }

    private IEnumerator EndDrag()
    {
        while ((transform.position - curParent.position).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector2.Lerp(transform.position, curParent.position, 0.5f);
            yield return null;
        }

        transform.SetParent(curParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
