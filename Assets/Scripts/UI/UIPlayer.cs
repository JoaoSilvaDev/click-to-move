using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIPlayer : MonoBehaviour
{
    public TextMeshProUGUI playerID;
    public UIItemList itemList;
    private PlayerInventory playerInventory;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnLocalClientIdChanged;
    }

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

    private void OnLocalClientIdChanged(ulong newClientID)
    {
        playerID.text = $"client id: {NetworkManager.Singleton.LocalClientId}";
    }

    private void OnInventoryChanged(NetworkList<InventoryItem> items)
    {
        // print("OnInventoryChanged");
        itemList.DisplayItems(items);
    }
}
