using UnityEngine;

public class Lever : Interactable
{
    public Interactable remoteInteractable;

    public override void Hover()
    {
        base.Hover();
        renderers[0].material.color = defaultColor + new Color(0.2f, 0.2f, 0.2f);
    }

    public override void Unhover()
    {
        base.Unhover();
        renderers[0].material.color = defaultColor;
    }

    public override void ClientInteract(Player interactor)
    {
        base.ClientInteract(interactor);
        remoteInteractable.ClientInteract(interactor);
    }
}
