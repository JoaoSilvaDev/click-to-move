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
    protected Color defaultColor;

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
        defaultColor = rend.material.color;
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
