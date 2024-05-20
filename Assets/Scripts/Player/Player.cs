using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public PlayerMovement movement;
    public PlayerInteraction interaction;
    public PlayerModel model;
    public PlayerInventory inventory;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            gameObject.name = $"Player {NetworkManager.Singleton.LocalClientId}";
            PlayerManager.Instance.AddPlayer(this);
            movement.Init(this);
            interaction.Init(this);
        }
    }
}
