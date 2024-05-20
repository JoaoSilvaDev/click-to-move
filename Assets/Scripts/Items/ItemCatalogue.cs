using UnityEngine;

[CreateAssetMenu(menuName = "DEMO/Item Catalogue")]
public class ItemCatalogue : ScriptableObject
{
    [SerializeField] private ItemData[] items;
    public ItemData[] Items { get { return items; } }

    public ItemData GetItemData(uint itemID)
    {
        foreach (ItemData data in items)
        {
            if (data.ID == itemID)
                return data;
        }
        Debug.Log($"Invalid item ID: {itemID}");
        return null;
    }
}