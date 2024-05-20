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
        SetVisible(false);
        SetCanInteract(false);
        interactor.inventory.SetItem(new InventoryItem(item.ID, (uint)Random.Range(0, 20)), Random.Range(0, 8));
    }
}
