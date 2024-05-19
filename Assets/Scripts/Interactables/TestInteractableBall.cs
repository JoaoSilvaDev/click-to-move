using UnityEngine;

public class TestInteractableBall : Interactable
{
    public Transform doorMode;

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
    }
}
