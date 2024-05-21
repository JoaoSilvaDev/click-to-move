using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class NetworkInitializer : MonoBehaviour
{
    private async void Start()
    {
        UI.instance.SetAuthenticating();

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

        UI.instance.SetLobbyMenu();
    }

    public void StartHost()
    {
        HostManager.instance.StartHost();
        UI.instance.SetConnecting();
    }

    public void StartClient()
    {
        ClientManager.instance.StartClient(UI.instance.joinCodeInput.text);
        UI.instance.SetConnecting();
    }
}