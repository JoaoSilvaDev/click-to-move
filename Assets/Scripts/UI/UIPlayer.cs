using Unity.Netcode;

public class UIPlayer : NetworkBehaviour
{
    public UIItemList itemList;
    private PlayerInventory playerInventory;

    private void Update()
    {
        if (!playerInventory)
        {
            if (PlayerManager.Instance.GetPlayerByClientID(NetworkManager.Singleton.LocalClientId))
            {
                playerInventory = PlayerManager.Instance.GetPlayerByClientID(NetworkManager.Singleton.LocalClientId).inventory;
                playerInventory.OnInventoryChanged += OnInventoryChanged;
            }
        }
    }

    private void OnInventoryChanged(NetworkList<InventoryItem> items)
    {
        itemList.DisplayItems(items);
    }
}
