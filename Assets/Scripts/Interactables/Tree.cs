using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class Tree : Interactable
{
    public int baseHealth = 4;
    public ItemData item;
    public int itemQuantityMin = 1;
    public int itemQuantityMax = 1;

    private NetworkVariable<int> health = new NetworkVariable<int>(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private Color hoverColor;
    private Color interactColor;
    private float interactionTimer = 0f; // total interaction time
    private float hitTimer = 0f; // hit interaction time
    private float interactorHitFrequency = 100f;
    // time between each hit (this is fetched fom the respective interactor at runtime)

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        health.OnValueChanged += OnHealthChanged;
        SetHealth(baseHealth);

        hoverColor = defaultColor + new Color(0.2f, 0.2f, 0.2f);
        interactColor = defaultColor + new Color(0.2f, -0.2f, -0.2f);
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
        SetHealth(baseHealth);
    }

    public override void OnNetworkInteract()
    {
        base.OnNetworkInteract();
        OnInteractVisuals();
        interactionTimer = 0f;
        hitTimer = 0f;
        // here we use GetPlayerByClientID because its more performant to just store the interactorId ulong and then fetch the player locally
        // than to send the whole Player class instance through the network
        interactorHitFrequency = PlayerManager.Instance.GetPlayerByClientID(interactorId.Value).miningFrequency;
    }

    public override void OnNetworkRelease()
    {
        base.OnNetworkRelease();
        OnReleaseVisuals();
        interactionTimer = 0f;
        hitTimer = 0f;
        interactorHitFrequency = 100f;
    }

    private void Update()
    {
        if (isBeingInteracted.Value)
        {
            interactionTimer += Time.deltaTime;
            hitTimer += Time.deltaTime;

            if (hitTimer >= interactorHitFrequency)
            {
                hitTimer = 0;
                Hit();
            }
        }
    }

    private void OnInteractVisuals()
    {
        // rend.material.color = interactColor;
    }

    private void OnReleaseVisuals()
    {
        // rend.material.color = IsBeingHovered ? hoverColor : defaultColor;
    }

    private void Hit()
    {
        OnHitVisuals();
        SetHealth(health.Value - 1);
    }

    private void OnHitVisuals()
    {
        transform.DOShakePosition(duration: 0.15f, strength: 0.3f, vibrato: 50);
    }

    #region NetworkVariable health

    public void SetHealth(int value)
    {
        if (IsServer)
            SetHealthValue(value);
        else
            SetHealthValueServerRpc(value);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetHealthValueServerRpc(int value) { SetHealthValue(value); }
    private void SetHealthValue(int value) { health.Value = value; }
    private void OnHealthChanged(int previousValue, int newValue)
    {
        if (newValue <= 0)
        {
            SetVisible(false);
            SetCanInteract(false);
            if (interactor)
                interactor.inventory.AddItem(item.ID, (uint)Random.Range(itemQuantityMin, itemQuantityMax));
        }
    }

    #endregion

    protected override void OnVisibleChanged(bool previousValue, bool newValue)
    {
        base.OnVisibleChanged(previousValue, newValue);
        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
            r.enabled = newValue;
    }
}
