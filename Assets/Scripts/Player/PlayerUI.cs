using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI playerID;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnLocalClientIdChanged;
    }

    private void OnLocalClientIdChanged(ulong newClientID)
    {
        playerID.text = $"client id: {NetworkManager.Singleton.LocalClientId}";
    }
}
