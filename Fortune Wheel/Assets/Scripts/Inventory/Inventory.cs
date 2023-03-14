using System.Collections.Generic;
using ScriptableObjects;

[System.Serializable]
public static class Inventory
{
    public static List<InventorySlot> Slots = new List<InventorySlot>();

    public static void AddNewItem(InventorySlot slot)
    {
        for (var i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].prize != slot.prize) continue;
            Slots[i].amount += slot.amount;
            return;
        }

        var newItem = new InventorySlot
        {
            prize = slot.prize,
            amount = slot.amount
        };
        Slots.Add(newItem);
        
        SaveInventory();
    }
    
    public static void SaveInventory()
    {
        FileHandler.SaveListToJson(Slots, "Inventory.json");
    }
    
    public static void LoadInventory()
    {
        Slots = FileHandler.ReadListFromJson<InventorySlot>("Inventory.json");
    }
    
    public static string SetAmountText(int amount)
    {
        if (amount <= 0) return "";
        if (amount < 1000) return "x" + amount;
        
        var thousand = amount / 1000;
        var hundred = (amount - thousand * 1000) / 100;
        return "x" + thousand + "." + hundred + "K";
    }
}
