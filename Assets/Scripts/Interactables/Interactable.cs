using UnityEngine;
using Unity.Netcode;
using System;
using System.Reflection;

// interactable are objects that can be clicked on, some examples
// things that cause an action (ex: door, open close)
// things you can break and get items from  (ex: trees, rocks)
// things that cause an action on other things (activatables) (ex: lever to draw bridge, sign that opens a read UI)
// things you can pick up (ex: bones, leaves) (requires inventory)
public class Interactable : NetworkBehaviour
{
    [Header("Visuals")]
    public MeshRenderer rend;
    protected Color defaultColor;

    private NetworkVariable<bool> visible = new NetworkVariable<bool>(
        true,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> canInteract = new NetworkVariable<bool>(
        true,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    // in-scene placed NetworkObjects: Awake -> Start -> OnNetworkSpawn
    // dynamically spawned NetworkObjects: Awake -> OnNetworkSpawn -> Start

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        canInteract.OnValueChanged += OnCanInteractChanged;
        visible.OnValueChanged += OnVisibleChanged;
        rend.enabled = visible.Value;

        rend.material = Instantiate(rend.material);
        defaultColor = rend.material.color;
    }

    public virtual void Hover() { }

    public virtual void Unhover() { }

    public virtual void Interact(Player interactor) { }

    #region Visibility

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

    #region Interactability

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

    private void OnCanInteractChanged(bool previousValue, bool newValue)
    {
    }

    #endregion
}
