using UnityEngine;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using System;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;

public class ClientManager : MonoBehaviour
{
    public static ClientManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public async void StartClient(string joinCode)
    {
        JoinAllocation allocation;

        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"JoinAllocation failed. Error:{e}");
            throw;
        }

        Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"client: {allocation.AllocationId}");

        var relayServerData = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();
    }
}
