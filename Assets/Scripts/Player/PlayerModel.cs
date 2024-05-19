using UnityEngine;
using Unity.Netcode;

public class PlayerModel : NetworkBehaviour
{
    public MeshRenderer mesh;
    
    public NetworkVariable<Color> color = new(Color.white,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner);
    
    private Player player;

    public void Init(Player player)
    {
        this.player = player;
        // Setup variable listeners
        color.OnValueChanged += OnColorChanged;

        if (IsOwner)
        {
            color.Value = Color.HSVToRGB(Random.Range(0f, 1f), 0.5f, 0.7f);
        }
        else
        {
            SetMeshColor(color.Value);
        }
    }

    private void OnColorChanged(Color previousValue, Color newValue)
    {
        SetMeshColor(newValue);
    }

    private void SetMeshColor(Color color)
    {
        mesh.material.color = color;
    }
}
