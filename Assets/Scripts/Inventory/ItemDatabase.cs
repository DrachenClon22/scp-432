using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDatabase : MonoBehaviour {

    public Sprite defaultSprite;

    public Item[] items;

    public Item getItemByID(int id)
    {
        for(int i = 0; i < items.Length; i++)
            if (items[i].ID == id)
                return items[i];

        return new Item();
    }
}

[System.Serializable]
public class Item
{
    public int ID;
    public string Title;
    public string Description;
    public bool Stackable;
    public Sprite Icon;

    public Item(int id, string title, string description, bool stackable)
    {
        this.ID = id;
        this.Title = title;
        this.Description = description;
        this.Stackable = stackable;
    }

    public Item(int id, string title, string description, bool stackable, Sprite icon)
    {
        this.ID = id;
        this.Title = title;
        this.Description = description;
        this.Stackable = stackable;
        this.Icon = icon;
    }

    public Item()
    {
        ID = -1;
    }
}
