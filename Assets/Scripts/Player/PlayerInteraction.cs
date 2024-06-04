using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerInteraction : NetworkBehaviour
{
    public float interactionRange = 2f;
    private Camera cam;
    private Interactable hoveredInteractable;
    private Interactable interactingInteractable;
    private Player player;
    public bool IsHoveringInteractable { get { return hoveredInteractable != null; } }
    public bool IsInteractingInteractable { get { return interactingInteractable != null; } }

    public void Init(Player player)
    {
        this.player = player;
        cam = Camera.main;
    }

    private void Update()
    {
        if (!IsOwner) return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Interactable newInteractable;
            if (hit.collider.gameObject.TryGetComponent<Interactable>(out newInteractable))
            {
                if (newInteractable != hoveredInteractable)
                    SelectInteractable(newInteractable);
            }
            else
            {
                DeselectCurentInteractable();
            }
        }
        else
        {
            DeselectCurentInteractable();
        }

        if (IsHoveringInteractable && Input.GetMouseButton(0))
            TryInteract(hoveredInteractable);

        if (IsInteractingInteractable && Input.GetMouseButtonUp(0))
            TryRelease(interactingInteractable);
    }

    public void DeselectCurentInteractable()
    {
        if (IsInteractingInteractable)
        {
            interactingInteractable.ClientRelease(player);
            interactingInteractable = null;
        }

        if (IsHoveringInteractable)
        {
            hoveredInteractable.Unhover();
            hoveredInteractable = null;
        }
    }

    private void SelectInteractable(Interactable interactable)
    {
        DeselectCurentInteractable();
        interactable.Hover();
        hoveredInteractable = interactable;
    }

    public bool TryInteract(Interactable interactable)
    {
        if (!interactable.canInteract.Value) return false;
        if (interactingInteractable == interactable) return false;

        Vector3 closestPoint = interactable.GetClosestPoint(transform.position);
        closestPoint.y = transform.position.y;
        if (Vector3.Distance(transform.position, closestPoint) > interactionRange)
        {
            player.movement.SetTargetInteractable(interactable);
            return false;
        }
        else
        {
            Interact(interactable);
            return true;
        }
    }

    public bool TryRelease(Interactable interactable)
    {
        if (!IsInteractingInteractable) return false;
        if (!interactable.canInteract.Value) return false;

        Release(interactable);
        return true;
    }

    private void Interact(Interactable interactable)
    {
        interactingInteractable = interactable;
        interactable.ClientInteract(player);
    }

    private void Release(Interactable interactable)
    {
        interactingInteractable = null;
        interactable.ClientRelease(player);
    }

    private void OnDrawGizmos()
    {
        // if (hoveredInteractable)
        // {
        //     Vector3 closestPoint = hoveredInteractable.GetClosestPoint(transform.position);
        //     closestPoint.y = transform.position.y;

        //     if (Vector3.Distance(transform.position, closestPoint) > interactionRange)
        //         Gizmos.color = Color.red;
        //     else
        //         Gizmos.color = Color.green;

        //     Gizmos.DrawWireSphere(closestPoint, 0.5f);
        //     Gizmos.DrawLine(transform.position, closestPoint);
        //     print(Vector3.Distance(transform.position, closestPoint));
        // }
    }
}
