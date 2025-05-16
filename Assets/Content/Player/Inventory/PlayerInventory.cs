using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string name;
    public string type;
    public int quantity;
    public int maxStack;
    public int durability;
    public int damage;
    public string[] effects;

    public InventoryItem(string name, string type, int quantity, int maxStack, int durability, int damage, string[] effects)
    {
        this.name = name;
        this.type = type;
        this.quantity = quantity;
        this.maxStack = maxStack;
        this.durability = durability;
        this.damage = damage;
        this.effects = effects;
    }
}

[System.Serializable]
public class PlayerInventorySystem
{
    public InventoryItem mainSlot;
    public InventoryItem secondSlot;
    public List<InventoryItem> specialSlots = new List<InventoryItem>(3);
    public List<InventoryItem> throwableSlots = new List<InventoryItem>(3);
    public InventoryItem healthSlot;
    public List<InventoryItem> additionalSlots = new List<InventoryItem>(9);

    public void AddItem(InventoryItem item)
    {
        if (item == null) return;

        // Check if item can stack
        foreach (var slot in additionalSlots)
        {
            if (slot != null && slot.name == item.name && slot.quantity < slot.maxStack)
            {
                int spaceLeft = slot.maxStack - slot.quantity;
                int amountToAdd = Mathf.Min(spaceLeft, item.quantity);
                slot.quantity += amountToAdd;
                item.quantity -= amountToAdd;
                if (item.quantity <= 0) return;
            }
        }

        // Add to first available slot
        for (int i = 0; i < additionalSlots.Count; i++)
        {
            if (additionalSlots[i] == null)
            {
                additionalSlots[i] = item;
                return;
            }
        }

        Debug.Log("Inventory is full!");
    }

    public void DropItem(int index)
    {
        if (index >= 0 && index < additionalSlots.Count && additionalSlots[index] != null)
        {
            Debug.Log("Dropped: " + additionalSlots[index].name);
            additionalSlots[index] = null;
        }
    }

    public void UseItem(int index)
    {
        if (index >= 0 && index < additionalSlots.Count && additionalSlots[index] != null)
        {
            Debug.Log("Used: " + additionalSlots[index].name);
            additionalSlots[index].quantity--;
            if (additionalSlots[index].quantity <= 0)
            {
                additionalSlots[index] = null;
            }
        }
    }
}
