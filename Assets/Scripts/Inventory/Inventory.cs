using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ItemDatabase))]
public class Inventory : MonoBehaviour {

    public GameObject inventoryPanel;
    public GameObject slotPanel;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    private static List<Item> items = new List<Item>();
    private static List<GameObject> slots = new List<GameObject>();

    private int slotAmount = 18;

    private static ItemDatabase database;
    private static GameObject inventoryItem_static;

    public static void AddItem(int id)
    {
        Item itemToAdd = database.getItemByID(id);

        if (itemToAdd.ID != -1)
        {
            int index = 0;
            if (itemToAdd.Stackable && isItemInInventory(itemToAdd, out index))
            {
                ItemData data = slots[index].transform.GetChild(0).GetComponent<ItemData>();
                data.amount += 1;
                data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].ID == -1)
                    {
                        items[i] = itemToAdd;
                        GameObject itemObj = Instantiate(inventoryItem_static);

                        itemObj.GetComponent<Image>().sprite = (itemToAdd.Icon) ? itemToAdd.Icon : database.defaultSprite;
                        itemObj.GetComponent<ItemData>().item = itemToAdd;
                        itemObj.name = itemToAdd.Title;
                        itemObj.transform.SetParent(slots[i].transform);
                        itemObj.transform.localPosition = Vector2.zero;

                        slots[i].GetComponent<Slot>().storage = itemObj.GetComponent<ItemData>();

                        UnityEngine.Events.UnityAction action = new UnityEngine.Events.UnityAction(() => { });
                        switch(itemToAdd.ID)
                        {
                            case 1:
                                {
                                    action = new UnityEngine.Events.UnityAction(() => Manage_Objects.ChooseWeapon(Manage_Objects.currentWeapon.CAMERA));
                                    break;
                                }
                            case 2:
                                {
                                    action = new UnityEngine.Events.UnityAction(() => Manage_Objects.ChooseWeapon(Manage_Objects.currentWeapon.FLASHLIGHT));
                                    break;
                                }
                            case 3:
                                {
                                    action = new UnityEngine.Events.UnityAction(() => Manage_Objects.UseBattery());
                                    break;
                                }
                            case 4:
                                {
                                    action = new UnityEngine.Events.UnityAction(() => Manage_Objects.ShowDocument());
                                    break;
                                }
                        }
                        itemObj.GetComponent<Button>().onClick.AddListener(action);

                        break;
                    }
                }
            }
        }
    }

    public static void RemoveItem(int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == id)
            {
                if (slots[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text == "")
                {
                    slots[i].GetComponent<Slot>().storage = null;
                    items.Remove(items[i]);

                    Destroy(slots[i].transform.GetChild(0).GetComponent<Image>());
                    slots[i].transform.GetChild(0).SetParent(slots[i].transform.parent.parent);
                } else
                {
                    int amount = int.Parse(slots[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text);
                    slots[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = (amount == 2) ? "" : (amount - 1).ToString();
                }
                break;
            }
        }
    }

    private void Awake()
    {
        slots.Clear();
        items.Clear();
    }

    private void Start()
    {
        inventoryItem.GetComponent<RectTransform>().sizeDelta = GridScaleFix.size;        
        inventoryItem_static = inventoryItem;
        database = GetComponent<ItemDatabase>();

        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());

            slots.Add(Instantiate(inventorySlot));
            slots[i].transform.SetParent(slotPanel.transform);
            slots[i].AddComponent<Slot>();
        }

        AddItem(1);
        AddItem(2);

        AddItem(3);
        AddItem(3);
        AddItem(3);

        AddItem(4);
    }

    private static bool isItemInInventory(Item item, out int index)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == item.ID)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }
}
