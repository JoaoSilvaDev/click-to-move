using UnityEngine;

public class TestInteractableBall : Interactable
{
    public Transform doorMode;

    private Color defaultColor;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        rend.material = Instantiate(rend.material);
        defaultColor = rend.material.color;
    }

    public override void Hover()
    {
        rend.material.color = defaultColor + new Color(0.2f, 0.2f, 0.2f);
    }

    public override void Unhover()
    {
        rend.material.color = defaultColor;
    }

    public override void Interact(Player interactor)
    {
        SetVisible(false);
    }
}
