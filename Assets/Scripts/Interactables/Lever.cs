using UnityEngine;

public class Lever : Interactable
{
    public Interactable remoteInteractable;

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
        print("remoteInteractable.Interact");
        remoteInteractable.Interact(interactor);
    }
}
