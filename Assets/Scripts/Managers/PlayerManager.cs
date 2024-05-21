using System.Collections.Generic;
using Unity.Netcode;

public class PlayerManager : Singleton<PlayerManager>
{
    public List<Player> players = new List<Player>();

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public Player GetPlayerByClientID(ulong clientID)
    {
        foreach (Player player in players)
        {
            if (clientID == player.OwnerClientId)
                return player;
        }
        return null;
    }
}
