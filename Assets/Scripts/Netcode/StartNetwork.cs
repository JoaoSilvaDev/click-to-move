using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using TMPro;

public class StartNetworkMenu : MonoBehaviour
{
    public Canvas connectingPanel;
    public Canvas menuPanel;
    public TMP_InputField joinCodeInput;

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }

        connectingPanel.enabled = false;
        menuPanel.enabled = true;
    }

    public void StartHost()
    {
        HostManager.instance.StartHost();
    }

    public void StartClient()
    {
        ClientManager.instance.StartClient(joinCodeInput.text);
    }
}