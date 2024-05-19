using UnityEngine;
using Unity.Netcode;
using Unity.AI.Navigation;

public class Door : Interactable
{
    public NavMeshLink navMeshLink;
    public Vector3 openRotation, closeRotation;
    private Color defaultColor;
    private float lerpSpeed = 5f;

    private NetworkVariable<bool> open = new NetworkVariable<bool>(
        false,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        open.OnValueChanged += OnOpenChanged;
        navMeshLink.activated = open.Value;
        rend.enabled = !open.Value;

        rend.material = Instantiate(rend.material);
        defaultColor = rend.material.color;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(open.Value ? openRotation : closeRotation),
            lerpSpeed * Time.deltaTime);
    }

    public override void Hover()
    {
        base.Hover();
        rend.material.color = defaultColor + new Color(0.2f, 0.2f, 0.2f);
    }

    public override void Unhover()
    {
        base.Unhover();
        rend.material.color = defaultColor;
    }

    public override void Interact(Player interactor)
    {
        base.Interact(interactor);
        Toggle();
    }

    private void Toggle()
    {
        SetOpen(!open.Value);
    }

    public void SetOpen(bool value)
    {
        if (IsServer)
            open.Value = value;
        else
            SetOpenServerRpc(value);
    }

    private void OnOpenChanged(bool previousValue, bool newValue)
    {
        // animate
        // set navmesh thing
        navMeshLink.activated = newValue;
        // rend.enabled = !newValue;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetOpenServerRpc(bool value)
    {
        open.Value = value;
    }

}
