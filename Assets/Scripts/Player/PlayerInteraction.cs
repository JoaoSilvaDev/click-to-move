using UnityEngine;
using Unity.Netcode;

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
            interactingInteractable.Release(player);
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

        if (Vector3.Distance(transform.position, interactable.GetClosestPoint(transform.position)) > interactionRange)
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
        interactable.Interact(player);
    }

    private void Release(Interactable interactable)
    {
        interactingInteractable = null;
        interactable.Release(player);
    }
}
