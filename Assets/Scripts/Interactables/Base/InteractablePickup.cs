using Unity.Netcode;
using UnityEngine;

public class InteractablePickup : NetworkBehaviour
{
    [Header("Item")]
    public ItemData item;
    public int itemQuantityMin = 1;
    public int itemQuantityMax = 1;

    [Header("Visuals")]
    public Color hoverColorAdd = new Color(0.2f, 0.2f, 0.2f);

    private Interactable interactable;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.OnClientInteract += OnClientInteract;
        interactable.OnHover += OnHover;
        interactable.OnUnhover += OnUnhover;
    }

    private void OnHover()
    {
        interactable.renderers[0].material.color = interactable.defaultColor + new Color(0.2f, 0.2f, 0.2f);
    }

    private void OnUnhover()
    {
        interactable.renderers[0].material.color = interactable.defaultColor;
    }

    private void OnClientInteract(Player interactor)
    {
        bool addedToInventory = interactor.inventory.AddItem(
            item.ID, (uint)Random.Range(itemQuantityMin, itemQuantityMax + 1)); //+1 to make it inclusive

        if (addedToInventory)
        {
            interactable.SetVisible(false);
            interactable.SetCanInteract(false);
        }
    }
}
