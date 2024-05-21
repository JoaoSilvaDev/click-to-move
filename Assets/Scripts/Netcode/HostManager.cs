using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using System;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;

public class HostManager : MonoBehaviour
{
    [SerializeField] private int maxConnections = 4;
    public Dictionary<ulong, ClientData> ClientData { get; private set; }
    public string joinCode { get; private set; }

    public static HostManager instance { get; private set; }
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

    public async void StartHost()
    {
        Allocation allocation;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        }
        catch (Exception e)
        {
            Debug.Log($"sCreateAllocation failed. Eror: {e}");
            throw;
        }

        Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"server: {allocation.AllocationId}");

        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (Exception e)
        {
            Debug.Log($"GetJoinCode failed. Eror: {e}");
            throw;
        }

        var relayServerData = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartHost();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {

        if (ClientData.Count >= maxConnections)
        {
            response.Approved = false;
            return;
        }

        response.Approved = true;
    }

    private void OnNetworkReady()
    {

    }
}
