using DG.Tweening;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class InteractableResource : NetworkBehaviour
{
    [Header("Settings")]
    public int baseHealth = 4;
    public ItemData item;
    public int itemDropQuantityMin = 1;
    public int itemDropQuantityMax = 1;

    [Header("Visuals")]
    public Color hoverColorAdd = new Color(0.2f, 0.2f, 0.2f);
    public Color interactColorAdd = new Color(0.2f, -0.2f, -0.2f);

    [Header("Events")]
    public UnityEvent OnDestroyResource;

    // Hitting
    private float interactionTimer = 0f; // total interaction time
    private float hitTimer = 0f; // hit interaction time
    private float interactorHitFrequency = 100f;
    // time between each hit (this is fetched fom the respective interactor at runtime)

    private NetworkVariable<int> health = new NetworkVariable<int>(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private Interactable interactable;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();

        interactable.OnHover += OnHover;
        interactable.OnUnhover += OnUnhover;

        interactable.OnClientInteract += OnClientInteract;
        interactable.OnClientRelease += OnClientRelease;

        interactable.OnNetworkInteractCallback += OnNetworkInteract;
        interactable.OnNetworkReleaseCallback += OnNetworkRelease;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        health.OnValueChanged += OnHealthChanged;
        SetHealth(baseHealth);
    }

    private void Update()
    {
        if (interactable.isBeingInteracted.Value)
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

    private void OnHover()
    {
        interactable.renderers[0].material.color = interactable.defaultColor + hoverColorAdd;
    }

    private void OnUnhover()
    {
        interactable.renderers[0].material.color = interactable.defaultColor;
    }

    private void OnClientInteract(Player interactor)
    {
        OnInteractVisuals();
    }

    private void OnClientRelease(Player interactor)
    {
        OnReleaseVisuals();
    }

    private void OnNetworkInteract()
    {
        OnInteractVisuals();
        interactionTimer = 0f;
        hitTimer = 0f;
        // here we use GetPlayerByClientID because its more performant to just store the interactorId ulong and then fetch the player locally
        // than to send the whole Player class instance through the network
        interactorHitFrequency = PlayerManager.Instance.GetPlayerByClientID(interactable.interactorId.Value).miningFrequency;
        Hit();
    }

    private void OnNetworkRelease()
    {
        OnReleaseVisuals();
        interactionTimer = 0f;
        hitTimer = 0f;
        interactorHitFrequency = 100f;
        SetHealth(baseHealth);
    }

    private void OnInteractVisuals()
    {
        interactable.renderers[0].material.color = interactable.defaultColor + interactColorAdd;
    }

    private void OnReleaseVisuals()
    {
        if (interactable.IsBeingHovered)
            interactable.renderers[0].material.color = interactable.defaultColor + hoverColorAdd;
        else
            interactable.renderers[0].material.color = interactable.defaultColor;
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

    private void DestroyResource()
    {
        if (interactable.interactor)
            interactable.interactor.inventory.AddItem(item.ID, (uint)Random.Range(itemDropQuantityMin, itemDropQuantityMax + 1)); //+1 to make it inclusive

        interactable.SetCanInteract(false);

        // Disappearing visuals are handled by each resource
        // maybe this could just be an animation
        // but sometimes code is better :D

        print("OnDestroyResource");
        OnDestroyResource?.Invoke();
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
            DestroyResource();
    }

    #endregion
}
