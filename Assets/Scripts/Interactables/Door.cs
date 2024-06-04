using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class NewDoor : NetworkBehaviour
{
    public NavMeshObstacle navmeshObstacle;
    public Vector3 openRotation, closeRotation;
    public float lerpSpeed = 5f;

    private NetworkVariable<bool> open = new NetworkVariable<bool>(
        false,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private void Awake()
    {
        open.OnValueChanged += OnOpenChanged;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(open.Value ? openRotation : closeRotation),
            lerpSpeed * Time.deltaTime);
    }

    public void Toggle()
    {
        SetOpen(!open.Value);
    }

    #region NetworkVariable Open
    private void SetOpen(bool value)
    {
        if (IsServer)
            open.Value = value;
        else
            SetOpenServerRpc(value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetOpenServerRpc(bool value)
    {
        open.Value = value;
    }

    private void OnOpenChanged(bool previousValue, bool newValue)
    {
        if (navmeshObstacle)
            navmeshObstacle.enabled = !newValue;
    }
    #endregion
}
