using System;

[Serializable]
public class ClientData
{
    public ulong clientId;

    public ClientData(ulong clientId)
    {
        this.clientId = clientId;
    }
}
