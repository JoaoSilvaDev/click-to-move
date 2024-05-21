using System;
using Unity.Netcode;
using UnityEngine;

public class UIItemList : MonoBehaviour
{
    public UIItemSlot[] itemSlots;

    public void DisplayItems(NetworkList<InventoryItem> items)
    {
        // print($"DisplayItems items:{items.Count} itemSlots:{itemSlots.Length}");

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].IsEmpty)
                itemSlots[i].SetEmpty();
            else
                itemSlots[i].SetItemData(
                    CatalogueManager.Instance.ItemCatalogue.GetItemData(items[i].ID), items[i].quantity);
        }
    }

    public void DisplayItems(uint[] IDs, uint[] quantities)
    {
        // print($"items:{IDs.Length} itemSlots:{itemSlots.Length}");

        for (int i = 0; i < IDs.Length; i++)
        {
            if (IDs[i] == 0)
                itemSlots[i].SetEmpty();
            else
                itemSlots[i].SetItemData(
                    CatalogueManager.Instance.ItemCatalogue.GetItemData(IDs[i]), quantities[i]);
        }
    }
}