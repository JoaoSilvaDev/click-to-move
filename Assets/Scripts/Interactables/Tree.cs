using UnityEngine;

public class Tree : Interactable
{
    private Color hoverColor;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        hoverColor = defaultColor + new Color(0.2f, 0.2f, 0.2f);
    }

    public override void Hover()
    {
        base.Hover();
        rend.material.color = hoverColor;
    }

    public override void Unhover()
    {
        base.Unhover();
        rend.material.color = defaultColor;
    }

    public override void Interact(Player interactor)
    {
        base.Interact(interactor);
        OnInteractVisuals();
    }

    public override void Release(Player interactor)
    {
        base.Release(interactor);
        OnReleaseVisuals();
    }

    public override void OnNetworkInteract()
    {
        base.OnNetworkInteract();
        OnInteractVisuals();
    }

    public override void OnNetworkRelease()
    {
        base.OnNetworkRelease();
        OnReleaseVisuals();
    }

    private void OnInteractVisuals()
    {
        rend.material.color = Color.red;
    }

    private void OnReleaseVisuals()
    {
        rend.material.color = IsBeingHovered ? hoverColor : defaultColor;
    }
}
