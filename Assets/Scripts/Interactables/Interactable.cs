using UnityEngine;
using Unity.Netcode;

public class Interactable : NetworkBehaviour
{
    [Header("Visuals")]
    public MeshRenderer rend;

    private NetworkVariable<bool> visible = new NetworkVariable<bool>(
        true,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    // in-scene placed NetworkObjects: Awake -> Start -> OnNetworkSpawn
    // dynamically spawned NetworkObjects: Awake -> OnNetworkSpawn -> Start

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        visible.OnValueChanged += OnVisibleChanged;
        rend.enabled = visible.Value;

        rend.material = Instantiate(rend.material);
    }

    public virtual void Hover() { }

    public virtual void Unhover() { }

    public virtual void Interact(Player interactor) { }

    #region Visibility
    public void SetVisible(bool value)
    {
        if (IsServer)
            visible.Value = value;
        else
            SetVisibleServerRpc(value);
    }

    private void OnVisibleChanged(bool previousValue, bool newValue)
    {
        rend.enabled = newValue;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetVisibleServerRpc(bool value)
    {
        visible.Value = value;
    }
    #endregion
}
