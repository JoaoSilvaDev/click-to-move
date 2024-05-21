using Unity.Netcode;
using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    public int serverTarget = 60;
    public int hostTarget = 120;
    public int clientTarget = 120;
    public int serverVSyncCount = 0;
    public int hostVSyncCount = 0;
    public int clientVSyncCount = 0;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            QualitySettings.vSyncCount = serverVSyncCount;
            Application.targetFrameRate = serverTarget;
        }
        else if (NetworkManager.Singleton.IsHost)
        {
            QualitySettings.vSyncCount = hostVSyncCount;
            Application.targetFrameRate = hostTarget;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            QualitySettings.vSyncCount = clientVSyncCount;
            Application.targetFrameRate = clientTarget;
        }
    }
}
