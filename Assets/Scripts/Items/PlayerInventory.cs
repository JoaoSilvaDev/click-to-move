using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct InventoryItem : INetworkSerializable, IEquatable<InventoryItem>
{
    public uint ID;
    public uint quantity;

    public InventoryItem(uint ID, uint quantity)
    {
        this.ID = ID;
        this.quantity = quantity;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out ID);
            reader.ReadValueSafe(out quantity);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(ID);
            writer.WriteValueSafe(quantity);
        }
    }

    public bool Equals(InventoryItem other)
    {
        throw new NotImplementedException();
    }
}

public class PlayerInventory : NetworkBehaviour
{
    private NetworkList<InventoryItem> items;
    private NetworkVariable<bool> dirty = new NetworkVariable<bool>(
        false,
        readPerm: NetworkVariableReadPermission.Owner,
        writePerm: NetworkVariableWritePermission.Server
    );
    public Action<NetworkList<InventoryItem>> OnInventoryChanged;

    // variables
    // private int inventorySize = 8;
    // private Slot handSlot; // this holds the items in the players "hand" when they are dragging items

    private void Awake()
    {
        items = new NetworkList<InventoryItem>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            items.Add(new InventoryItem(0, 0));
            items.Add(new InventoryItem(0, 0));
            items.Add(new InventoryItem(0, 0));
            items.Add(new InventoryItem(0, 0));

            items.Add(new InventoryItem(0, 0));
            items.Add(new InventoryItem(0, 0));
            items.Add(new InventoryItem(0, 0));
            items.Add(new InventoryItem(0, 0));
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (dirty.Value)
        {
            OnInventoryChanged?.Invoke(items);
            SetDirty(false);
        }
    }

    #region Add Item
    public void SetItem(InventoryItem item, int slotIndex)
    {
        // print("SetItem");
        if (IsServer)
            SetItemValue(item, slotIndex);
        else
            SetItemValueServerRpc(item, slotIndex);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetItemValueServerRpc(InventoryItem item, int slotIndex)
    {
        // print("SetItemValueServerRpc");
        SetItemValue(item, slotIndex);
    }
    private void SetItemValue(InventoryItem item, int slotIndex)
    {
        // print("SetItemValue");
        items[slotIndex] = item;
        SetDirty(true);
    }

    #endregion

    public void SetDirty(bool value)
    {
        if (IsServer)
            SetDirtyValue(value);
        else
            SetDirtyValueServerRpc(value);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetDirtyValueServerRpc(bool value) { SetDirtyValue(value); }
    private void SetDirtyValue(bool value) { dirty.Value = value; }

    // [ServerRpc(RequireOwnership = false)]
    // private void PopulateEmptyInventoryServerRpc()
    // {
    //     for (int i = 0; i < inventorySize; i++)
    //         inventory.Add(new Slot(-1, 0));
    // }
    // private void PopulateEmptyInventory()
    // {
    //     if (IsServer)
    //     {
    //         for (int i = 0; i < inventorySize; i++)
    //             inventory.Add(new Slot(-1, 0));
    //     }
    //     else
    //         PopulateEmptyInventoryServerRpc();

    //     dirty = true;
    // }

    // public void AddItem(int itemID, int quantity)
    // {
    //     int slotWithSameItem = GetFirstSlotIndexWithItem(itemID);
    //     int emptySlot = GetFirstEmptySlotIndex();

    //     if (slotWithSameItem == -1 && emptySlot == -1)
    //     {
    //         Debug.Log("No available slot");
    //     }
    //     else if (slotWithSameItem != -1)
    //     {
    //         if (IsServer)
    //             AddItem(slotWithSameItem, itemID, quantity);
    //         else
    //             AddItemServerRpc(slotWithSameItem, itemID, quantity);
    //     }
    //     else if (emptySlot != -1)
    //     {
    //         if (IsServer)
    //             AddItem(emptySlot, itemID, quantity);
    //         else
    //             AddItemServerRpc(emptySlot, itemID, quantity);
    //     }

    //     dirty = true;
    // }

    // private int GetFirstSlotIndexWithItem(int itemID)
    // {
    //     for (int i = 0; i < inventory.Count; i++)
    //     {
    //         if (inventory[i].ItemId == itemID)
    //             return i;
    //     }
    //     return -1;
    // }

    // private int GetFirstEmptySlotIndex()
    // {
    //     for (int i = 0; i < inventory.Count; i++)
    //     {
    //         if (inventory[i].ItemId == -1)
    //             return i;
    //     }
    //     return -1;
    // }

    // [ServerRpc(RequireOwnership = false)]
    // private void AddItemServerRpc(int slotIndex, int itemId, int quantity)
    // {
    //     if (slotIndex < 0 || slotIndex >= inventorySize)
    //     {
    //         Debug.LogError("Invalid slot index.");
    //         return;
    //     }

    //     if (CatalogueManager.Instance.ItemCatalogue.GetItemData(itemId) == null)
    //         return;

    //     var slot = inventory[slotIndex];
    //     if (slot.ItemId == itemId || slot.ItemId == -1)
    //     {
    //         inventory[slotIndex] = new Slot(itemId, slot.Quantity + quantity);
    //     }
    //     else
    //     {
    //         Debug.LogError("Slot is occupied by a different item.");
    //     }
    // }
    // private void AddItem(int slotIndex, int itemId, int quantity)
    // {
    //     if (slotIndex < 0 || slotIndex >= inventorySize)
    //     {
    //         Debug.LogError("Invalid slot index.");
    //         return;
    //     }

    //     if (CatalogueManager.Instance.ItemCatalogue.GetItemData(itemId) == null)
    //         return;

    //     var slot = inventory[slotIndex];
    //     if (slot.ItemId == itemId || slot.ItemId == -1)
    //     {
    //         inventory[slotIndex] = new Slot(itemId, slot.Quantity + quantity);
    //     }
    //     else
    //     {
    //         Debug.LogError("Slot is occupied by a different item.");
    //     }
    // }

    // // Move item from one slot to another in the main inventory
    // private void MoveItem(int fromIndex, int toIndex)
    // {
    //     if (fromIndex < 0 || fromIndex >= inventorySize || toIndex < 0 || toIndex >= inventorySize)
    //     {
    //         Debug.LogError("Invalid slot index.");
    //         return;
    //     }

    //     Slot fromSlot = inventory[fromIndex];
    //     Slot toSlot = inventory[toIndex];

    //     if (toSlot.ItemId == -1 || toSlot.ItemId == fromSlot.ItemId)
    //     {
    //         inventory[toIndex] = new Slot(fromSlot.ItemId, toSlot.Quantity + fromSlot.Quantity);
    //         inventory[fromIndex] = new Slot(-1, 0);
    //         dirty = true;
    //     }
    //     else
    //     {
    //         // Debug.LogError("Target slot is occupied by a different item.");
    //         SwapItems(fromIndex, toIndex);
    //     }
    // }

    // // Swap items between two slots in the main inventory
    // private void SwapItems(int index1, int index2)
    // {
    //     if (index1 < 0 || index1 >= inventorySize || index2 < 0 || index2 >= inventorySize)
    //     {
    //         Debug.LogError("Invalid slot index.");
    //         return;
    //     }

    //     Slot temp = inventory[index1];
    //     inventory[index1] = inventory[index2];
    //     inventory[index2] = temp;
    //     dirty = true;
    // }
}