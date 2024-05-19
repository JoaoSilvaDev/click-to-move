using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 2;
    public float rotSpeed = 8;

    private NavMeshAgent navmeshAgent;
    private Camera cam;
    private Player player;
    private Interactable targetInteractable;

    public void Init(Player player)
    {
        this.player = player;
        cam = Camera.main;
        navmeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButton(1)) //!player.interaction.IsHoveringInteractable)
                OnClick();

        if (targetInteractable)
        {
            if (Vector3.Distance(targetInteractable.transform.position, transform.position) < player.interaction.interactionRange)
            {
                if (player.interaction.TryInteract(targetInteractable))
                    targetInteractable = null;
            }
        }
    }

    private void OnClick()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
                SetDestination(hit.point);
        }
    }

    public void SetDestination(Vector3 destination)
    {
        player.interaction.DeselectCurentInteractable();
        navmeshAgent.SetDestination(destination);
    }

    public void SetTargetInteractable(Interactable interactable)
    {
        targetInteractable = interactable;
        Vector3 closestPointOnEdge = FindClosestPointOnEdge(interactable);
        SetDestination(closestPointOnEdge);
    }

    private Vector3 FindClosestPointOnEdge(Interactable interactable)
    {
        // Get the interactable's bounds
        Collider collider = interactable.GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogWarning("Interactable object has no collider.");
            return interactable.transform.position;
        }

        Vector3 closestPoint = collider.ClosestPointOnBounds(navmeshAgent.transform.position);

        // Sample the NavMesh at the closest point, using interactionRange
        if (NavMesh.SamplePosition(closestPoint, out NavMeshHit hit, player.interaction.interactionRange, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // If we can't find a valid point on the NavMesh, fall back to the original position
        return closestPoint;
    }
}
