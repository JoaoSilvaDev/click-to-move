using UnityEngine.AI;
using Unity.Netcode;
using UnityEngine;

// this class is used to sync NavmeshObstacle settings
public class NetworkedNavmeshObstacle : NetworkBehaviour
{
    public bool startActive = true;
    public NavMeshObstacle navmeshObstacle;

    private NetworkVariable<bool> obstacleActive = new NetworkVariable<bool>(
        true,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private void Awake()
    {
        navmeshObstacle = GetComponent<NavMeshObstacle>();
        obstacleActive.OnValueChanged += OnObstacleActiveChanged;
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.J))
    //         SetObstacleActive(!obstacleActive.Value);
    // }

    #region NetworkVariable enable
    public void SetObstacleActive(bool value)
    {
        if (IsServer)
        {
            // print($"obstacleActive.Value = {value}");
            obstacleActive.Value = value;
        }
        else
            SetObstacleActiveServerRpc(value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetObstacleActiveServerRpc(bool value)
    {
        // print($"obstacleActive.Value = {value}");
        obstacleActive.Value = value;
    }

    private void OnObstacleActiveChanged(bool previousValue, bool newValue)
    {
        // print($"OnObstacleActiveChanged{newValue}");
        navmeshObstacle.enabled = newValue;
    }
    #endregion
}
