using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {

    public ItemData storage;

    public void OnDrop(PointerEventData eventData)
    {
        if (!storage)
        {
            storage = eventData.pointerDrag.GetComponent<ItemData>();
            storage.curParent = transform;
        }
    }
}
