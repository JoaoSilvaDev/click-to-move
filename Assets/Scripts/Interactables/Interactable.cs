using UnityEngine;
using Unity.Netcode;

// interactable are objects that can be clicked on, some examples
// things that cause an action (ex: door, open close)
// things you can break and get items from  (ex: trees, rocks)
// things that cause an action on other things (activatables) (ex: lever to draw bridge, sign that opens a read UI)
// things you can pick up (ex: bones, leaves) (requires inventory)
public class Interactable : NetworkBehaviour
{
    [Header("Visuals")]
    public MeshRenderer rend;
    public Collider coll;
    protected Color defaultColor;

    private NetworkVariable<bool> visible = new NetworkVariable<bool>(
        true,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> canInteract = new NetworkVariable<bool>(
        true,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> isBeingInteracted = new NetworkVariable<bool>(
        false,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<ulong> interactorId = new NetworkVariable<ulong>(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    protected Player interactor;
    public bool IsBeingHovered { get; private set; } = false; // should not be seen by other players

    // in-scene placed NetworkObjects: Awake -> Start -> OnNetworkSpawn
    // dynamically spawned NetworkObjects: Awake -> OnNetworkSpawn -> Start

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (coll == null)
            coll = GetComponent<Collider>();

        visible.OnValueChanged += OnVisibleChanged;
        isBeingInteracted.OnValueChanged += OnIsBeingInteractedChange;
        rend.enabled = visible.Value;

        rend.material = Instantiate(rend.material);
        defaultColor = rend.material.color;
    }

    #region Base Interaction
    public virtual void Hover() { IsBeingHovered = true; } // executed only on the client
    public virtual void Unhover() { IsBeingHovered = false; } // executed only on the client

    public virtual void Interact(Player interactor) // executed only on the client
    {
        SetIsBeingInteracted(true);
        SetInteractor(interactor);
    }

    public virtual void Release(Player interactor) // executed only on the client
    {
        SetIsBeingInteracted(false);
        SetInteractor(null);
    }

    public virtual void OnNetworkInteract() { } // executed on every client
    public virtual void OnNetworkRelease() { } // executed on every client
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

    private void OnVisibleChanged(bool previousValue, bool newValue)
    {
        rend.enabled = newValue;
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
            OnNetworkInteract();
        else
            OnNetworkRelease();
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
