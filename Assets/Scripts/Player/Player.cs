using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Header("Components")]
    public PlayerMovement movement;
    public PlayerInteraction interaction;
    public PlayerModel model;
    public PlayerInventory inventory;

    [Header("Gameplay Settings")]
    public float miningFrequency { get; private set; } = 0.4f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        PlayerManager.Instance.AddPlayer(this);
        
        if (IsOwner)
        {
            gameObject.name = $"Player {NetworkManager.Singleton.LocalClientId}";
            movement.Init(this);
            interaction.Init(this);
        }
    }
}
