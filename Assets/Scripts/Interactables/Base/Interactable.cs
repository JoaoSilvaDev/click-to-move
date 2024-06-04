using UnityEngine;
using Unity.Netcode;
using System;

// interactable are objects that can be clicked on, some examples
// things that cause an action (ex: door, open close)
// things you can break and get items from  (ex: trees, rocks)
// things that cause an action on other things (activatables) (ex: lever to draw bridge, sign that opens a read UI)
// things you can pick up (ex: bones, leaves) (requires inventory)
public class Interactable : NetworkBehaviour
{
    [Header("Visuals")]
    public MeshRenderer[] renderers;
    public Collider coll;
    [HideInInspector] public Color defaultColor;

    private NetworkVariable<bool> visible = new NetworkVariable<bool>(
        true,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> canInteract = new NetworkVariable<bool>(
        true,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    [HideInInspector]
    public NetworkVariable<bool> isBeingInteracted = new NetworkVariable<bool>(
        false,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    [HideInInspector]
    public NetworkVariable<ulong> interactorId = new NetworkVariable<ulong>(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    [HideInInspector]
    public Player interactor;
    public bool IsBeingHovered { get; private set; } = false; // should not be seen by other players

    // in-scene placed NetworkObjects: Awake -> Start -> OnNetworkSpawn
    // dynamically spawned NetworkObjects: Awake -> OnNetworkSpawn -> Start

    public Action OnHover;
    public Action OnUnhover;
    public Action<Player> OnClientInteract;
    public Action<Player> OnClientRelease;
    public Action OnNetworkInteractCallback;
    public Action OnNetworkReleaseCallback;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        visible.OnValueChanged += OnVisibleChanged;
        isBeingInteracted.OnValueChanged += OnIsBeingInteractedChange;
        canInteract.OnValueChanged += OnCanInteractChange;
        SetRenderers(visible.Value);

        renderers[0].material = Instantiate(renderers[0].material);
        defaultColor = renderers[0].material.color;
    }

    public void SetRenderers(bool enabled)
    {
        foreach (MeshRenderer r in renderers)
            r.enabled = enabled;
    }

    #region Base Interaction
    public virtual void Hover() { IsBeingHovered = true; OnHover?.Invoke(); } // executed only on the client
    public virtual void Unhover() { IsBeingHovered = false; OnUnhover?.Invoke(); } // executed only on the client


    // these are only on the client
    // but they can, ofcourse call things on all clients (like SetVisible())
    public virtual void ClientInteract(Player interactor) // executed only on the client
    {
        SetIsBeingInteracted(true);
        SetInteractor(interactor);
        OnClientInteract?.Invoke(interactor);
    }
    public virtual void ClientRelease(Player interactor) // executed only on the client
    {
        SetIsBeingInteracted(false);
        SetInteractor(null);
        OnClientRelease?.Invoke(interactor);
    }

    // these are callbacks that are called on all clients when a player starts interacting with it
    // so if we want to do something on ALL clients when something is interacted with
    public virtual void NetworkInteractCallback() { OnNetworkInteractCallback?.Invoke(); } // executed on every client
    public virtual void NetworkReleaseCallback() { OnNetworkReleaseCallback?.Invoke(); } // executed on every client
    #endregion

    #region NetworkVariable visible

    public void SetVisible(bool value)
    {
        if (IsServer)
            SetVisibleValue(value);
        else
            SetVisibleValueServerRpc(value);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetVisibleValueServerRpc(bool value) { SetVisibleValue(value); }
    private void SetVisibleValue(bool value) { visible.Value = value; }

    protected virtual void OnVisibleChanged(bool previousValue, bool newValue)
    {
        SetRenderers(newValue);
    }

    #endregion

    #region NetworkVariable canInteract

    public void SetCanInteract(bool value)
    {
        if (IsServer)
            SetCanInteractValue(value);
        else
            SetCanInteractValueServerRpc(value);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetCanInteractValueServerRpc(bool value) { SetCanInteractValue(value); }
    private void SetCanInteractValue(bool value) { canInteract.Value = value; }
    private void OnCanInteractChange(bool previousValue, bool newValue)
    {
        coll.enabled = newValue;
    }

    #endregion

    #region NetworkVariable isBeingInteracted

    public void SetIsBeingInteracted(bool value)
    {
        if (IsServer)
            SetIsBeingInteractedValue(value);
        else
            SetIsBeingInteractedValueServerRpc(value);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetIsBeingInteractedValueServerRpc(bool value) { SetIsBeingInteractedValue(value); }
    private void SetIsBeingInteractedValue(bool value) { isBeingInteracted.Value = value; }
    private void OnIsBeingInteractedChange(bool previousValue, bool newValue)
    {
        if (newValue)
            NetworkInteractCallback();
        else
            NetworkReleaseCallback();
    }

    #endregion

    #region NetworkVariable interactor

    public void SetInteractor(Player interactor)
    {
        this.interactor = interactor;

        if (interactor == null)
            SetInteractorId(0);
        else
            SetInteractorId(interactor.OwnerClientId);
    }

    private void SetInteractorId(ulong Id)
    {
        if (IsServer)
            SetInteractorIdValue(Id);
        else
            SetInteractorIdValueServerRpc(Id);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetInteractorIdValueServerRpc(ulong value) { SetInteractorIdValue(value); }
    private void SetInteractorIdValue(ulong value) { interactorId.Value = value; }

    #endregion

    public Vector3 GetClosestPoint(Vector3 point)
    {
        return coll.ClosestPoint(point);
    }
}
