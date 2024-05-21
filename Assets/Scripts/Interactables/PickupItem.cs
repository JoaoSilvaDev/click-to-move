using UnityEngine;

public class PickupItem : Interactable
{
    public ItemData item;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public override void Hover()
    {
        base.Hover();
        rend.material.color = defaultColor + new Color(0.2f, 0.2f, 0.2f);
    }

    public override void Unhover()
    {
        base.Unhover();
        rend.material.color = defaultColor;
    }

    public override void Interact(Player interactor)
    {
        base.Interact(interactor);
        if (interactor.inventory.AddItem(new InventoryItem(item.ID, 1)))
        {
            SetVisible(false);
            SetCanInteract(false);
        }
    }
}
