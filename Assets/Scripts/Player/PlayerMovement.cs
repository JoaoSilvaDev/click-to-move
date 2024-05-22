using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System.Collections;

public class PlayerMovement : NetworkBehaviour
{
    private Camera cam;
    private Player player;
    public Interactable targetInteractable { get; private set; }

    // navmesh
    private NavMeshAgent navmeshAgent;
    private bool moveAcrossNavMeshesStarted = false;
    private Vector3 offMeshLinkStart;
    private Vector3 offMeshLinkEnd;
    private float offMeshLinkDuration;
    private float offMeshLinkTimer = 0f;

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

        if (navmeshAgent.isOnOffMeshLink && !moveAcrossNavMeshesStarted)
            StartNavmeshLinkTraversal();

        if (moveAcrossNavMeshesStarted)
            UpdateNavmeshLinkTraversal();

        if (targetInteractable)
        {
            if (player.interaction.TryInteract(targetInteractable))
            {
                targetInteractable = null;
                Stop();
            }
        }
    }

    #region Input
    private void OnClick()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                if (IsPathPossible(hit.point))
                {
                    targetInteractable = null;
                    SetDestination(hit.point);
                }
            }
        }
    }
    #endregion

    #region Navmesh Link Traversal
    private void StartNavmeshLinkTraversal()
    {
        offMeshLinkStart = navmeshAgent.transform.position;
        offMeshLinkEnd = navmeshAgent.currentOffMeshLinkData.endPos + Vector3.up * navmeshAgent.baseOffset;
        offMeshLinkDuration = (offMeshLinkEnd - offMeshLinkStart).magnitude / navmeshAgent.speed;

        navmeshAgent.updateRotation = false;
        offMeshLinkTimer = 0f;
        moveAcrossNavMeshesStarted = true;
    }

    private void UpdateNavmeshLinkTraversal()
    {
        offMeshLinkTimer += Time.deltaTime;
        float progress = offMeshLinkTimer / offMeshLinkDuration;
        transform.position = Vector3.Lerp(offMeshLinkStart, offMeshLinkEnd, progress);
        // SetDestination(transform.position);
        if (progress >= 1f)
        {
            transform.position = offMeshLinkEnd;
            // SetDestination(offMeshLinkEnd);

            navmeshAgent.updateRotation = true;
            navmeshAgent.CompleteOffMeshLink();
            moveAcrossNavMeshesStarted = false;
        }
    }
    #endregion

    #region Navmesh Pathfinding
    public void SetDestination(Vector3 destination)
    {
        navmeshAgent.SetDestination(destination);
    }
    public void SetDestination(Interactable interactable)
    {
        Vector3 point = interactable.GetClosestPoint(transform.position);
        point.y = transform.position.y;
        navmeshAgent.SetDestination(point);
    }

    public void SetTargetInteractable(Interactable interactable)
    {
        if (interactable == targetInteractable) return;

        targetInteractable = interactable;
        if (IsPathPossible(interactable.transform.position))
        {
            SetDestination(interactable);
        }
        else
        {
            Vector3 closestPointOnEdge = GetClosestPointOnEdge(interactable);
            SetDestination(closestPointOnEdge);
        }
    }

    private void Stop()
    {
        SetDestination(transform.position);
    }

    private Vector3 GetClosestPointOnEdge(Interactable interactable)
    {
        // Get the interactable's closest point
        Vector3 closestPoint = interactable.GetClosestPoint(transform.position);

        // Sample the NavMesh at the closest point, using interactionRange
        if (NavMesh.SamplePosition(closestPoint, out NavMeshHit hit, player.interaction.interactionRange, NavMesh.AllAreas))
            return hit.position;

        // If we can't find a valid point on the NavMesh, fall back to the original position
        return closestPoint;
    }

    bool IsPathPossible(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        navmeshAgent.CalculatePath(targetPosition, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }
    #endregion
}
