using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public PlayerMovement movement;
    public PlayerInteraction interaction;
    public PlayerModel model;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        PlayerManager.Instance.AddPlayer(this);

        movement.Init(this);
        interaction.Init(this);
        model.Init(this);
    }
}
