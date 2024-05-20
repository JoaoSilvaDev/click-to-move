using UnityEngine;
using Unity.Netcode;

public class PlayerModel : NetworkBehaviour
{
    public MeshRenderer mesh;

    public NetworkVariable<Color> color = new NetworkVariable<Color>(
        Color.white,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        color.OnValueChanged += OnColorChanged;
        mesh.material.color = color.Value;

        if (IsOwner)
            SetColor(Color.HSVToRGB(Random.Range(0f, 1f), 0.5f, 0.7f));
    }

    private void OnColorChanged(Color previousValue, Color newValue)
    {
        mesh.material.color = color.Value;
    }

    private void SetColor(Color value)
    {
        if (IsServer)
            SetColorValue(value);
        else
            SetColorServerRpc(value);
    }
    private void SetColorValue(Color value) { color.Value = value; }
    [ServerRpc(RequireOwnership = false)]
    private void SetColorServerRpc(Color value) { SetColorValue(value); }
}
